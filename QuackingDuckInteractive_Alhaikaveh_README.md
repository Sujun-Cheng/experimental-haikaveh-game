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
- Cinemachine plugin for game, such that the camera is always centered on the character the player is in control of.
- Porting the new Input system for navigation, making sure that the character always moves relatively to the camera.
- Imported models for the player characters (Official Bilibili models from Genshin) as MMD, ported into Blender, and imported as FBX into Unity. 
	-Model: 
		\Assets\ModelsAndAnimations\Kaveh
		\Assets\ModelsAndAnimations\Alhaitham
Character Movement:
Player Character locomotion:
- Walk, Run, Sprint, Jump and Falling animations in the animator tree. Reparameterized movement such that it is represented by a unit vector and a magnitude for all movement in the game to standardize it. 
	-Animators: 
		Assets/AnimatorControllers/AlhaithamAnimatorController.controller
		Assets/AnimatorControllers/KavehAnimatorController.controller
- Added sound effects for walking
AI Follower Navigation:
- Walk, Run, Sprint, Jump. 
- AI Character follower navigation, including traversing nav mesh links and synchronizing ai nav agent position with character transform position in order to follower player
- Combat animation and combat effects for character Kaveh
- VFX Effects for character Kaveh	
Story:
- Wrote part of the dialogue, the premise, and the story
Music:
- Composed both ambient and battle music in game.
	-Music: Assets/Music
- Configured music manager such that the music would switch depending on if there is an enemy who is in combat
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
23. Modified NPCObjectiveTracker.cs, InteractObjective.cs, ObjectiveUI.cs and EnemyStatus.cs to fix bugs in counting the NPCs in the objective (comment with // Prevent double counting)

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
