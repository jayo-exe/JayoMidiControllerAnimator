using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class SliderControl : MonoBehaviour, IMidiControlListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;
        [Tooltip("The GameObject representing a handle that will move in response to Control Changes")]
        public GameObject SliderObject;
        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;
        [Tooltip("The Control Number for which this virtual control should respond to Control Changes")]
        public int ControlNumber;

        [Tooltip("The z-position of the control at the lowest value")]
        public float MinimumPositon;
        [Tooltip("The z-position of the control at the highest value")]
        public float MaximumPosition;

        private int minimumValue;
        private int maximumValue;
        private int currentValue;

        public void Start()
        {
            minimumValue = 0;
            maximumValue = 127;
            midiAnimatorCore.registerControlListener(ControlNumber, this);
            SetControlValue(ControlNumber, MidiChannel, minimumValue);
        }

        public void SetControlValue(int controlNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;

            if (newValue == currentValue) return;
            currentValue = newValue;

            float clampedValue = Mathf.Clamp(currentValue, minimumValue, maximumValue);
            float normalizedValue = (clampedValue - minimumValue) / (maximumValue - minimumValue);
            float newPosition = Mathf.Lerp(MinimumPositon, MaximumPosition, normalizedValue);

            Vector3 currentPosition = SliderObject.transform.localPosition;
            currentPosition.z = newPosition;
            SliderObject.transform.localPosition = currentPosition;
        }
    }
}





