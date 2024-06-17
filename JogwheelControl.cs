using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class JogwheelControl : MonoBehaviour, IMidiControlListener, IMidiNoteListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;
        [Tooltip("The GameObject representing the jogwheel in the \"normal\" state")]
        public GameObject SpinnerObject;
        [Tooltip("The GameObject representing the button in the \"pressed\" state")]
        public GameObject AltSpinnerObject;
        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;
        [Tooltip("The Control Number for which this virtual control should respond to Control Changes.  emitted when turning the jogwheel in normal state")]
        public int ControlNumber;
        [Tooltip("The Alternate Control Number for which this virtual control should respond to Control Changes. emitted when turning the jogwheel in pressed state")]
        public int AltControlNumber;
        [Tooltip("The Control Number for which this virtual control should respond to Control Changes.  emitted when pressing or releasing the jogwheel")]
        public int ActiveNoteNumber;
        [Tooltip("The total number of control change messages sent in one full revolution")]
        public float TicksPerRevolution;
        [Tooltip("Enable this to turn the virtual control in the opposite direction")]
        public bool reverseDirection;

        private int minimumValue;
        private int maximumValue;
        private float degreesPerTick;
        private float midpointValue;

        public void Start()
        {
            minimumValue = 1;
            maximumValue = 127;
            degreesPerTick = 360f / TicksPerRevolution;
            midpointValue = Mathf.Lerp(minimumValue, maximumValue, 0.5f);
            midiAnimatorCore.registerNoteListener(ActiveNoteNumber, this);
            midiAnimatorCore.registerControlListener(ControlNumber, this);
            midiAnimatorCore.registerControlListener(AltControlNumber, this);
            SetNoteOff(ActiveNoteNumber, MidiChannel, 0);
            AltSpinnerObject.SetActive(false);
        }

        public void SetControlValue(int controlNumber, int channel, int newValue)
        {

            if (channel != MidiChannel) return;
            Debug.Log($"JogwheelControl detected value of { newValue}");
            float spinFactor = 1.0f;
            float totalTicks = (maximumValue - newValue) + 1;
            if(newValue <= midpointValue)
            {
                spinFactor = -1.0f;
                totalTicks = (newValue - minimumValue) + 1;
            }
            if(reverseDirection)
            {
                spinFactor = spinFactor * -1;
            }
            Debug.Log($"total ticks: {totalTicks}");
            float degreesToRotate = totalTicks * degreesPerTick * spinFactor;

            Quaternion currentRotation = SpinnerObject.transform.localRotation;
            Quaternion currentAltRotation = AltSpinnerObject.transform.localRotation;

            float newRotation = currentRotation.eulerAngles.y + degreesToRotate;
            if(newRotation > 360f)
            {
                newRotation = newRotation - 360f;
            }
            if (newRotation < -360f)
            {
                newRotation = newRotation + 360f;
            }
            Debug.Log($"Adjusting rotation by {degreesToRotate} degrees");
            SpinnerObject.transform.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotation, currentRotation.eulerAngles.z);
            AltSpinnerObject.transform.localRotation = Quaternion.Euler(currentAltRotation.eulerAngles.x, newRotation, currentAltRotation.eulerAngles.z);

        }

        public void SetNoteOn(int noteNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;
            SpinnerObject.SetActive(false);
            AltSpinnerObject.SetActive(true);
        }

        public void SetNoteOff(int noteNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;
            SpinnerObject.SetActive(true);
            AltSpinnerObject.SetActive(false);
        }
    }
}
