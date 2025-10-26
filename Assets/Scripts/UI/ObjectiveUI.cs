using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI collectablesText;
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI statusText;

    [Header("Optional: Checkmark Images")]
    public Image collectablesCheckmark;
    public Image npcCheckmark;

    public void UpdateObjectives(int collected, int totalCollectables, int npcsGone, int totalNPCs)
    {
        // Update collectables text
        if (collectablesText != null)
        {
            bool collectablesDone = collected >= totalCollectables;
            collectablesText.text = collectablesDone ?
                $"✓ Collect Items: {collected}/{totalCollectables}" :
                $"Collect Items: {collected}/{totalCollectables}";

            // Change color if complete
            collectablesText.color = collectablesDone ? Color.green : Color.white;
        }

        // Update NPC text
        if (npcText != null)
        {
            bool npcsDone = npcsGone >= totalNPCs;
            npcText.text = npcsDone ?
                $"✓ Deal with NPCs: {npcsGone}/{totalNPCs}" :
                $"Deal with NPCs: {npcsGone}/{totalNPCs}";

            // Change color if complete
            npcText.color = npcsDone ? Color.green : Color.white;
        }

        // Update overall status
        if (statusText != null)
        {
            bool allDone = collected >= totalCollectables && npcsGone >= totalNPCs;
            statusText.text = allDone ? "ALL OBJECTIVES COMPLETE!" : "Objectives In Progress...";
            statusText.color = allDone ? Color.yellow : Color.white;
        }

        // Update checkmarks if using images
        if (collectablesCheckmark != null)
        {
            collectablesCheckmark.enabled = collected >= totalCollectables;
        }

        if (npcCheckmark != null)
        {
            npcCheckmark.enabled = npcsGone >= totalNPCs;
        }
    }
}