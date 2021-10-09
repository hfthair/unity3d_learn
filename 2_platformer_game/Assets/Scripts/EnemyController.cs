using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Hero {
    public float aiGroundDetectDepth = 0.1f;
    public bool showAiGroundDetect = false;
    public LayerMask aiGroundMask;
    public float aiEnemyDetectLength = 5f;
    public bool showAiEnemyDetect = false;
    public LayerMask aiEnemyDetectMask;

    private StateMachine stateMachine;
    private StatePatrol statePatrol;
    private BoxCollider2D boxCollider;


    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        boxCollider = GetComponent<BoxCollider2D>();

        stateMachine = new StateMachine();

        statePatrol = new StatePatrol(this);
        stateMachine.Init(statePatrol);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        if (stateMachine != null) {
            stateMachine.Update();
        }
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (stateMachine != null) {
            stateMachine.OnDrawGizmos();
        }
    }
}

public class StatePatrol : StateBase {
    private EnemyController enemyController;

    private BoxCollider2D boxCollider;
    private bool movingRight = false;

    public StatePatrol(EnemyController controller) {
        enemyController = controller;
        boxCollider = enemyController.GetComponent<BoxCollider2D>();
    }

    public override void OnEnter() {
        movingRight = false;
        enemyController.Move(movingRight);
    }

    public override void OnExit() {
        enemyController.Stop();
    }

    public override void Update() {
        if (enemyController == null) {
            return;
        }

        if (!AIGroundDetect()) {
            movingRight = !movingRight;
            enemyController.Move(movingRight);
        }
    }
    
    public override void OnDrawGizmos() {
        if (enemyController.showAiGroundDetect && boxCollider != null) {
            Vector2 pos = GetAIGroundDetectPos();
            Gizmos.DrawLine(pos, new Vector2(pos.x, pos.y - enemyController.aiGroundDetectDepth));
        }
        if (enemyController.showAiEnemyDetect && boxCollider != null) {
            Gizmos.DrawLine(boxCollider.bounds.center, new Vector2(boxCollider.bounds.center.x + enemyController.aiEnemyDetectLength, boxCollider.bounds.center.y));
        }
    }

    // --------------- AI only functions -------------------------------------

    public bool AIGroundDetect() {
        Vector2 pos = GetAIGroundDetectPos();
        RaycastHit2D[] detect = Physics2D.RaycastAll(pos, Vector2.down, enemyController.aiGroundDetectDepth, enemyController.aiGroundMask);
        return detect.Length > 0;
    }

    private Vector2 GetAIGroundDetectPos() {
        if (movingRight) {
            return new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        } else {
            return boxCollider.bounds.min;
        }
    }

    public bool AIEnemyDetect(Vector2 direction) {
        RaycastHit2D[] detect = Physics2D.RaycastAll(boxCollider.bounds.center, direction, enemyController.aiEnemyDetectLength, enemyController.aiEnemyDetectMask);
        return detect.Length > 0;
    }

}
