# Enemy AI State Machine (Unity)

https://youtu.be/DdxdhO6CiA0

## Overview
This project implements a third-person enemy AI using a Finite State Machine (FSM) in Unity. The system controls enemy behavior during combat, including movement, detection, attacking, and reactive actions.

## Features
- Camera-relative player movement (New Input System)
- Enemy AI with FSM behavior
- Lock-on system (targets enemies in front of camera)
- Animation-driven combat (hit detection via events)
- NavMesh-based movement

## Enemy States
- **Circle**  
  Enemy orbits/positions around the player and waits for engagement.

- **Approach**  
  Moves toward the player using NavMeshAgent.

- **Attack1**  
  Primary attack. Triggers when in range.

- **Attack2**  
  Combo follow-up attack after successful hit.

- **Charge**  
  Dash attack toward player after taking damage.

## Controls
| Action   | Key |
|----------|-----|
| Move     | WASD |
| Attack   | Left Click |
| Evade    | Shift |
| Lock-On  | Tab |

## Tech Used
- Unity (2022+)
- C#
- NavMesh AI
- Animator Controller
- Unity Input System
- Cinemachine FreeLook Camera

## Known Issues
- NavMesh must be baked or enemy will not move
- Missing references may cause NullReference errors
- Lock-on requires enemies to be in front of camera

## Setup
1. Open project in Unity
2. Bake NavMesh (`Window > AI > Navigation`)
3. Assign references:
   - Player Transform
   - Hitbox
   - Animator
4. Press Play

## Notes
- FSM uses timers to prevent state spam
- Attack damage only applies during animation events
- System is modular and can be expanded with more states
