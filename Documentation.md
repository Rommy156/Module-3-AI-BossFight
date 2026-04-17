# Enemy AI State Machine Documentation

## Overview
The enemy AI uses a finite state machine (FSM) with five states:
- Circle  
- Approach  
- Attack1  
- Attack2  
- Charge  

The system controls behavior using distance checks, timers, and hit detection to create a responsive combat loop.

---

## State Behaviors & Transitions

### Circle
**Behavior**
- Default state  
- Enemy stays near and faces the player  

**Transitions**
- → Approach: player detected  
- → Attack1: player in range  

---

### Approach
**Behavior**
- Moves toward player using NavMeshAgent  

**Transitions**
- → Attack1: within attack range  
- → Charge: if damaged (`hitsTaken >= threshold`)  

---

### Attack1
**Behavior**
- First attack  
- Stops movement  
- Uses animation event for hit detection  

**Transitions**
- → Attack2: if hit lands  
- → Circle: if attack misses  

---

### Attack2
**Behavior**
- Combo follow-up attack  

**Transitions**
- → Approach: after attack finishes  
- → Charge: if damaged  

---

### Charge (Dash Attack)
**Behavior**
- Enemy dashes toward player  
- Moves using stored direction (`chargeDir`)  
- Rotation is disabled during dash  

**Transitions**
- → Circle: after timer (~3s)  
- Resets damage counter  

---

## Key Features
- Timer-based transitions (prevents state spam)  
- Animator `SetBool` controls animations per state  
- Hit validation using `OverlapSphere` + animation events  
- Vision system (radius + angle + raycast)  
- Sound trigger system for reactive behavior  

---

## Challenges & Solutions

**States running every frame**  
- Fixed using timers and `hasAttackedThisState` flag  

**Hits registering from far away**  
- Fixed using `Physics.OverlapSphere` range check  

**Charge looping constantly**  
- Stored direction once and prevented re-trigger  

**Attack2 not triggering**  
- Corrected hit count logic and transition timing  

**Retreat movement issues**  
- Replaced with controlled dash using `agent.Move`  
