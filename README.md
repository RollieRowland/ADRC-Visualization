# Dual-Tiltrotor Quadcopter Control Structure
This project is the control structure for a dual-tiltrotor quadcopter with a testing interface in C# and C++ with physics simulation, and a hardware implementation written in C++.

## Simulation
Written in C# with C++/CLI/CLR interface with C++ control structure library.

![alt text](/videoScreenshot.PNG)

## Hardware Implementation
Written in C++ using C++ control structure code in secondary project for running on BCM2835 Raspberry PI with PCA9685 16 channel PWM controller, a TCA9548A multiplexer, six MPU6050s, and one MPU9150.


## Abstract
The primary deliverable of this project - the implementation of a control structure that efficiently manages a custom dual-tilt rotor quadcopter - relies on the fusion of multiple experimental and complex concepts from the fields of control theory, physics, robotics, and mathematics.

The first phase of this project revolved around testing each of the individual components that will, in the end, be used together to complete the implementation of the control structure. One unknown factor was the implementation of the ADRC feedback control algorithm. The algorithm was fully implemented in code from information given in research paper, therefore, it’s success was unknown requiring thorough testing. This testing was completed through the implementation of a simulation of a simple mechanical system - an inverted pendulum - that is easily controlled by a standard PID controller. But to fully test the versatility of this algorithm, the mechanical aspects of the pendulum had to change - such as the mass at the end of the pendulum arm. Depending on the implementation, the inversion of the arm should be as responsive with one mass as it is with another. This testing proved successful, verifying that the ADRC was implemented properly.

The following phase relied on building a physics and mathematics library specific to this application from prior knowledge and segments of formulas in research papers. One main development that came from this was the quaternion class - representing three-dimensional rotation using complex numbers in four dimensional space - that allows conversion from any other existing form of rotation as well as a custom formalization of rotation and with the capability of algebraic operations. Although even having full capabilities of visualizing three-dimensional rotation, it is impossible to directly correlate any of the quaternions 4 parameters in three-dimensional space. Which is where the custom formalization of rotation comes in, the direction-angle, which allows for direct use in kinematics calculations.

With the description of the stepping stones of the project explained, the culmination of these concepts allow the development of the primary deliverable of this project, the control structure for the dual-tilt rotor quadcopter; a quadcopter where each of the rotors can be rotated in respect to the frame in two degrees of freedom. This control structure is built on quaternion algebra, the ADRC feedback controller, and much more. With this functionality as a foundation, this control structure will be able to rotate the quadcopter in any orientation from any orientation interpolating smoothly, and position the quadcopter in any point in Euclidean space while in any orientation, while automatically correcting for changes in the environment.

## Running
To run the simulation, open the Visual Studio Solution File (.sln), then compile the DTRQUserInterface, and run.

To run the hardware implementation, open the Visual Studio Solution File (.sln), configure a remote build platform, select the remote build platform, and then build. After being built, execute the file on the external system.

To run the implemented test cases, open the test manager, and run all.
