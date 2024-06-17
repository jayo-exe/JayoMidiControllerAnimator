using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class PadControl : MonoBehaviour, IMidiNoteListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;
        [Tooltip("The GameObject representing the pad in the \"inactive\" state")]
        public GameObject PadObject;
        [Tooltip("The GameObject representing the pad in the \"active\" state")]
        public GameObject AltPadObject;
        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;
        [Tooltip("The Note Number for which this virtual control should respond to Control Changes")]
        public int NoteNumber;

        private int CurrentValue;

        public void Start()
        {
            midiAnimatorCore.registerNoteListener(NoteNumber, this);
            SetNoteOff(NoteNumber, MidiChannel, 0);
            AltPadObject.SetActive(false);
        }

        public void SetNoteOn(int noteNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;
            if (newValue == CurrentValue) return;
            CurrentValue = newValue;

            PadObject.SetActive(false);
            AltPadObject.SetActive(true);
        }

        public void SetNoteOff(int noteNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;
            if (newValue == CurrentValue) return;
            CurrentValue = newValue;

            PadObject.SetActive(true);
            AltPadObject.SetActive(false);
        }
    }
}