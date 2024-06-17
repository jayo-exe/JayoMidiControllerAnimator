using System;
using UnityEngine;

namespace JayoMidiControllerAnimator
{
    public interface IMidiNoteListener
    {
        void SetNoteOn(int noteNumber, int channel, int newValue);
        void SetNoteOff(int noteNumber, int channel, int newValue);
    }
}
