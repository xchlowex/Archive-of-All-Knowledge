# Scripts Documentation

Overview of all functions and systems in the game scripts folder.

## Core Dialogue System

### DialogueData.cs
Data models for dialogue branching system.

**Classes:**
- `DialogueLine` - Individual dialogue line with speaker name, text, and optional choices
- `DialogueChoice` - Player choice with text, next node reference, and humanity modifier (-1 for AI, +1 for human)
- `DialogueNode` - Container for multiple dialogue lines, identified by nodeId
- `DialogueData` - ScriptableObject that holds list of nodes and starting node ID

---

### DialogueJsonLoader.cs
Static utility for loading dialogue data from JSON sources.

**Functions:**
- `LoadFromTextAsset(TextAsset jsonAsset)` - Load dialogue from a TextAsset, validates input and delegates to JSON parser
- `LoadFromJson(string json, string sourceName = "JSON")` - Parse JSON into DialogueData, creates runtime instance, validates node existence, auto-fills startNodeId if missing, logs all errors

---

### DialogueManager.cs
Runtime dialogue UI controller with typewriter effect and branching support. **Singleton pattern.**

**Functions:**
- `StartDialogue(DialogueData dialogue)` - Initialize current dialogue/node/index and display first line
- `DisplayCurrentLine()` - Set speaker text, manage choices, start typewriter effect
- `TypeText(string text)` - Character-by-character animation based on textSpeed setting
- `WaitAndShowChoices(List<DialogueChoice> choices)` - Wait for typing to finish, then display choice buttons
- `ShowChoices(List<DialogueChoice> choices)` - Instantiate choice buttons and bind click handlers
- `OnChoiceSelected(DialogueChoice choice)` - Apply humanity modifier via GameManager, move to next node or end dialogue
- `ContinueDialogue()` - Skip typing animation or advance to next line/node
- `EndDialogue()` - Close UI panels and clear dialogue state
- `Update()` - Listen for mouse click or spacebar to continue

**Properties:**
- `IsDialogueActive` - True if dialogue panel is visible

---

### Dialogue_example.cs
Legacy/example dialogue system with simple line array (deprecated, use DialogueManager instead).

**Functions:**
- `Start()` - Clear text and begin dialogue
- `StartDialogue()` - Reset line index and start typing coroutine
- `TypeLine()` - Character-by-character rendering via coroutine
- `NextLine()` - Advance to next line or disable when finished
- `Update()` - Mouse click advances line or skips to full text instantly

---

## Interaction & NPC System

### NPCBase.cs
Base class for all interactive NPCs, implements IInteractable interface.

**Functions:**
- `ShowPrompt()` - Display interaction prompt, face player, trigger greet animation
- `HidePrompt()` - Hide interaction prompt, stop talking animation
- `Interact()` - Virtual method for subclasses to override (call global dialogue system)
- `SetTalking(bool isTalking)` - Control NPC talking animation state
- `FacePlayer()` - Flip NPC sprite to face player direction

**Animator Parameters:**
- `greetTrigger` - Animation trigger when prompted (default: "greet")
- `talkBool` - Animation bool for talking state (default: "isTalking")

---

### NPCDialogue.cs
Concrete NPC implementation with flexible dialogue source resolution.

**Functions:**
- `Interact()` - Resolve dialogue source and start dialogue via DialogueManager
- `ShowPrompt()` / `HidePrompt()` - Toggle interaction prompt visibility
- `GetResolvedDialogue()` - Check dialogue sources in priority order: DialogueData field → cached runtime JSON → Resources folder JSON → inline JSON text → assigned TextAsset
- `OnDestroy()` - Clean up runtime-created dialogue assets

**Dialogue Sources (in priority order):**
1. Direct DialogueData ScriptableObject reference
2. TextAsset JSON file
3. Resources folder path string
4. Inline JSON text

---

### PromptHover.cs
Helper component for interaction prompt visual feedback (mouse hover animation).

**Functions:**
- `Start()` - Find parent NPCBase component
- `OnMouseEnter()` - Start NPC talking animation when mouse hovers over prompt
- `OnMouseExit()` - Stop NPC talking animation when mouse leaves prompt

**Note:** Works with OnMouseEnter/Exit, requires collider on prompt object.

---

## Player Control & Movement

### PlayerMovement.cs
Handles player movement input, velocity, and directional animations.

**Functions:**
- `Start()` - Cache Rigidbody2D, set top-down physics (gravity=0, freeze rotation), find animator and sprite renderer
- `Update()` - Freeze movement during dialogue, read arrow/WASD input, update animations
- `FixedUpdate()` - Apply velocity to Rigidbody based on input
- `HandleAnimations()` - Set directional animation bools (isRunningX, isRunningLeft, isRunningFront, isRunningBack), flip sprite based on direction

**Input:** Arrow keys or WASD (via Horizontal/Vertical axes)

**Animator Parameters:**
- `isRunningX` - Moving right
- `isRunningLeft` - Moving left
- `isRunningFront` - Moving down
- `isRunningBack` - Moving up

---

### PlayerDialogue.cs
Handles player interaction with NPCs (find nearest, show/hide prompts).

**Functions:**
- `Update()` - Check for E key press with nearest interactable (only when dialogue not active)
- `OnTriggerEnter2D(Collider2D other)` - Track interactables in range and show prompt
- `OnTriggerExit2D(Collider2D other)` - Remove interactables from range and hide prompt
- `RefreshCurrentInteractable()` - Pick the closest interactable in range
- `GetInteractableFromCollider(Collider2D other)` - Find IInteractable on collider or parent
- `OnDisable()` - Hide all prompts and clear state

**Input:** E key to interact with nearest NPC

---

### PlayerControl.cs
Alternative player controller combining movement + interaction (similar to PlayerDialogue but with movement integration).

**Functions:**
- `Start()` - Cache Rigidbody2D, find animator, validate components
- `Update()` - Freeze controls during dialogue, read movement input, update animations, handle interaction with E key
- `FixedUpdate()` - Apply movement velocity
- `HandleAnimations()` - Set directional animation bools with deadzone threshold (0.01f)
- `OnTriggerEnter2D(Collider2D other)` - Track and show prompt for interactables
- `OnTriggerExit2D(Collider2D other)` - Remove and hide prompt for interactables
- `RefreshCurrentInteractable()` - Nearest interactable selection
- `GetInteractableFromCollider(Collider2D other)` - Retrieve IInteractable interface
- `OnDisable()` - Cleanup

**Note:** PlayerControl + PlayerDialogue are largely redundant; keep only one in your final project.

---

## Scene Management

### DoorTeleport.cs
Trigger-based scene transition with player spawn point passing via PlayerPrefs.

**Functions:**
- `Start()` - Hide tutorial prompt initially
- `Update()` - When player in range and presses E, save spawn point name and load target scene
- `OnTriggerEnter2D(Collider2D other)` - Player enters door range, show prompt
- `OnTriggerExit2D(Collider2D other)` - Player leaves door range, hide prompt

**Inspector Fields:**
- `sceneToLoad` - Target scene name
- `spawnPointName` - Name of spawn GameObject in target scene (stored in PlayerPrefs as "LastExitName")
- `tutorialObject` - "Press E" UI element

---

### PlayerSpawnHandler.cs
Reads spawn point from PlayerPrefs and repositions player at scene load.

**Functions:**
- `Start()` - Read "LastExitName" from PlayerPrefs, find matching GameObject, move player to that position

**Dependencies:** DoorTeleport must set "LastExitName" before loading scene.

---

## Global State Management

### GameManager.cs
Singleton for global game state (humanity score, star completion tracking). **Persists across scenes via DontDestroyOnLoad.**

**Functions:**
- `ModifyHumanity(int amount)` - Add/subtract from humanity score, invoke OnHumanityChanged event
- `CompleteStar(int islandIndex)` - Mark island star as complete, invoke OnStarCompleted event, check if all stars complete and log unlock message
- `GetHumanityScore()` - Return current humanity value
- `IsStarComplete(int islandIndex)` - Check if specific star is complete

**Events:**
- `OnHumanityChanged(int newScore)` - Fired when humanity changes
- `OnStarCompleted(int islandIndex, bool isComplete)` - Fired when star completes

**Global State:**
- `humanityScore` - Player ethics alignment (-1 from AI choices, +1 from human choices)
- `starCompletion` - Array of 3 booleans (one per island)

---

## Interfaces

### IInteractable
Interface for all interactive game objects.

**Functions:**
- `Interact()` - Called when player presses E while in range
- `ShowPrompt()` - Display interaction hint (e.g., "Press E" bubble)
- `HidePrompt()` - Hide interaction hint

**Implementations:** NPCBase, NPCDialogue

---

## System Architecture Summary

```
INPUT (PlayerMovement/PlayerDialogue)
  ↓
INTERACT (E key) → IInteractable.Interact()
  ↓
NPC/DIALOGUE (NPCDialogue.Interact())
  ↓
DialogueManager (Singleton)
  ↓
DialogueData (branching tree)
  ↓
CHOICE → ModifyHumanity(GameManager)
  ↓
GameManager.OnHumanityChanged (event for UI)

SCENE TRANSITION:
INPUT (E at door) → DoorTeleport.Update()
  ↓
PlayerPrefs.SetString("LastExitName") → Load scene
  ↓
PlayerSpawnHandler.Start() → Read PlayerPrefs → Position player
```

---

## Known Issues & Redundancy

- **PlayerMovement + PlayerDialogue + PlayerControl**: Three overlapping player control scripts. Recommend keeping only **PlayerControl** (most complete) and removing the other two.
- **Dialogue_example.cs**: Legacy example script, not integrated with main dialogue system. Safe to delete.
- **PromptHover.cs**: Only works with mouse hover (OnMouseEnter/Exit), not keyboard interaction. Consider removing or updating for consistency.

---

## Quick Setup Checklist

- [ ] Attach PlayerMovement (or PlayerControl) to Player GameObject
- [ ] Attach PlayerSpawnHandler to Player GameObject
- [ ] Attach DoorTeleport to door colliders
- [ ] Create DialogueManager in scene (singleton will persist)
- [ ] Attach NPCDialogue to NPC GameObjects
- [ ] Attach GameManager to persistent GameObject (survives scene loads)
