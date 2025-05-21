
# SideScrollerActionProject

A personal 2.5D side-scroller action RPG project I am working on in my free times. It lacks comments and explanations at some points and scripts since it is a personal project and there is no other person working on it. Therefore I'm only commenting when I feel like I might forget something on the specific script.

This project showcases various aspects of my game project including:


#### Character controller
An extensive character controller system that I use for both player and AI characters, including new <b>Input System</b> support. It includes features like:

- Dynamic rigidbody based precise controls similar to kinematic rigidbody
- Smooth step up/down with distance-height fine tuning controls
- Slope angle (sliding down above the angle)
- Collision information and events
- Coyote time (jumping right after leaving the ground)
- Custom gravity support on any axis
- Character states using FSM
- AI actions support
- 

>and various other features.


#### Custom Animation System
Since <b>Unity Animation</b> visual state machines cause headaches and becomes spaghetti easily, I have decided that I can use an asset I'd bought within a bundle way before, <b>Animancer</b>, and gave it a try. After some time that I used to experiment with the asset, it has become way easier for me to control and define animation state changes and interpolation conditions.
It increased the amount of code I have to maintain but after finding a way to create animation states for the character's available character states, it became fairly easy to maintain for my project.
I'm not using default animation system in this project anymore.

#### Custom AI system compatible with XY axes of the game (not mature yet)
- Perception system for AI characters to detect their surroundings, using non-alloc overlap spheres to reduce unnecessary garbage generation
- Basic sequences like follow, move etc.

#### Platforming Elements
>Ledges, vaults, ropes, ladders, slide walls, wall jumping etc.

Since the game is planned to be a Platformer Action RPG, it includes various platforming elements (more to come).
It also benefits from <b>Inverse Kinematics</b>, for example, when the character is vaulting, it connects its right hand to the vault object's relative hand transform, making it more of a believable vault jump.

#### Surface and Volumes
There are surfaces and volumes defined by their own settings, changing the player and other characters' speed, acceleration, deceleration, jump height etc. properties.

#### Faction System
The game has a basic faction system for player, ally, neutral and enemy actors to make things easier between them to identify their surroundings

#### Character and Environment Profiles
- Profile storages for every character and non character (i.e. breakable objects) defining their base stats and necessary information, also preparing them for a robust save system later on.

#### Combat System
Though the system is not in its mature stage, I believe I've created a good basic hierarchy for combat system:

- "Damageable" component for both characters and environment (breakables) that can be engaged with melee or long-range combat
- All fighter actors have <b>Attack Storage</b> ScriptableObject that contain <b>AttackElement</b>s that contain the necessary information related to the specific attack like damage type, amount, status effects, pushback, knockdown values etc. and a <b>DamageSource</b> prefab component which is the physical form of the attack's damage.
- DamageSource component carries information like owner (who sent the attack), damage area etc.
- It also uses non-alloc sphere casting with a resizeable array of hit targets, eliminating constant trigger checks, therefore unnecessary garbage generation.

####  Local Event Manager System
Aside from the game's main event system that handles global events, every Actor including the Player has their own "LocalEventManager", helping their various components send messages to each other without the necessity of keeping references for not-so-related other components.

#### Inventory System
The game contains a basic form of inventory system which is not mature yet but enough to handle the player's weapon inventory, also initiating the "Item" system and thus item databases. Items are divided into equipable, consumable, non-equipable items in the form of ScriptableObjects.

#### Object Pooling
The game utilizes object pooling for damage sources for now, which will expand to vfx, particles, ammo (arrows etc), magic effects, actors (enemy and ally prefabs) and so on, every poolable object has their own optimization settings like pool size.


---
In terms of design, I've used singleton patterns, state machines, various event systems that perform well. I have also used abstraction and interfaces where it mattered. The project is still in its early stages but it has a solid basis for the upcoming features.