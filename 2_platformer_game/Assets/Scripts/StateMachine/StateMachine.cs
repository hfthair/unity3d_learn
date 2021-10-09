using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private StateBase currentState;

    public void Init(StateBase state) {
        currentState = state;
        currentState.OnEnter();
    }

    public void SwitchState(StateBase state) {
        if (currentState != null) currentState.OnExit();
        currentState = state;
        currentState.OnEnter();
    }

    public void Update() {
        // Stop if too far from the cam
        if (currentState != null) {
            currentState.Update();
        }
    }
}
