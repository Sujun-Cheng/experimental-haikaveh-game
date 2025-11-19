using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    public UnityEvent PrerequisiteStartCondition;
    public UnityEvent CompleteCondition;
    public ObjectiveState State;
    public enum ObjectiveState
    {
        NOT_STARTED,
        STARTED,
        IN_PROGRESS,
        COMPLETED
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void FinishObjective()
    {
        if (State != ObjectiveState.COMPLETED)
        {
            State = ObjectiveState.COMPLETED;
            EventManager.TriggerEvent<ObjectiveCompleteEvent, GameObject>(gameObject);
            Destroy(this.gameObject);
        }

    }

  
}
