# Archive of All Knowledge

You, as the player, wake up on a deserted central island. The only thing you see is a board — “Archive of All Knowledge” with dim stars, and the only goal in your mind: light them up. There are other mysterious islands, each with a quest you have to complete. Besides these quests, there seems to be some other NPCs who are waiting for your help. Would you try to build connections to grasp even the smallest idea of what this world is about, or rush through your journey for your own escapefor the sake of efficiency ignore them? What awaits you after the stars are all lit up? 

## Game Theme

Archive of All Knowledge is a 2D RPG game structuring around a main story discussing the relationship between humanity and AI:
- How AI is trained. This will be reflected by the 3 main quests the player has to complete, each corresponds to a domain of AI: Computer vision, robotics, NLP. Some people have technophobia because they don’t know how such amazing technology works.
- How are humans different from AI. AI is trained to give high accuracy answers by billions of data, making them smart and logical all the time, giving appropriate suggestions and fast search results. On the other hand, humans will not always be logical and objective, but this is not a bad thing. For each person has the gift of "choice".

## Gameplay

To echo our game theme, the gameplay flows through a continuous plot together with its own minigame:
- Island of Sight (Vision): restore light in a dark maze.
- Island of Action (Robotics): survive a timed combat gauntlet.
- Island of Language (NLP): solve logic and word puzzles.
- along with 3 optional subquests, 1 on each island (dialogue-based)
- An initially locked island, which the player can only enter when all 3 islands are completed. Here the ending will be presented.

As each island is completed, a star is lit on the central task board, progressing the story. 

For each minigame, the player can only complete the island if they meet the success requirement. If not, they are required to repeat the minigame until they meet the success requirement.

After all 3 islands are complete (signalized by the stars on the task board on the main island), the game transitions toward a final revelation and 2 completely different ending branches. 
The stakes of the game is dependent on the subquest choices the player choose, the player will either unlock a good ending or a bad ending based on their choices.

To unlock an ending, the player should finish all main qests on the 3 islands (subquests are optional) to enter the final island. Oncen the player ends the dialogue with the final island NPC, the game will be considered finish and tops immediately.

The narrative arc and full story outline are documented in `Assets/Plot.txt`.

## Engine and Tech Stack

- Unity version: `2022.3.62f3c1`
- Render pipeline: Universal Render Pipeline (URP) 2D
- Input model: classic Unity input axes (`Horizontal`, `Vertical`) and key checks
- Dialogue UI: TextMeshPro-based dialogue panel and choice buttons
- Sprites: generated from Nano-banana
- Music: Mubert & Suno v5.5

Key packages (from `Packages/manifest.json`):

- `com.unity.render-pipelines.universal`
- `com.unity.textmeshpro`
- `com.unity.cinemachine`
- Unity 2D feature packages (`tilemap`, sprite tools, physics2d)

## How To Open and Run

Only use the `main` branch.

1. Open Unity Hub.
2. Add this folder as a project:
   - `Archive-of-All-Knowledge`
3. Make sure Unity `2022.3.62f3c1` is installed and selected.
4. Open a playable scene from `Assets/Scenes/`.
5. Press Play. It should automatically take you to `Central_island.unity` no matter which scene you play.

Main scene assets currently include:
- 1 scene for main island
- 3 scenes for vision island
- 3 scenes for robotics island
- 2 scenes for NLP island
- 1 scene for final island

## Controls

- Move: arrow keys
- Interact with NPCs and doors: `E`
- Continue dialogue: click/`space`
- shoot bullets in Robotics island: click on mouse

Notes:
- movement is paused while dialogue is active.
- Every interactable object has a speech bubble when entering its trigger area, hinting the player that the object is interactable. (Including NPCs, doors, special objects)

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

Notes:
- The `HumanityScore` will affect which ending the player receives, and is modified by exploring subquests and the final choice.
- There are currently 2 endings

### 3) Scene Transition and Spawn Positioning

Door travel is handled by `Assets/Scripts/DoorTeleport.cs`:

- On `E`, loads configured target scene.
- Stores `LastExitName` via `PlayerPrefs`.

Spawn recovery is handled by `Assets/Scripts/PlayerSpawnHandler.cs`:

- Finds a scene object named by `LastExitName`.
- Moves player to that spawn point.

### 4) Quest Completion Triggering

`Assets/Scripts/IslandQuestCompletionTrigger.cs` can mark stars complete when the player enters a trigger volume.
This will also be reflected on the sprite changes on the main island. (i.e. the stars will be lit up upon completing the main quest on the corresponding island)

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

### 5) Robotics Island minigame state and progression
`RoboticsGameManager`  is a scene-specific singleton that manages the survival combat mechanics:

- `Timer` (gameTimer, _currentTime)

- Minigame State (`_isGameActive`, `CompleteChallenge`)

- Enemy Scaling (`spawnRate`, `spawnPoints`)

Important methods in Assets/Scripts/RoboticsGameManager.cs:

`StartGame()`

`SpawnEnemy()`

`EndGame(bool didWin)` (checks if win or gameOver)

`LoadIslandScene()`

`RestartGame()`


## Appendix: Quick Start For Dialogue Authors

1. Create or edit a JSON file in `Assets/DialogueData/`.
2. Keep the schema compatible with `DialogueData` runtime parsing.
3. Assign the JSON file to an NPC via `NPCDialogue -> Dialogue Json`.
4. Ensure a scene-level `DialogueManager` is present and wired to UI references.
5. Play test: enter trigger, press `E`, verify text and branching choices.

## License and Ownership

All Rights Reserved.
