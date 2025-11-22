using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI objectiveDescription;
    public TextMeshProUGUI objectiveProgress;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI objectiveFlavorText;

    [Header("Optional: Checkmark Images")]
    public Image collectablesCheckmark;
    public Image npcCheckmark;
    public void UpdateObjectives(int collected, int totalCollectables, int npcsGone, int totalNPCs)
    {

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

    public void UpdateObjectives(Objective[] objectives, int step)
    {
        if (step < objectives.Length)
        {
            Objective obj = objectives[step];
            string objDescription = obj.description;
            Dictionary<string, string> map = obj.ObjectiveProgress;
            if (objectiveDescription != null)
            {
                objectiveDescription.text = objDescription;
                objectiveDescription.color = Color.white;
            }
           
            if (map != null && map.Count > 0)
            {
                if (objectiveProgress != null)
                {
                    string text = string.Empty;
                    Debug.Log($"map: {map}");
                    foreach (KeyValuePair<string,string> keyValuePair in map.OrderBy(i => i.Key))
                    {
                        text += keyValuePair.Value;
                        text += "\n";
                    }
                    objectiveProgress.text = text;
                    objectiveProgress.color = Color.white;
                }

            } else
            {
                objectiveProgress.text = string.Empty;
            }
            statusText.text =  "Objectives In Progress...";
            statusText.color =  Color.white;

        } else
        {
            Debug.Log("All objectives complete, game won");
            objectiveDescription.text = string.Empty;
            objectiveProgress.text = string.Empty;
            statusText.text = "ALL OBJECTIVES COMPLETE!";
            statusText.color = Color.yellow ;
        }
    }

    public void UpdateFlavorText(string text, Color color)
    {
        objectiveFlavorText.text = text;
        objectiveFlavorText.color = color;
    }
}