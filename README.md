# 12 Degree of Freedom Quadcopter Simulation
This system is written in C# and uses Helix3D to visualize the kinematics of the system.

The control structure heavily relies on the usage of the ADRC (Active Disturbance Rejection Control) algorithm to control each individual thruster to position and rotate the quadcopter in 3d space, while keeping it stable.

To run the simulation, the Visual Studio Solution File (.sln) can be opened, compiled, and ran. Or the binaries (.exe) located in the build bin folder can be ran as well.
