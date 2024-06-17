using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using System.Linq;

namespace JayoMidiControllerAnimator
{

    public class MidiAnimatorCore : MonoBehaviour
    {

        [Tooltip("The name of the MIDI input to recieve messages from")]
        public string MidiInputName;
        [Tooltip("The name of the MIDI output to repeat messages to.  Leave empty for no repeat.")]
        public string MidiOutputName;
        [Tooltip("The name of the second MIDI output to repeat messages to.  Leave empty for no repeat.")]
        public string MidiOutputTwoName;
        public Dictionary<int, List<IMidiNoteListener>> noteListeners;
        public Dictionary<int, List<IMidiControlListener>> controlListeners;

        public MainThreadDispatcher mainThread;
        private Dictionary<int, int> controlStates;
        private Dictionary<int, int> noteStates;
        private InputDevice midiInputDevice;
        private OutputDevice midiOutputDevice;
        private OutputDevice midiOutputDeviceTwo;


        public void Awake()
        {
            Debug.Log("Midi animator Core Awake!");
            EditorApplication.playModeStateChanged += ModeChanged;
            mainThread = gameObject.AddComponent<MainThreadDispatcher>();
            noteListeners = new Dictionary<int, List<IMidiNoteListener>>();
            controlListeners = new Dictionary<int, List<IMidiControlListener>>();
            noteStates = new Dictionary<int, int>();
            controlStates = new Dictionary<int, int>();
    }

        private void ModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("Exiting Play Mode");
                deInitMidi();
            }
        }

        private void Start()
        {
            initMidi();
        }

        private void OnEnable()
        {
            //initMidi();
        }

        private void OnDisable()
        {
            //deInitMidi();
        }

        private void OnDestroy()
        {
            deInitMidi();
        }

        private void OnApplicationQuit()
        {
            deInitMidi();
        }

        public bool initMidi()
        {
            if(midiInputDevice != null) { return true;  }
            Debug.Log("Starting MIDI stuffs");
            try
            {
                
                midiInputDevice = InputDevice.GetByName(MidiInputName);
                Debug.Log($"Loaded Input Device: {midiInputDevice.Name}");
                if(MidiOutputName != null && MidiOutputName != "")
                {
                    midiOutputDevice = OutputDevice.GetByName(MidiOutputName);
                    Debug.Log($"Loaded Output Device: {midiOutputDevice.Name}");

                }
                if (MidiOutputTwoName != null && MidiOutputTwoName != "")
                {
                    midiOutputDeviceTwo = OutputDevice.GetByName(MidiOutputTwoName);
                    Debug.Log($"Loaded Secondary Output Device: {midiOutputDeviceTwo.Name}");

                }
                midiInputDevice.EventReceived += OnEventReceived;
                midiInputDevice.StartEventsListening();
                Debug.Log($"MIDI is readty and listening!");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"Couldn't initialize MIDI Controller '{MidiInputName}': {e.Message}");
                return false;
            }
        }

        public bool deInitMidi()
        {
            try
            {
                if (midiInputDevice == null) { return true; }
                midiInputDevice.StopEventsListening();
                midiInputDevice.EventReceived -= OnEventReceived;
                midiInputDevice.Dispose();
                if (midiOutputDevice != null) midiOutputDevice.Dispose();
                if (midiOutputDeviceTwo != null) midiOutputDeviceTwo.Dispose();
                Debug.Log($"Disposed of MIDI Controller");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"Couldn't destroy MIDI Controller '{MidiInputName}': {e.Message}");
                return false;
            }
        }

        private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            Debug.Log("Event Recieved!");
            if(midiOutputDevice != null) midiOutputDevice.SendEvent(e.Event);
            if (midiOutputDeviceTwo != null) midiOutputDeviceTwo.SendEvent(e.Event);

            var midiDevice = (MidiDevice)sender;
            var Event = e.Event;

            if (Event is NoteOnEvent noteOnEvent)
            {
                
                try
                {
                    if (noteOnEvent.Velocity == 0)
                    {
                        Debug.Log($"A Note was released: {noteOnEvent.NoteNumber} , {noteOnEvent.Velocity}, CH {noteOnEvent.Channel}");
                        noteStates.Remove(noteOnEvent.NoteNumber);
                        if (noteListeners.ContainsKey(noteOnEvent.NoteNumber))
                        {
                            mainThread.Enqueue(() => {
                                foreach (IMidiNoteListener listener in noteListeners[noteOnEvent.NoteNumber])
                                {
                                    listener.SetNoteOff(noteOnEvent.NoteNumber, noteOnEvent.Channel, (int)noteOnEvent.Velocity); 
                                }
                            });
                        }
                    } else
                    {
                        Debug.Log($"A Note was pressed: {noteOnEvent.NoteNumber} , {noteOnEvent.Velocity}, CH {noteOnEvent.Channel}");
                        noteStates.Add(noteOnEvent.NoteNumber, noteOnEvent.Velocity);
                        if (noteListeners.ContainsKey(noteOnEvent.NoteNumber))
                        {
                            mainThread.Enqueue(() =>
                            {
                                foreach (IMidiNoteListener listener in noteListeners[noteOnEvent.NoteNumber])
                                {
                                    listener.SetNoteOn(noteOnEvent.NoteNumber, noteOnEvent.Channel, (int)noteOnEvent.Velocity);
                                }
                            });
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.Log($"Couldn't handle note press: {ex.Message}");
                }
                
            }
            if (Event is NoteOffEvent noteOffEvent)
            {
                Debug.Log($"A Note was released: {noteOffEvent.NoteNumber} , {noteOffEvent.Velocity}, CH {noteOffEvent.Channel}");
                noteStates.Remove(noteOffEvent.NoteNumber);
                if (noteListeners.ContainsKey(noteOffEvent.NoteNumber))
                {
                    mainThread.Enqueue(() =>
                    {
                        foreach (IMidiNoteListener listener in noteListeners[noteOffEvent.NoteNumber])
                        {
                            listener.SetNoteOff(noteOffEvent.NoteNumber, noteOffEvent.Channel, noteOffEvent.Velocity);
                        }
                    });
                }
            }
            if (Event is ControlChangeEvent controlChangeEvent)
            {
                Debug.Log($"A Control was changed: {controlChangeEvent.ControlNumber} , {controlChangeEvent.ControlValue}, CH {controlChangeEvent.Channel}");
                controlStates[controlChangeEvent.ControlNumber] = controlChangeEvent.ControlValue;
                if (controlListeners.ContainsKey(controlChangeEvent.ControlNumber))
                {
                    Debug.Log("found control listeners registered for this ID");
                    mainThread.Enqueue(() =>
                    {
                        foreach (IMidiControlListener listener in controlListeners[controlChangeEvent.ControlNumber])
                        {
                            listener.SetControlValue(controlChangeEvent.ControlNumber, controlChangeEvent.Channel, (int)controlChangeEvent.ControlValue);
                        }
                    });
                } else
                {
                    Debug.Log($"no control listeners registered for this ID ({controlChangeEvent.ControlNumber})");
                }
            }
        }

        public int GetNoteValue(int noteNumber)
        {
            int returnValue = 0;
            noteStates.TryGetValue(noteNumber, out returnValue);
            return returnValue;
        }

        public int GetControlValue(int noteNumber)
        {
            int returnValue = 0;
            noteStates.TryGetValue(noteNumber, out returnValue);
            return returnValue;
        }

        public void registerNoteListener(int noteNumber, IMidiNoteListener listener)
        {
            Debug.Log($"Registering Note Listener for {noteNumber}"); 
            if (!noteListeners.ContainsKey(noteNumber))
            {
                noteListeners[noteNumber] = new List<IMidiNoteListener>();
            }
            if(!noteListeners[noteNumber].Contains(listener))
            {
                noteListeners[noteNumber].Add(listener);
            }
        }

        public void registerControlListener(int controlNumber, IMidiControlListener listener)
        {
            Debug.Log($"Registering Control Listener for {controlNumber}");
            if (!controlListeners.ContainsKey(controlNumber))
            {
                Debug.Log($"Registering Control Listener group for {controlNumber}");
                controlListeners[controlNumber] = new List<IMidiControlListener>();
            }
            if (!controlListeners[controlNumber].Contains(listener))
            {
                Debug.Log($"adding Control Listener for {controlNumber}");
                controlListeners[controlNumber].Add(listener);
            }
        }

    }
}


