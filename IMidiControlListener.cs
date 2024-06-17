using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public interface IMidiControlListener
    {
        void Start();
        void SetControlValue(int controlNumber, int channel, int newValue);
    }
}
