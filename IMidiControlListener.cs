using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public interface IMidiControlListener
    {
        void SetControlValue(int controlNumber, int channel, int newValue);
    }
}
