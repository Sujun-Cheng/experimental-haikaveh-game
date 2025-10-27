Game Instructions

1. Open the Scene
Open the scene file:
assets/scenes/MainMenu.unity

2. Start the Game
Basic Controls:

WASD – Move character

Mouse – Adjust camera view

Tab – Switch between characters

Space – Jump

Shift/Right Mouse Button - Sprint

1-0 - Variable Speed

Left Mouse Button – Attack

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
Two playable characters can talk to each other and NPCs, depending on the choice made with the NPCs, they will either turn hostile or remain friendly. 
Player health bar, enemy health bar. 
Player characters' health bar and enemy characters' health bar will decrease or increase to reflect the actual damage taken or heal received.
Main menu scene to start game and quit game.
2. Healthbar.png, UI Prefab: DialogueCanvas, EnemyHealth, InteractionPrompt
3. BillBoard.cs, DialogueManager.cs, EnemyUIManager.cs, HealthBar.cs, MainMenu.cs, NPCInteraction.cs, PlayerUIManager.cs, StartofGameDialogue.cs

Zhe Dang 
completed all enemies features including:
Scripts:
EnemyStatus.cs, EnemyController.cs
Animators:
EnemyCast, EnemyShockWave, EnemyThrust
Enemy Prefabs:
Cordan_Container, Janice_Container, Leo_Container, Tink_Container, TY_Container, Vans_Container

Qianqian: 
combat controller scripts for player characters (melee and mage).
health system for player characters.
Attack animator and animation for one of the main charater (Alhaitham).
Weapon handler script for Alhaitham (melee).
Add weapon to Alhaitham's hand, and show weapon only when attack.
Add VFX visual effect to Alhaitham (code in weapon handler, "add event" to the animation at the appropriate time frame to trigger visual effect)
Collectable Item script to track collectable objective.
NPC objective script to track NPC conversation objective.
Objective manager script to manage objective.
UI for objective.
objective UI script to update objective progress on UI.
Build scene: background buildings and trees.
Import enemy character models. 
Write game story focus mainly on enemy dialogue.
Scripts:
CombatController.cs
MageCombatController.cs
WeaponHandler.cs
HealthSystem.cs
CollectableItem.cs
ObjectiveManager.cs
NPCObjectiveTracker.cs
ObjectiveUI.cs