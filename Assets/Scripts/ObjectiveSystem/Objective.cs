using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    public string description;
    public Dictionary<string, string> ObjectiveProgress;
    public ObjectiveState State;
    
    public enum ObjectiveState
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETED
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected void FinishObjective()
    {
        if (State != ObjectiveState.COMPLETED)
        {
            Debug.Log($"Finishing objective {description}");
            State = ObjectiveState.COMPLETED;
            EventManager.TriggerEvent<ObjectiveCompleteEvent, Objective>(this);
            gameObject.SetActive(false);
        }

    }

    protected void UpdateProgress()
    {
        
        EventManager.TriggerEvent<ObjectiveUpdateEvent, Objective>(this);
            

    }

    public abstract bool CheckCompleteCondition();

  
}
