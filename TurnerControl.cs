using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class TurnerControl : MonoBehaviour, IMidiControlListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;

        [Tooltip("The GameObject representing a knob that will rotate in response to Control Changes")]
        public GameObject TurnerObject;

        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;

        [Tooltip("The Control Number for which this virtual control should respond to Control Changes")]
        public int ControlNumber;

        [Tooltip("The rotation of the control at the lowest value")]
        public float MinimumRotation;

        [Tooltip("The rotation of the control at the highest value")]
        public float MaximumRotation;

        private int minimumValue;
        private int maximumValue;
        private int currentValue;

        public void Start()
        {
            minimumValue = 0;
            maximumValue = 127;
            midiAnimatorCore.registerControlListener(ControlNumber, this);
            SetControlValue(ControlNumber, MidiChannel, minimumValue);
            Quaternion currentRotation = TurnerObject.transform.localRotation;
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, MinimumRotation, currentRotation.eulerAngles.z);
            TurnerObject.transform.localRotation = currentRotation;
        }

        public void SetControlValue(int controlNumber, int channel, int newValue)
        {

            if (channel != MidiChannel) return;
            if (newValue == currentValue) return;
            currentValue = newValue;

            float clampedValue = Mathf.Clamp(currentValue, minimumValue, maximumValue);
            float normalizedValue = (clampedValue - minimumValue) / (maximumValue - minimumValue);
            float newRotation = Mathf.Lerp(MinimumRotation, MaximumRotation, normalizedValue);

            Quaternion currentRotation = TurnerObject.transform.localRotation;
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotation, currentRotation.eulerAngles.z);
            TurnerObject.transform.localRotation = currentRotation;
        }
    }
}
