using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    virtual public void OnEnter() {}
    virtual public void Update() {}
    virtual public void OnDrawGizmos() {}
    virtual public void OnExit() {}
}
