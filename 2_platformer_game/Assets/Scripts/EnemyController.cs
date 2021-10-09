using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private BoxCollider2D boxCollider;
    private Rigidbody2D body;
    private Hero hero;

    private StateMachine stateMachine;
    private StatePatrol statePatrol;


    // Start is called before the first frame update
    void Start() {
        hero = GetComponent<Hero>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        stateMachine = new StateMachine();

        statePatrol = new StatePatrol();
        statePatrol.heroController = hero;
        statePatrol.enemyController = this;
        stateMachine.Init(statePatrol);
    }

    // Update is called once per frame
    void Update() {
        if (stateMachine != null) {
            stateMachine.Update();
        }
    }
}

public class StatePatrol : StateBase {
    public EnemyController enemyController {get; set;}
    public Hero heroController {get; set;}

    private bool movingRight = false;

    public override void OnEnter() {
        movingRight = false;
        heroController.Move(movingRight);
    }

    public override void OnExit() {
        heroController.Stop();
    }

    public override void Update() {
        if (enemyController == null || heroController == null) {
            return;
        }
        
        if (!heroController.AIGroundDetect()) {
            movingRight = !movingRight;
            heroController.Move(movingRight);
        }
    }
}
