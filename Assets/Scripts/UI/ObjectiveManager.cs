using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Objective Targets")]
    public int totalCollectables = 10;
    public int totalNPCConversations = 3;

    [Header("Current Progress")]
    public int collectablesCollected = 0;
    public int npcConversationsCompleted = 0;
    public int npcsDisappeared = 0;

    [Header("UI Reference")]
    public ObjectiveUI objectiveUI;
    public GameObject winTextObject;

    [Header("Win Condition")]
    public UnityEvent onAllObjectivesComplete;

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
    }

    private void Start()
    {
        UpdateUI();
        winTextObject.SetActive(false); 
    }

    // Called when player collects an item
    public void CollectItem()
    {
        collectablesCollected++;
        Debug.Log($"Collected item! ({collectablesCollected}/{totalCollectables})");
        UpdateUI();
        CheckWinCondition();
    }

    // Called when NPC conversation starts
    public void CompleteNPCConversation()
    {
        npcConversationsCompleted++;
        Debug.Log($"Completed conversation! ({npcConversationsCompleted}/{totalNPCConversations})");
        UpdateUI();
        CheckWinCondition();
    }

    // Called when NPC disappears (either killed or right choice)
    public void NPCDisappeared()
    {
        npcsDisappeared++;
        Debug.Log($"NPC disappeared! ({npcsDisappeared}/{totalNPCConversations})");
        UpdateUI();
        CheckWinCondition();
    }

    private void UpdateUI()
    {
        if (objectiveUI != null)
        {
            objectiveUI.UpdateObjectives(
                collectablesCollected,
                totalCollectables,
                npcsDisappeared,
                totalNPCConversations
            );
        }
    }

    private void CheckWinCondition()
    {
        if (collectablesCollected >= totalCollectables &&
            npcsDisappeared >= totalNPCConversations)
        {
            Debug.Log("ALL OBJECTIVES COMPLETE! YOU WIN!"); // need to add game win UI later
            winTextObject.SetActive(true);
            onAllObjectivesComplete?.Invoke();
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

    public bool AreAllObjectivesComplete()
    {
        return AreCollectablesComplete() && AreNPCsComplete();
    }
}