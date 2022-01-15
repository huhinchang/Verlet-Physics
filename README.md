# About The Project
![simulation of rope being cut](Recordings/Rope.gif)

Verlet integration physics simulation implemented on the CPU and GPU
- CPU (cyan): C#
- GPU (magenta): Unity compute shaders (Based on DX11 HLSL)

# Installation
- Unzip `Release/Standalone.zip`
- Launch `Verlet Physics.exe`

# Usage
## General
- Anything BLUE is the CPU
- Anything PINK is the GPU
- The point that is white is the "selected" point. (see tools section for more info)

## Tools tab
Tool|Description|Notes
----|----|----
Place| - LMB to place node <br> - RMB to place locked node| - locked nodes don't move <br> - automatically links the new node to the "selected" node <br> - automatically selects the new node
Paint| - LMB to unlock nearest node to mouse <br> - RMB to lock nearest node to mouse | 
Erase| - LMB to erase nearest node to mouse |- Also deletes lines that were connected to the node
Link|  - LMB to select nearest node to mouse <br> - RMB to connect nearest node to the "selected" node
Knife| - LMB drag to cut all intersecting lines | - Does not affect nodes

## Settings tab
Setting|Description|Notes
----|----|----
Render CPU above| - Toggles render order
Show CPU| - Shows/hides CPU (blue)| - Hidden nodes are still affected by tools!
Show GPU| - Shows/hides GPU (pink)| - Hidden nodes are still affected by tools!
CPU iterations|  - How many constraint iterations the CPU (blue) should perform | - Higher = more stable but less performant
GPU iterations|  - How many constraint iterations the GPU (pink) should perform | - Higher = more stable but less performant
