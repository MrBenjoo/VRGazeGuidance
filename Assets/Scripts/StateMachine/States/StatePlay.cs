using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlay : IState
{

    StateMachineLogic callback;


    public StatePlay(StateMachineLogic callback) { this.callback = callback; }


    public void Enter()
    {
        Debug.Log("entering test state (StatePlay)");
    }


    public void Execute()
    {
        Debug.Log("updating test state (StatePlay)");
        if (Time.time > 10f)
        {
            callback.PlayCompleted();
        }
    }


    public void Exit()
    {
        Debug.Log("exiting test state (StatePlay)");
    }


}
