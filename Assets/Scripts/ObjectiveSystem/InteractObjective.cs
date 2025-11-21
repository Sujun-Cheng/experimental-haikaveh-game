using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractObjective : Objective
{
    public static InteractObjective Instance { get; private set; }

    [Header("Objective Targets")]
    public int totalCollectables = 10;
    public int totalNPCConversations = 3;

    [Header("Current Progress")]
    public int collectablesCollected = 0;
    public int npcConversationsCompleted = 0;
    public int npcsDisappeared = 0;

 
    

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ObjectiveProgress = new Dictionary<string, string>
        {
            { "Collectibles",  getCollectibleUiString() }
            ,
            { "NPCs" , getNPCUiString()}
            
        };


    }



    // Called when player collects an item
    public void CollectItem()
    {
        collectablesCollected++;
        Debug.Log($"Collected item! ({collectablesCollected}/{totalCollectables})");
        updateProgress();
        if (CheckCompleteCondition())
        {
            FinishObjective();
        }
    }

    // Called when NPC conversation starts
    public void CompleteNPCConversation()
    {
        npcConversationsCompleted++;
        updateProgress();
        Debug.Log($"Completed conversation! ({npcConversationsCompleted}/{totalNPCConversations})");
        if (CheckCompleteCondition())
        {
            FinishObjective();
        }
    }

    // Called when NPC disappears (either killed or right choice)
    public void NPCDisappeared()
    {
        npcsDisappeared++;
        updateProgress();
        Debug.Log($"NPC disappeared! ({npcsDisappeared}/{totalNPCConversations})");
        if (CheckCompleteCondition())
        {
            FinishObjective();
        }
    }



    // Optional: Get individual objective status
    public bool AreCollectablesComplete()
    {
        return collectablesCollected >= totalCollectables;
    }

    public bool AreNPCsComplete()
    {
        return npcsDisappeared >= totalNPCConversations;
    }

    public override bool CheckCompleteCondition()
    {
        return AreCollectablesComplete() && AreNPCsComplete();
    }

    private void updateProgress()
    {
        ObjectiveProgress["Collectibles"] = getCollectibleUiString();
        ObjectiveProgress["NPCs"] = getNPCUiString();
        UpdateProgress();
    }

    private string getCollectibleUiString()
    {
        return (AreCollectablesComplete() ?
                $"✓ Collect Items: {collectablesCollected}/{totalCollectables}" :
                $"Collect Items: {collectablesCollected}/{totalCollectables}");
    }

    private string getNPCUiString()
    {
        return (AreNPCsComplete() ?
                $"✓ Deal with NPCs: {npcsDisappeared}/{totalNPCConversations}" :
                $"Deal with NPCs: {npcsDisappeared}/{totalNPCConversations}");
    }
}