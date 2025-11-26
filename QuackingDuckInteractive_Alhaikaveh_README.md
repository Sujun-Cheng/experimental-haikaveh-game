Game Instructions

1. Open the Scene
Open the scene file:
assets/scenes/MainMenu.unity


2. Start the Game
Basic Controls:

WASD/Left Stick– Move character

Mouse/Right Stick – Adjust camera view

Tab/ North Button– Switch between characters

Space/ South Button – Jump

Shift/Right Mouse Button/ East Button - Sprint

1-0 - Variable Speed

Left Mouse Button/West Button – Attack



Gameplay Settings:

Dual-character system: Control the main character; the secondary character follows using NavMesh.

Follows by walking at close range.

Runs to catch up when far away.
Alhaitham: Purely offensive Character. Deals large amount of damage.
Kaveh: Hybrid character. Drains the HP of enemies arounds him and uses it to heal allies. Deals smaller amounts of damage.

Enemy Interaction:

Interacting with enemies triggers a dialogue system.

Dialogue choices determine whether the encounter ends in peace or battle.

The player must deal with all enemies to complete the game objective.

Collectibles:

All collectible items must be gathered to complete the game.

Tip:

If the main character falls off a platform, quickly switch to the secondary character to avoid retracing progress.

Current Issues

Jump Key Not Responsive

Win Condition not triggerable

Sometimes the jump action (Space key) fails to trigger properly.

Death Handling Bug

When the main character dies, the game does not correctly process the death event.

There is no UI screen or feedback displayed upon death.

No End of Game

Player can fall off world


MANIFEST:
Sujun Cheng:
1. Player:
Set up the New Input System mappings, schemes, and all interactions to work with the new input system
In PlayerInput, multiple mapping schemes and action maps are set up to work with character locomotion, dialogue, and pause menu. 
Converted usages of the old input system to the new input system in order to have more centralized control of all interactions, modularize input mappings, and eliminate race conditions caused by two different input systems simultaneously being active. 
Player movement is mapped to a 2d vector indicating a direction relative to the player. The player is subsequently rotated in that direction and the magnitude of the vector is given as character velocity to the animator
Character animator and root motion system code (walking, running, sprinting, jumping, Kaveh’s attack)
Downloaded character animations for walking, running, sprinting, jump up, falling, landing for both playable characters, as well as the attack animation for character Kaveh.
Added sound for footsteps in animation
Set up animations in the animator controller. Implemented a 1d blend tree for walking/running, supporting animation blending and speed variability. 
Set up animator state machines for sprinting and jumping. Sprint animation plays when the Dash input (see new input system for how controls are mapped) is detected. Jump up animation is played when the player is grounded and the Jump input is detected. 
Jump transitions smoothly to the falling animation and will stay in the falling state until ground is detected, at which point it will transition to the landing animation. 
In the case of ground not being detected, character will transition directly into falling state without the jump animation
Applied horizontal and vertical jump force for character in the event that the player was walking/running/sprinting when jumping. The jump animation only supports direct up and down jumping within root motion and thus additional upward impulse is needed to make the character jump up. The same goes for horizontal jump forces: it makes little sense for the character to stop moving horizontally when jumping when it was in a moving state immediately before. The horizontal jump force scales with the speed that the character was moving, and is multiplied by a constant so that the jump horizontal velocity would be smaller than if the character stayed on the ground, as would be the case in real life due to trigonometry. 
The entire character is implemented as a rigidbody with the animator component attached to it. The rigidbody is the driven component so the animator component applies the forward movement while the rigidbody position is manually updated in the code. 
Cinemachine: 
Installed cinemachine from package manager
Set up a Free Look camera to follow the active player and switch to the other character when character switching is pressed 
Tweaked various settings so that the field of view would focus on the character model and showcase character movements in a meaningful way without limiting the field of view of the environment. 
Implemented an algorithm so that the player character’s move forward key would always cause the character to move in the direction of the camera, rather than the orientation they were facing before. This works by rotating the movement vector provided by the game controls by the difference in orientation between the camera and the player. This new vector is subsequently passed down to the character’s root motion controller script and the character would be rotated accordingly. 
Kaveh’s attack mechanic: 
Installed animation for attack, and implemented the mechanics of the attack to siphon the health of enemies within a certain radius and heal allies around him
Inputted vfx effects for both the attack and the heal. 
Character manager: 
Set up the characters at the beginning of the game.
Implemented parts of the code so that when switching between characters, the playable character would be in an AI following state and the former AI character would be in a player controlling state. 
Set up the input controls for character switching
Implemented a trigger so that a certain character would be added to the list of AI companions upon entering it. This is used within the domed building where the player first encounters character Alhaitham
Main character controller:
The centralized state machine for our character. It supports the AI Following state, AI Combat state, AI Idle state, and Player Controlled state.
Partially implemented the state switching APIs: disabling and enabling certain scripts based on state. 
2. AI Follower:
AI State machine: 
Introduced AI Following state, AI Combat state, AI Idle state, and Player Controlled state as enums
AI Following mechanic complete with root motion
AI agent would go towards player character at variable speeds: they will sprint when very far away, run when far away, walk when close, and stop within a certain radius. 
In order for the AI agent to be able to transition between playable and non-playable, the control script must be compatible with the existing framework for character movement. The character is implemented as a rigidbody with an animator and Navmesh agent attached to it. The navmesh agent rotation and movement is disabled.
In the grounded state, the animator is doing the work while driving the rigidbody component, as was in the player controlled state. I implemented an algorithm so that the nav mesh agent would supply movement vectors to the root motion script just as the player would do in their character control. The movement vector is calculated by taking the nav agent path’s steering target (guaranteed to be a straight line), normalizing it, and then multiplying it by the agent’s max speed. 
In the non-grounded state, the ai nav mesh agent is disabled and rigidbody is set to not kinematic, allowing gravity to do the work of moving the character before hitting the ground and re-enabling the ai navmesh agent. 
Added support for AI nav mesh links: In the case of the agent traversing the nav mesh agent link, we turn off the animator and turn on the AI nav agent update position. We then start a coroutine to manually update the transform and animator in a parabolic motion while looking at the end position of the nav mesh link so that it would look like the character is jumping towards that position. We then complete the navmesh link traversal and the animator is once again given control of the character.
3. Checkpoint System:
Implemented a prefab and a checkpoint system so that when the player passes a checkpoint, it would send an Checkpoint Event to the checkpoint system with itself as the parameter so that the system can keep track of the most recent checkpoint passed and teleport the player accordingly upon a death event. 
Implemented a Death Event that would be emitted when the player or an AI agent’s health bar hits 0, or certain colliders have been hit. This would subsequently move the transform of the character to the most recent waypoint. 
The waypoint has healing properties: this is to prevent the player from being stuck in a dead state when the health bar hits 0 after teleportation and subsequently being unable to move. 
Implemented out of bounds collider script so that it would trigger a death event when it is hit. This will teleport the player to the nearest checkpoint. 
Implements the revive function in the health system so the player would no longer be dead
4. In game mechanical elements:
Implemented a collapsible bridge such that it would be normal before being stepped on, but the floor boards will collapse when something collides with it. 
Added sound to floorboards in bridge so that there would be audio feedback when there are collisions
Added a triggerable trap such that when a player collides with it, all game objects linked into the trigger will be enabled. 
Added a script such that upon triggering a button, it would disable the objects linked to it with a certain amount of timed lag between each object. This is also used as a puzzle mechanic in the domed building.
Water plane: The water has a collider that triggers a death event when the player comes into contact with it. 
Hologram shader: Created a shader so that it would achieve a hologram-like effect with a customizable color, transparency, and intensity
5. In game map and scene design: 
Placement of in game elements: expanded the NPC spawners, enemy spawners to cover the entire abandoned town area. Added collectible objects to the maze-like component of the town to expand the playable area of the game. Moved certain NPCs to later parts of the game for better pacing.
Waterfall particle system: there are 2 in the game, first in the starting scene and the second one obscuring the secret room that Kaveh falls into when the bridge collapses. 
The terrain: Painted all mountains, islands, static terrain, trees, bushes, and ground texture in the game. 
The water: Added a plane that serves as an out of bounds area. This is to limit the mobility of the player and prevent them from exploring the surrounding mountain area, which could be mistaken for part of the game scene when they are in fact intended to prevent the player from climbing over and seeing the ends of the map. This also serves as a puzzle mechanic, as players should aim to prevent touching the water. Also added a sound source to the water so the player can hear the sound of water flowing. 
Tunnel: Serves as a transition between the natural world of only mountains and rivers and the ruins of the island. Making the player traverse through it gives them time to get used to the controls and also creates enough time to give the player some amount of exposition through internal narration.
All static elements in the secret room under river
Fences around the town to prevent the character from entering it before encountering Alhaitham.
The domed building where the character Alhaitham was found: 
The dome itself
Added a balcony-like area immediately after the star bridge so that the player would feel compelled to land there and head into the domed building instead of trying to explore the rest of the island. This is because the story follows a somewhat linear progression and the character switching mechanic needs to be introduced as soon as possible to allow players access to the full mechanics of the game. 
The staircase leading into the domed building: This is a failsafe in case the players jumped off the balcony and cannot get back into the building or otherwise got themselves stuck outside. There is a steep drop off once inside so that players cannot attempt to head back outside once inside the building. 
The water inside: prevents players from getting stuck if they fall off the platform by teleporting them back onto the platform since the only entrances and exits are in the middle of the building. 
The platforms where Alhaitham gets discovered. This is intended to lead the player directly to the town area of the map. 
Triggerable traps for the player, which forces the player to utilize the character switching mechanic and forces the progression of the story in one direction.
Environment shaders: altered the ambient lighting such that areas without sky light such as tunnels would be visible by introducing ground light. Changed the directional light source such that the light emitted would be a cooler color tone, as is natural at night.
6. Objective system:
Added an generic Objective class, which will keep the current objective active and disable all other ones in its list.
Added an objective quest system that starts the quest when the game loads and progresses the quest to the next objective when it detects that an objective is completed.
Once all objectives on the list are completed, the quest system will trigger the win condition.
This is implemented using the Objective Complete signal, which the Objective object will emit once the conditions for its completion are fulfilled.
Added a Target Reached Objective subclass, which will emit the event when the player enters its trigger area. 
Edited the Interact objective to inherit the Objective class so it can be used within the framework of the quest system. 
Also set up and placed all objectives in the quest system except for the Interact Object, which was partially modified by me but not completely written. 
Wrote all the objectives in the quest system except for the Interact Object
7. Objective Flavor text system: used for exposition and communicating the plot and dialogue between two player characters
Implemented a triggerable flavor text prefab that comes with a trigger collider and a configurable list of lines.
Upon collision, a coroutine will trigger and all lines configured within the prefab will be rendered on screen with a letter typeout effect one by one, in the color specified by the programmer. 
Entering a new trigger will stop all coroutines and override the previous flavor text object with the contents of the new object, preventing race conditions from occurring and ensuring that the latest trigger entered is the most recent text displayed on screen. 
This flavor text system can be used in conjunction with the objectives in the quest system, since the plot follows a linear progression. This allows certain information and dialogue to only be communicated once players have progressed to certain points in the game, and old information that has no use to the player will no longer be available.
All writing (exposition, internal narration, dialogue) using the objective flavor text system is done by me, as well as the placement of these triggerable dialogues.
8. Objective Marker system: used to indicate the target of the current objective. 
Uses a world space to screen function to map the transform of the objective, then renders a sprite onto a panel at that location. The value is clamped so that the objective marker sprite would not go off the screen. Also added a check to prevent the marker from rendering if the target is behind the camera. 
9. Music:
Composed ambient background music
Composed battle music. 
Added In Battle event so that the game would switch the music to battle music when NPCs become hostile. 
Implemented switch music algorithm, where the game would revert the music back to peaceful music if no In Battle events are detected within 5 seconds. 
10. Non Game Development contributions:
The editing for the game trailer
Camerawork within the game scene for cinematic shots.
Background music used for trailer
The gameplay video

Scripts: 
Assets/Scripts/CharacterControl/CharacterManager.cs
Assets/Scripts/CharacterControl/CharacterInputController.cs
Assets/Scripts/CharacterControl/RootMotionControlScript.cs
Assets/Scripts/MainCharacterController.cs
Assets/Scripts/AppEvents/PlayerFootstepsEmitter.cs
Assets/Scripts/AppEvents/PlayerFootstepsEvent.cs
Assets/Scripts/AppEvents/AudioEventManager.cs
Assets/Scripts/AINavV2.cs
Assets/Scripts/MageCombatController.cs
Assets/Scripts/AppEvents/EnemyAttackingEmitter.cs
Assets/Scripts/AppEvents/EnemyAttackingEvent.cs

Zhipeng Zhu:
1. Dialogue system, dialogue chat box, and choice option box. 
2. Two playable characters can talk to each other and NPCs, depending on the choice made with the NPCs, they will either turn hostile or remain friendly. 
3. Player health bar, enemy health bar. 
4. Player characters' health bar and enemy characters' health bar will decrease or increase to reflect the actual damage taken or heal received. Main menu scene to start game and quit game. 
5. In-game pause menu, to restart game or to quit game.

Healthbar.png, UI Prefab: DialogueCanvas, EnemyHealth, InteractionPrompt
Scripts:
BillBoard.cs, DialogueManager.cs, EnemyUIManager.cs, HealthBar.cs, MainMenu.cs, NPCInteraction.cs, PlayerUIManager.cs, ChoiceTextHighlight.cs, PauseMenu.cs, TextHighlightOnHover.cs, TwoColorText.cs

Zhe Dang 
1. Enemy Prefab Setup
Created 6 interactive enemy prefabs complete with weapons, hitboxes, and attack VFX.
Files: Cordan_Container, Janice_Container, Leo_Container, Tink_Container, TY_Container, Vans_Container
2. Core Combat & Animation
Built the base logic for enemy health and movement. Set up Animators to handle specific attack styles like casting or thrusting.
Animators: EnemyCast, EnemyShockWave, EnemyThrust
3. Slime AI Logic
Behavior: Switches between Idle/Roam and Chase/Attack. The AI detects the player within range to attack and returns to patrolling when the player leaves.
4. Input System
Configured PlayerInput.inputactions to include full gamepad support alongside keyboard controls.

Scripts: 
SlimeAIController.cs, SlimeCombat.cs, EnemyStatus.cs, EnemyController.cs

Qianqian: 
Alpha:
1. combat controller scripts for player characters (melee and mage).
2. health system for player characters.
3. Attack animator and animation for one of the main charater (Alhaitham).
4. Weapon handler script for Alhaitham (melee).
5. Add weapon to Alhaitham's hand, and show weapon only when attack.
6. Add VFX visual effect to Alhaitham (code in weapon handler, "add event" to the animation at the appropriate time frame to trigger visual effect)
7. Collectable Item script to track collectable objective.
8. NPC objective script to track NPC conversation objective.
9. Objective manager script to manage objective.
10. UI for objective.
11. objective UI script to update objective progress on UI.
12. Build scene: background buildings and trees.
13. Import enemy character models. 
14. Write game story focus mainly on NPC and character dialogues.

Final Game:
15. Wrote EnemySpawner.cs to automatically generate slime without replacement on the map.
16. Wrote NPCSpawener.cs and NPCWalkers.cs to automatically generate NPCs that walks around with replacement on the map. NPC disappear after reaching the boundary to prevent cumulation.
17. Added FriendlyNPCs with different status: playing guitar (wrote GuitarHandler.cs and GuitarMusic.cs and add guitar music for sound effect when player approaches), chatting, arguing, dancing, clapping, cheering, laying down, etc.
18. Modified game terrain with additional town buildings added.
19. Wrote choices in the game, dilemma leading to different consequences (friendly outcome vs. NPC turning into enemy)
20. Add screens to create impression of lively town, 
21. using the same mechanism for creating the screen, add star effects to the bridge.
22. Add directional lights, candles in the tunnel for better shading and lightning style.
23. Modified NPCObjectiveTracker.cs, InteractObjective.cs, ObjectiveUI.cs and EnemyStatus.cs to fix bugs in counting the NPCs in the objective

Scripts:
CombatController.cs
MageCombatController.cs
WeaponHandler.cs
HealthSystem.cs
CollectableItem.cs
ObjectiveManager.cs
NPCObjectiveTracker.cs
ObjectiveUI.cs
EnemyStatus.cs (only modified it to debug)
InteractObjective.cs (only modified it to debug)
EnemySpawner.cs
NPCSpawener.cs
NPCWalkers.cs
GuitarHandler.cs
GuitarMusic.cs


Zhuoyun Cai
Pressure Button Trigger:
When a character stands on a pressure button, it becomes active and plays the corresponding button animation and sound.
After 3 seconds, the linked platform or slope moves to its designated position, allowing the character to continue along the path.
AI Navmesh:
Baked the appropriate Navmesh in order to make sure the AI-controlled characters can follow the player and jump to reach the player’s position.
Pickups:
Rotating pickups are placed along the path to guide the player in the correct direction.
When collected, pickups disappear.

Scripts:
AudioPlay.cs
GateTrigger.cs
GateTriggerDown.cs
PlayerCollectable.cs
PressureButton.cs
PressureButtonDown.cs
PressureButtonTrap.cs
PressureButtonUp.cs
Rotator.cs
SlopeTrigger.cs
TrapTrigger.cs
TrapTriggerUp.cs
