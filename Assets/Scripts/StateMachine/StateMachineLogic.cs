using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineLogic : MonoBehaviour
{

    StateMachine stateMachine = new StateMachine();


    //
    void Start()
    {
        stateMachine.ChangeState(new StateCalibration(this));
    }


    //
    void Update()
    {
        stateMachine.Update();
    }


    //
    public void CalibrationCompleted()
    {
        stateMachine.ChangeState(new StatePlay(this));
    }


    //
    public void PlayCompleted()
    {
        stateMachine.ChangeState(new StateExperiment(this));
    }


    //
    public void Experiment01Completed()
    {
        Debug.Log("Experiment01Completed");
    }


}
