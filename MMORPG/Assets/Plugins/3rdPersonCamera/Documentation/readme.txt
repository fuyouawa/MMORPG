----------------------------------------------
            3rd Person Camera
 Copyright © 2015-2019 Thomas Enzenebner
            Version 1.0.5.3
         t.enzenebner@gmail.com
----------------------------------------------

Thank you for buying the 3rd Person Camera Asset!

If you have any questions, suggestions, comments or feature requests, please send
an email to: t.enzenebner@gmail.com

Visit the forum: https://forum.unity.com/threads/3rd-person-camera-emulating-the-world-of-warcraft-camera-with-smart-pivot-and-thickness-checks.383754/#post-4356535
Beta access: http://enzenebner.com/beta

------------------------------
	Upgrade from 1.0.5.3
------------------------------

PlayerLayer was a single integer before and has changed to a LayerMask. You need to set the player layer again when importing 1.0.5.3!
Changes to inputs of Freeform will be overwritten but can be set in the component now

------------------------------
	Upgrade from 1.0.3
------------------------------

I've added a namespace "ThirdPersonCamera" to all scripts, so if you happen to reference it somewhere in your projects just add the namespace at the top of the script:

using ThirdPersonCamera;

------------------------------
	Upgrade from <1.0.2
------------------------------

CameraController was split into CameraController and FreeForm. If you want the same functionality add the FreeForm component to the camera.


------------------------------
 How To use 3rd Person Camera
------------------------------

There are several prebuilt camera gameobjects in the "Prefabs" folder to start from. These are:
- 3rdPersonCamera_Basic: Most basic setup to enable clipping logic
- 3rdPersonCamera_FreeForm: 360 Orbit Camera with distance.
- 3rdPersonCamera_FreeFormAndTargeting: Orbit camera with lookAt/targeting support
- 3rdPersonCamera_Follow: smoothly aligns itself to the transform.forward of the target
- 3rdPersonCamera_Ultimate: FreeForm, Follow in one, controlled with DisableFollow
- 3rdPersonCamera_OverTheShoulder: FreeForm and over the shoulder camera from games like The Division

There's also 5 demoscenes to see how it's done. (Freeform demo, Follow demo, Freeform + Target demo, Ball demo and OverTheShoulder demo)

There are 5 important components:
- CameraController
- Freeform
- Follow
- DisableFollow
- LockOnTarget/Targetable
- OverTheShoulder

Camera Controller component:
The main part of the asset. Handles occlusion, smart pivoting and thickness checks.

There are a number of inputs and options you can tweak.
The most important is, which Transform to follow. Set this in "Target".

Options:
	- Target (Transform): Set this to the transform the camera should follow, can be changed at runtime
	- Offset vector (Vector3): change this vector to offset the pivot the camera is rotating around	
	- Camera Offset vector (Vector3): offset the camera in any axis without changing the pivot point
	- Smart Pivot (bool): Uses a pivot when the camera hits the ground and prevents moving the camera closer to the target
						  when looking up.
	- Occlusion Check (bool): Automatically reposition the camera when the character is blocked by an obstacle.
	- Thickness Check (bool): Thickness checking can be configured to ignore  smaller obstacles like sticks, 
							  trees, etc... to not reposition or zoom in the camera.
	
	- Desired Distance (float): The distance how far away the camera should be
	- Collision Distance (float): Offset for the camera to not clip in objects		
	- Max Thickness (float): Adjusts the thickness check. The higher, the thicker the objects can be and there will be no
							 occlusion check. Warning: Very high values could make Occlusion checking obsolete and as a result
							 the followed target can be occluded
	- Max Thickness Iterations (int): The number of iterations with that the thickness is calculated. The higher, the more performance 
								it will take. Consider this as the number of objects the thickness ray will go through.
	- Zoom Out Step Value (float): The increment steps how fast the player can zoom out
	- Zoom Out Step Value per Frame (float): The speed how fast the camera can zoom out automatically to the desired distance
	- Smooth Target Mode (bool): Enable to smooth out target following, to hide noisy position changes that should not be picked up by the camera immediately
								 or to enable smooth transitions to other targets that are changed during runtime
	- Smoothing Speed (float): The speed at which the camera lerps to the actual target position
	- Collision Layer (LayerMask): Set this layermask to specify with which layers the camera should collide
	- Player Layer (LayerMask): Set this to your player layer so ground checks will ignore the player collider
	
Public variables:
	- bool playerCollision (get/set): Set to true to deactivate the player model    
    - float Distance (get): The current distance from the target plus offsetvector to the camera
	

Freeform compontent:
	This script handles the main rotation mechanic of the camera and is used for Freeform camera movement. It's not needed if you just
	want follow mode! It's dependency is the CameraController. It can be extended with the LockOnTarget component or OverTheShoulder component.
	
	- Camera Enabled (bool): Enables/Disables the camera rotation updates. Useful when you want to lock the camera in place or
							 to turn off the camera, for example, when hovering an interface element with the mouse
	- Camera Mode (enum): Always and hold - Either the camera rotation is always on or you have to press the mouse button to look around
	- Mouse Input: a list of integer mouseButton values to enable freelook, default is left/right mouse button
	- Keyboard Input: a list of KeyCodes to enable freelook
	- Controller Enabled (bool): Enables controller support
	- Controller Invert Y (bool): Inverts the Y-axis
	- Mouse Invert Y (bool): Inverts the Y-Axis
	- Lock Mouse Cursor (bool): When looking around the mouse cursor will be locked	
	- Mouse Sensitivity (Vector2): Adjusts the sensitivity of looking around with the mouse
	- Controller Sensitivity (Vector2): Adjusts the sensitivity of looking around with the controller	
	
Follow component:
	This script handles following the target without any manual camera input. Useful for games that handle non-humanoid targets 
	like racing or flying games. Its dependency is the CameraController.
	
	- Follow (bool): Enables/Disables the follow mode
	- Align on Slopes (bool): Enables/Disables automatic alignment of the camera when the target is moving on upward or downward slopes
	- Rotation Speed (float): How fast the camera should align to the transform.forward of the target
	- Look Backwards (bool): Enables/Disables looking backwards
	- Check Motion for Backwards (bool): Enables/Disables automatic checking when the camera should look back
	- Backwards Motion Threshold (float): The minimum magnitude of the motion vector when the camera should consider looking back
	- Angle Threshold (float): The minimum angle when the camera should consider looking back
	- Tilt Vector (Vector3): Applies an additional vector to tilt the camera in place. Useful when the offset vector gets too big 
							and leaves the collision box of the model
	- Layer Mask (LayerMask): The layer which should be used for the ground/slope checks. Usually just the Default layer.
	
LockOnTarget component:
	This script handles locking onto targets. It's dependencies are the CameraController and Freeform component.
	
	- Follow Target (Targetable): When not null, the camera will align itself to focus the Follow Target.
	- Rotation Speed (float): How fast the camera should align to the Follow Target

Targetable component:
	Every Target that's focusable needs a Targetable component.
	
	- Offset (Vector3): Give the target an offset when the transform.position is not fitting.

DisableFollow component:
	Disables follow component at adjustable triggers to allow FreeForm. Its triggers are target motion, time and mouse input.

	- activateMotionCheck (bool): Enables motion check with its motionThreshold
    - activateTimeCheck  (bool): Enables timed checks and resets to follow after timeToActivate seconds are elapsed
    - activateMouseCheck (bool): Enables reactivation of follow when mouse buttons are released

    - timeToActivate (float): Time in seconds when follow is reactivated. It will also not reactivate when the user still has input.
    - motionThreshold (float): Distance which has to be crossed to reactivate follow

OverTheShoulder component:
	With this script the camera can change into an aim and release mode by pressing the right mouse button.
	It is controlled with these values:		
		- Max Value (float): How much the camera moves away from its zero position. 0.5 means it'll set the max camera offset vector to (axis * maxValue), so -0.5f to 0.5f
		- Aim Speed (float): How fast it lerps to the max position when starting to aim
		- Release Speed (float): How fast it lerps to the zero position when releasing
		- Left (bool): When activated the camera will slide to the left
		- Base Offset (Vector3): The base offset serves as starting and endpoint when releasing.
		- Slide Axis (Vector3): You can tweak the axis on which the camera slides on, usually it will be just operating on the x axis to slide 
								left and right from the targeted character but it can be changed to any direction in case gravity changes for example.
								The intended design is to use normalized values between -1 and 1
								The difference to the "Additional Axis Movement" vector is that the slide axis goes back and forth when aiming/releasing
								were the additional axis is fixed.
		- Additional Axis Movement (Vector3): This axis can be used to have additional offsets when aiming. Unlike the slide axis this
											  axis is intended for non-normalized values much like the base offset. 
											  It can be used to make the camera zoom high above the character for example when aiming.
		- CC (CameraController): reference to CameraController, this is set automatically when OverTheShoulder is on the same GameObject as CameraController
	
	
Additional setup for controller support:
For an easier setup use the preconfigured InputManager.asset from the 3rdPersonCamera/ProjectSettings folder. 
!Caution! - Doing so will overwrite any InputManager data you already have!

If you get this warning: "Controller Error - Right axis not set in InputManager. Controller is disabled!"
you have to set the following axis in the InputManager:
"Right_3": 3rd axis
"Right_4": 4th axis
"Right_5": 5th axis 
0 gravity, 0.3 dead, 1 sensitivity are good standard values.

If you need other names you can change them in the script.

------------------------------
	Controls for demos
------------------------------
- WASD for movement
- Left/right click to rotate in freeform demo
- Right click, lock on target in target demo
- "r" key to reset car/ball
- "q" and "e" to rotate camera in ball demo
	
-----------------
 Version History
-----------------
1.0.5.4
	- added forceCharacterDirection to FreeForm so the character turns automatically in the same direction the camera is facing.
	  This works only for character controllers that use absolute directions for movement. The demo character controller
	  uses relative movement, so it won't work right.
	- fixed a sorting bug with occlusion hits
	- improved thickness logic for clipping geometry that occurs often in prefab based level design
1.0.5.3
	- improved jitter when camera was zooming out against a wall
	- improved thickness handling
1.0.5.2
	- added min/max distance to CameraController
	- added namespace for demoscene scripts
1.0.5
	- added an additional camera offset vector for skewed over-the-shoulder camera type of games like "The Division"
	  that can either be fixed or controlled via the OverTheShoulder script
	- added OverTheShoulder component
	- added new scene to demonstrate Camera Offset called "DivisionCamera - DemoScene"
	- changed PlayerLayer data type from integer to LayerMask for convenience and the possibility to have more player layers
	- added "Smooth Target Mode" and "Smooth Target Value" that lerps to the target position for smoother translations/rotations
	  to dampen noisy position changes for either physics or network based games	
	- added support for having no target when initializing or starting
	- added mouse/keyboard inputs to FreeForm which uses a list of mouse buttons (Integer) and keyboard inputs (KeyCode)
1.0.4
	- added smart DisableFollow script to utilize FreeForm + Follow
	- added Ball demo scene
	- added simple BallController script
	- added namespace "ThirdPersonCamera" to scripts
	- added more camera prefabs (Basic, Follow, FreeForm and Ultimate (Freeform/Follow/DisableFollow))
	- improved smart pivot transition from sloped surfaces
	- improved smoothness of alignToSlopes feature
	- improved camera when offset clips into geometry
	- restored Ethan crouch animation
	- removed Ethan air->ground crouch animation
1.0.3
	- added 2 new demo scenes (follow and follow+lock on)
	- added support for follow mode
	- added support for target locking
	- split CameraController into CameraController and Freeform	
	- improved smart pivot and occurances of snapping
	- improved thickness check	
	- changed hardcoded raycast layermasks to be configured in the editor
1.0.2	
	- changed collision sensitivity to be spherical
	- removed camera position/rotation initialization, editor values are now taken
	- improved smart pivoting and smart pivoting start and resets
	- improved detection algorithm when multiple raycasts are hit
	- added terrain to demoscene
	- added RotateTo public method
	- added x/y, playerCollision, Distance public get or/and set
1.0.1
	- improved thickness checking
	- improved smart pivoting on slopes
	- added collision distance to improve clipping occurrences	
	- added automatic mouse cursor locking when looking around (currently unstable in WebGL)
	- added interface handling to demo scene	
	- changed sensitivity handling
	- removed unnecessary files
	- added a script only package
1.0.0
	- initial release
