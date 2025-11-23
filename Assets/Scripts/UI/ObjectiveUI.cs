using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI objectiveDescription;
    public TextMeshProUGUI objectiveProgress;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI objectiveFlavorText;
    public float textAppearanceDelay;

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

    public void UpdateFlavorText(ObjectiveTextLine[] objectiveTextLines)
    {
        {
            StopAllCoroutines();
            StartCoroutine(PrintDialogue(objectiveTextLines));
        }
    }
    private IEnumerator PrintDialogue(ObjectiveTextLine[] objectiveTextLines)
    {
        int currentDialogueIndex = 0;
        try
        {
            while (currentDialogueIndex < objectiveTextLines.Count())
            {
                ObjectiveTextLine line = objectiveTextLines[currentDialogueIndex];
                yield return StartCoroutine(TypeText(line));
                                  

                currentDialogueIndex++;
            }
        }
        finally
        {
            objectiveFlavorText.text = string.Empty;
        }
    }

    private IEnumerator TypeText(ObjectiveTextLine text)
    {
        objectiveFlavorText.text = "";
        objectiveFlavorText.color = text.color;
        string fullText = text.text;

        for (int i = 0; i < fullText.Length; i++)
        {
            objectiveFlavorText.text += fullText[i];

            yield return new WaitForSeconds(textAppearanceDelay);
        }
        yield return new WaitForSeconds(0.5f);

    }
}