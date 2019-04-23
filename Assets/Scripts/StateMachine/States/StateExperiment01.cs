using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateExperiment : IState
{

    StateMachineLogic callback;


    public StateExperiment(StateMachineLogic callback) { this.callback = callback; }


    public void Enter()
    {
        Debug.Log("entering test state (StateExperiment)");
    }


    public void Execute()
    {
        Debug.Log("updating test state (StateExperiment)");
        if (Time.time > 15f)
        {
            callback.Experiment01Completed();
        }
    }


    public void Exit()
    {
        Debug.Log("exiting test state (StateExperiment)");
    }

}
