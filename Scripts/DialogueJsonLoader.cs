using UnityEngine;

public static class DialogueJsonLoader
{
    public static DialogueData LoadFromTextAsset(TextAsset jsonAsset)
    {
        if (jsonAsset == null)
        {
            Debug.LogWarning("DialogueJsonLoader received a null TextAsset.");
            return null;
        }

        return LoadFromJson(jsonAsset.text, jsonAsset.name);
    }

    public static DialogueData LoadFromJson(string json, string sourceName = "JSON")
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning($"DialogueJsonLoader received empty JSON from {sourceName}.");
            return null;
        }

        DialogueData runtimeData = ScriptableObject.CreateInstance<DialogueData>();
        runtimeData.hideFlags = HideFlags.DontSave;

        try
        {
            JsonUtility.FromJsonOverwrite(json, runtimeData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse dialogue JSON from {sourceName}: {ex.Message}");
            Object.Destroy(runtimeData);
            return null;
        }

        if (runtimeData.nodes == null || runtimeData.nodes.Count == 0)
        {
            Debug.LogError($"Dialogue JSON from {sourceName} does not contain any nodes.");
            Object.Destroy(runtimeData);
            return null;
        }

        if (string.IsNullOrWhiteSpace(runtimeData.startNodeId))
        {
            runtimeData.startNodeId = runtimeData.nodes[0].nodeId;
        }

        return runtimeData;
    }
}
