using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCalibration : IState
{

    StateMachineLogic callback;


    public StateCalibration(StateMachineLogic callback) { this.callback = callback; }


    public void Enter()
    {
        Debug.Log("entering test state (StateCalibration)");
    }


    public void Execute()
    {
        Debug.Log("updating test state (StateCalibration)");
        if (Time.time > 5f)
        {
            callback.CalibrationCompleted();
        }
    }


    public void Exit()
    {
        Debug.Log("exiting test state (StateCalibration) ");
    }

}

