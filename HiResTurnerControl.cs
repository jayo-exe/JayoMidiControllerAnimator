using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class HiResTurnerControl : MonoBehaviour, IMidiControlListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;

        [Tooltip("The GameObject representing a knob that will rotate in response to Control Changes")]
        public GameObject TurnerObject;
        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;

        [Tooltip("The Low Control Number for which this virtual control should respond to Control Changes")]
        public int ControlNumberLow;
        [Tooltip("The High Control Number for which this virtual control should respond to Control Changes")]
        public int ControlNumberHigh;
        [Tooltip("The y-rotation of the control at the lowest value")]
        public float MinimumRotation;
        [Tooltip("The y-rotation of the control at the highest value")]
        public float MaximumRotation;

        private int minimumValue;
        private int maximumValue;
        private int currentLsb;
        private int currentMsb;
        private int currentValue;

        public void Start()
        {
            minimumValue = 0;
            maximumValue = 16383;
            midiAnimatorCore.registerControlListener(ControlNumberLow, this);
            midiAnimatorCore.registerControlListener(ControlNumberHigh, this);
            SetControlValue(ControlNumberLow, MidiChannel, minimumValue);
            SetControlValue(ControlNumberHigh, MidiChannel, minimumValue);

            Quaternion currentRotation = TurnerObject.transform.localRotation;
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, MinimumRotation, currentRotation.eulerAngles.z);
            TurnerObject.transform.localRotation = currentRotation;
        }

        public void SetControlValue(int controlNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;
            if (controlNumber == ControlNumberHigh)
            {
                Debug.Log($"Setting high value to {newValue}");
                currentLsb = newValue;
            }
            else if (controlNumber == ControlNumberLow)
            {
                Debug.Log($"Setting low value to {newValue}");
                currentMsb = newValue;
            }

            currentValue = (currentMsb << 7) | currentLsb;
            Debug.Log($"Current MSB is {currentMsb}");
            Debug.Log($"Current LSB is {currentLsb}");
            Debug.Log($"Current 14-bit value is {currentValue}");

            float clampedValue = Mathf.Clamp(currentValue, minimumValue, maximumValue);
            float normalizedValue = (clampedValue - minimumValue) / (maximumValue - minimumValue);
            float newRotation = Mathf.Lerp(MinimumRotation, MaximumRotation, normalizedValue);

            Quaternion currentRotation = TurnerObject.transform.localRotation;
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotation, currentRotation.eulerAngles.z);
            TurnerObject.transform.localRotation = currentRotation;
        }
    }
}
