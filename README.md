# Jayo's MIDI Controller Animator

A collection of scripts, components, and utilities to standardize and simplify linking MIDI controller inputs to Unity objects

## Table of Contents

1. [Purpose](#purpose)
2. [Available Components](#available-components)
	1. [MIDI Animator Core](#midi-animator-core)
	2. [Button Control](#button-control)
	3. [Pad Control](#pad-control)
	4. [Slider Control](#slider-control)
	5. [Turner Control](#turner-control)
	6. [Spinner Control](#spinner-control)
	7. [Jogwheel Control](#jogwheel-control)
	8. [High-Resolution Variants](#high-resolution-variants)
3. [Creating Custom Controls](#creating-custom-controls)
4. [Working Examples](#working-examples)
	1. [Basic Demo](#basic-demo)
	2. [Detailed Demo](#detailed-demo)
5. [Dependencies](#dependencies)
6. [Special Thanks](#special-thanks)


## Purpose

I put this set of components together to provide an infrastructure for those who want to do something like replicate thier DJ controller in Unity and have their inputs reflected in real time.

Included are several individual components the mimic the behaviour of the most common types of MIDI inputs, the core logic to drive them, and a light framework for creating custom components with unique behaviours to suit any need.

## Available Components

Here's a list of all of the components available in this library at time of writing:

### MIDI Animator Core

This is the central component that defines a distinct MIDI controller, and communicates control changes and notes to the individual control components.  

It accepts the name of a MIDI input to listen to, and optionally allows those signals to be forwarded to up to two MIDI outputs.  This lets you connect to your MIDI device directly and still make the signal available to other applications if you're using something like loopMIDI to provide loopback ports.

### Button Control

This represents a basic button on a MIDI controller and responds to Note-On and Note-Off messages by swapping the visibility of an "inactive state" object and an "active state" object

### Pad Control

This is very similar to the Button Control (and at the current time functions exactly the same).  Howerver in future versions the Pad control will support handling of specific Note-On velocity values and polyphonic aftertouch

### Slider Control

This control represents a typical fader/slider on a MIDI controller, and will move a "handle" object in a linear way within specified dimensions in response to a control change

### Turner Control

This control represents fixed-limit knobs on a MIDI Controller; controls that are operated by turning them, but have fixed start- and end-stops.  Works very similar to a slider, but rotates an object in response to a control change rather than moving it

### Spinner Control

This control represents free-spinning encoders on a MIDI controller; those that are operated by turning but have no fixed start- or end-stop and can spin forever in either direction.  Encoders send Control Change messages, but the values describe the direction and velocity of the control rather than an absolute value

### Jogwheel Control

This control is the most complex example of the built-in control classes. It effectively combines two encoders (one for jogwheel rotation when it is being pressed, another when it is not), and a button (to read when the jogwheel is pressed or released)

### High-Resolution Variants

Also included are high-resolution variants of the Slider Control and Turner Control to properly work with elements that emit 14-bit high-resolution values 

## Creating Custom Controls

To create a custom control that works seamlessly with this library, you'll just need to create a MonoBehavior that implementsa the `IMidiControlListener` and/or `IMidiNoteListener` interfaces which mandate the methods that are required for the control to work.

In the `Start()` method of your control, you should access a MidiAnimatorCore instance and call `registerControlListener` or `registerNoteListener` so that the core will fire the right methods on the control at the right time, but in theory you could have your own process that doesn't include the MidiAnimatorCore and calls the SetNoteOn/SetNoteOff/SetControlValue methods on the control components directly.

## Working Examples

Included with the release package will be two functional examples that illustrate how the library can be used to get you up and running quickly

### Basic Demo

The Basic demo includes a MidiAnimatorController connected to examples of each type of built-in control component.  You can set the name of the MIDI input in the controller, and the note/control values on each of the controls to see them respond to your actions on your MIDI device.

### Detailed Demo

The detailed demo is a more involved recreation of a Hercules DJ Control Air controller (the one I have handy) with all of the inputs on this controller mapped to the expected note and control numbers to work as a real-time replica of the actual controller.  It can serve as an example for someone looking to replicate thier own controller

## Dependencies

This library uses [DryWetMidi](https://github.com/melanchall/drywetmidi) to interface with MIDI Devices, and so that library will need to be included in your project (or in the "Managed" folder in an existing app like VNyan, for example) in order to function correctly.  The release package will include an acceptable version of this library for x64 systems

## Special Thanks

2.0 for her efforts in testing/feedback and an almost magical ability to find edge- and corner-cases that made this library as flexible as it is!

Melanchall for developing and maintaining DryWetMidi, which makes tools like this possible!