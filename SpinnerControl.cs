using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public class SpinnerControl : MonoBehaviour, IMidiControlListener
    {
        [Tooltip("The Midi Animator Core that will drive this Control")]
        public MidiAnimatorCore midiAnimatorCore;
        [Tooltip("The GameObject representing a knob that will rotate in response to Control Changes")]
        public GameObject SpinnerObject;
        [Tooltip("The MIDI Channel from which targer messages must originate")]
        public int MidiChannel = 0;
        [Tooltip("The Control Number for which this virtual control should respond to Control Changes")]
        public int ControlNumber;
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
            midiAnimatorCore.registerControlListener(ControlNumber, this);
            degreesPerTick = 360f / TicksPerRevolution;
            midpointValue = Mathf.Lerp(minimumValue, maximumValue, 0.5f);
        }

        public void SetControlValue(int controlNumber, int channel, int newValue)
        {
            if (channel != MidiChannel) return;

            float spinFactor = 1.0f;
            float totalTicks = (maximumValue - newValue) + 1;
            if (newValue <= midpointValue)
            {
                spinFactor = -1.0f;
                totalTicks = (newValue - minimumValue) + 1;
            }
            if (reverseDirection)
            {
                spinFactor = spinFactor * -1;
            }
            Debug.Log($"total ticks: {totalTicks}");
            float degreesToRotate = totalTicks * degreesPerTick * spinFactor;

            Quaternion currentRotation = SpinnerObject.transform.localRotation;

            float newRotation = currentRotation.eulerAngles.y + degreesToRotate;
            if (newRotation > 360f)
            {
                newRotation = newRotation - 360f;
            }
            if (newRotation < -360f)
            {
                newRotation = newRotation + 360f;
            }
            Debug.Log($"Adjusting rotation by {degreesToRotate} degrees");
            SpinnerObject.transform.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotation, currentRotation.eulerAngles.z);

        }
    }
}
