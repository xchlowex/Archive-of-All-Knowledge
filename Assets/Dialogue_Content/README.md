# Dialogue System Setup Guide (Unity)

This folder contains dialogue JSON files that match the project's `DialogueData` schema.

Use this guide to implement and test a full NPC conversation flow:
- Player enters NPC trigger
- Player presses `E`
- Dialogue UI appears
- Text and choices are shown from JSON

## 1. JSON format expected by scripts

Each file should follow this shape:
- `startNodeId`
- `nodes` (array)
- each node: `nodeId`, `lines`
- each line: `speakerName`, `text`, `hasChoice`, `choices`
- each choice: `choiceText`, `nextNodeId`, `humanityModifier`

Example root keys:
```json
{
	"startNodeId": "start",
	"nodes": [
		{
			"nodeId": "start",
			"lines": [
				{
					"speakerName": "NPC",
					"text": "Welcome.",
					"hasChoice": false,
					"choices": []
				}
			]
		}
	]
}
```

## 2. Required scripts used by this project

Scene scripts are expected under `assets/Scripts`:
- `DialogueData.cs`
- `DialogueJsonLoader.cs`
- `DialogueManager.cs`
- `PlayerDialogue.cs`
- `NPCDialogue.cs`

`NPCDialogue` supports four dialogue sources (priority order):
1. `Dialogue Data` (`DialogueData` asset)
2. `Dialogue Json` (`TextAsset`)
3. `Dialogue Json Resources Path` (string path in `Resources`)
4. `Dialogue Json Text` (inline JSON pasted in inspector)

## 3. Scene setup

### A. DialogueManager object
1. Create empty object `DialogueManager`.
2. Attach `DialogueManager.cs`.
3. Assign references in inspector:
	 - `Dialogue Panel` (UI panel root)
	 - `Speaker Text` (TextMeshProUGUI)
	 - `Dialogue Text` (TextMeshProUGUI)
	 - `Choice Panel` (UI container)
	 - `Choice Button Prefab` (Button with TMP text child)

### B. Player object
1. Attach `PlayerDialogue.cs`.
2. Ensure player has:
	 - `Rigidbody2D`
	 - `Collider2D`
3. Leave `Interact Key` as `E` unless changed intentionally.

### C. NPC object
1. Attach `NPCDialogue.cs`.
2. Add `BoxCollider2D` (or other `Collider2D`) and enable `Is Trigger`.
3. Set one dialogue source:
	 - Preferred for JSON: assign `Dialogue Json` with a `TextAsset`.
4. `UI Interaction Prompt` is optional.

## 4. How to use JSON directly (recommended)

1. Copy your json file into Unity `Assets` so Unity imports it.
2. Confirm it appears as a text asset in Inspector.
3. Drag it into NPC `Dialogue Json` field.

If dragging is blocked:
1. Move file to `Assets/Resources/Dialogue/YourFile.json`.
2. In NPC, set `Dialogue Json Resources Path` to `Dialogue/YourFile`.
3. Or paste JSON into `Dialogue Json Text` as a fallback.

## 5. Play mode test flow

1. Press Play.
2. Move player into NPC trigger area.
3. Press `E`.
4. Expect:
	 - Dialogue panel opens
	 - Speaker and text appear with typewriter effect
	 - Mouse click or `Space` continues dialogue
	 - Choices appear when `hasChoice` is `true`

## 6. Troubleshooting checklist

If `E` does not open dialogue:
1. Confirm `DialogueManager` exists in scene.
2. Confirm all DialogueManager UI references are assigned.
3. Confirm NPC collider is trigger-enabled.
4. Confirm player has Rigidbody2D and Collider2D.
5. Confirm NPC has a valid dialogue source (data/json/path/text).
6. Check Unity Console warnings from `NPCDialogue`.

If JSON cannot be assigned in inspector:
1. Ensure file is inside Unity `Assets`.
2. Reimport the file.
3. Remove and re-add `NPCDialogue` component.
4. Use `Dialogue Json Resources Path` as fallback.

## 7. Optional ScriptableObject workflow

You can still use `DialogueData` assets instead of JSON:
1. Create `DialogueData` asset via `Create -> Dialogue -> DialogueData`.
2. Fill nodes/lines manually.
3. Assign asset to NPC `Dialogue Data`.

Use this only if you prefer inspector-authored dialogue over JSON files.
