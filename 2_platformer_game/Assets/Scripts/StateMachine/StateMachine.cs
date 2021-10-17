using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private StateBase currentState;
    private bool printState = false;
    
    public void Init(StateBase state, bool debug) {
        currentState = state;
        printState = debug;
        currentState.OnEnter();
    }

    public void SwitchState(StateBase state) {
        if (currentState != null) currentState.OnExit();
        currentState = state;
        currentState.OnEnter();
        if (printState) {
            Debug.Log("StateMachine enter: " + currentState.GetType().Name);
        }
    }

    public void Update() {
        if (currentState != null) {
            currentState.Update();
        }
    }

    public void OnDrawGizmos() {
        if (currentState != null) {
            currentState.OnDrawGizmos();
        }
    }
}
