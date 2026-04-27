# Archive of All Knowledge

You, as the player, wake up on a deserted central island. The only thing you see is a board — “Archive of All Knowledge” with dim stars, and the only goal in your mind: light them up. There are other mysterious islands, each with a quest you have to complete. Besides these quests, there seems to be some other NPCs who are waiting for your help. Would you try to build connections to grasp even the smallest idea of what this world is about, or rush through your journey for your own escapefor the sake of efficiency ignore them? What awaits you after the stars are all lit up? 

## Project Overview

Archive of All Knowledge is structured around three core challenge islands:

- Island of Sight (Vision): restore light in darkness.
- Island of Action (Robotics/Forge): survive a timed combat gauntlet.
- Island of Language (NLP): solve logic and word puzzles.

As each island is completed, a star is lit on the central task board. After all three stars are complete, the game transitions toward a final revelation and ending branch.

The narrative arc and full story outline are documented in `Assets/Plot.txt`.

## Engine and Tech Stack

- Unity version: `2022.3.62f3c1`
- Render pipeline: Universal Render Pipeline (URP) 2D
- Input model: classic Unity input axes (`Horizontal`, `Vertical`) and key checks
- Dialogue UI: TextMeshPro-based dialogue panel and choice buttons

Key packages (from `Packages/manifest.json`):

- `com.unity.render-pipelines.universal`
- `com.unity.textmeshpro`
- `com.unity.cinemachine`
- Unity 2D feature packages (`tilemap`, sprite tools, physics2d)

## How To Open and Run

1. Open Unity Hub.
2. Add this folder as a project:
   - `Archive-of-All-Knowledge`
3. Make sure Unity `2022.3.62f3c1` is installed and selected.
4. Open a playable scene from `Assets/Scenes/`.
5. Press Play.

Main scene assets currently include:

- `Central_island.unity`
- `Vision_island.unity`
- `Vision_quest.unity`
- `Vision_complete.unity`
- `NLP_island.unity`
- `NLP_island_complete.unity`
- `Final_island.unity`

## Controls (Current)

- Move: arrow keys
- Interact with NPCs and doors: `E`
- Continue dialogue: click

Note: movement is paused while dialogue is active.

## Core Gameplay Systems

### 1) Dialogue System

Dialogue is data-driven and supports multiple input sources:

- ScriptableObject (`DialogueData`)
- JSON TextAsset
- JSON loaded from Resources path
- Inline JSON text

Primary scripts:

- `Assets/Scripts/DialogueData.cs`
- `Assets/Scripts/DialogueJsonLoader.cs`
- `Assets/Scripts/DialogueManager.cs`
- `Assets/Scripts/NPCDialogue.cs`
- `Assets/Scripts/PlayerDialogue.cs`

Dialogue content files are in `Assets/DialogueData/`.

JSON schema (simplified):

- `startNodeId`
- `nodes[]`
  - `nodeId`
  - `lines[]`
    - `speakerName`
    - `text`
    - `hasChoice`
    - `choices[]`
      - `choiceText`
      - `nextNodeId`
      - `humanityModifier`
      - optional completion flags

### 2) Game State and Progression

`GameManager` is a singleton (`DontDestroyOnLoad`) that tracks:

- Humanity score (`humanityScore`)
- Island completion stars (`starCompletion[3]`)

Important methods in `Assets/Scripts/GameManager.cs`:

- `ModifyHumanity(int amount)`
- `CompleteStar(int islandIndex)`
- `GetHumanityScore()`
- `IsStarComplete(int islandIndex)`

Events:

- `OnHumanityChanged`
- `OnStarCompleted`

`TaskBoardStarsController` listens for star updates and toggles lit/unlit star visuals.

### 3) Scene Transition and Spawn Positioning

Door travel is handled by `Assets/Scripts/DoorTeleport.cs`:

- On `E`, loads configured target scene.
- Stores `LastExitName` via `PlayerPrefs`.

Spawn recovery is handled by `Assets/Scripts/PlayerSpawnHandler.cs`:

- Finds a scene object named by `LastExitName`.
- Moves player to that spawn point.

### 4) Quest Completion Triggering

`Assets/Scripts/IslandQuestCompletionTrigger.cs` can mark stars complete when the player enters a trigger volume.

## Narrative Structure (High Level)

- Prologue on a central island with a task board and three hints.
- Three domain islands test sight, action, and language.
- Optional side choices influence the hidden humanity direction.
- Final island reveals the ending and branches toward different outcomes.

## Current Development Notes

- Star completion logging exists for "all stars complete" in `GameManager`.
- Final island unlock behavior is currently marked by comment and may require additional implementation depending on desired world/scene gating logic.
- Dialogue pipeline already supports JSON-first authoring and branch-driven progression.

## Repository Structure (Important Folders)

- `Assets/` game assets, scenes, scripts, dialogue data
- `Assets/Scripts/` gameplay and system scripts
- `Assets/DialogueData/` dialogue JSON files and dialogue setup guide
- `Assets/Scenes/` Unity scenes
- `docs/` project documents, architecture, workflows
- `ProjectSettings/` Unity project configuration
- `Packages/` package manifest and lock

## Team Workflow Context

The repository includes substantial process documentation in `docs/`, including:

- `docs/WORKFLOW-GUIDE.md`
- `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md`

These files describe the intended collaborative design and development process around this game project.

## Quick Start For Dialogue Authors

1. Create or edit a JSON file in `Assets/DialogueData/`.
2. Keep the schema compatible with `DialogueData` runtime parsing.
3. Assign the JSON file to an NPC via `NPCDialogue -> Dialogue Json`.
4. Ensure a scene-level `DialogueManager` is present and wired to UI references.
5. Play test: enter trigger, press `E`, verify text and branching choices.

## License and Ownership

No license file is currently defined at the repository root.
If this project is intended for distribution, add a license file and update this section.
