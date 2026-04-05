# Dialogue Content Pack

These files are structured dialogue drafts based on `DialogueData`.

Use them as the source text when creating Unity `DialogueData` ScriptableObjects.

Structure:
- `startNodeId`
- `nodes`
- each node has `nodeId`
- each node has `lines`
- each line has `speakerName`, `text`, `hasChoice`, and `choices`
- each choice has `choiceText`, `nextNodeId`, and `humanityModifier`

Recommended Unity setup:
- Create one `DialogueData` asset per file.
- Copy the JSON content into matching ScriptableObject fields manually.
- Assign the asset to the relevant `NPCDialogue` component.
