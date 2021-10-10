using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Hero {
    public float aiGroundDetectDepth = 0.2f;
    public bool showAiGroundDetect = false;
    public LayerMask aiGroundMask;


    public float aiWallDetectDepth = 0.2f;
    public float aiWallDetecHeight = 0.1f;
    public bool showAiWallDetect = false;

    public float aiEnemyDetectLength = 5f;
    public bool showAiEnemyDetect = false;
    public LayerMask aiEnemyDetectMask;

    private StateMachine stateMachine;
    private StatePatrol statePatrol;
    private BoxCollider2D boxCollider;
    private Rigidbody2D body;


    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();

        statePatrol = new StatePatrol(stateMachine, this);
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
        
        // ground check
        if (showAiGroundDetect && boxCollider != null) {
            Vector2 pos = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
            Gizmos.DrawLine(pos, new Vector2(pos.x, pos.y - aiGroundDetectDepth));
            pos = boxCollider.bounds.min;
            Gizmos.DrawLine(pos, new Vector2(pos.x, pos.y - aiGroundDetectDepth));
        }
        // wall check
        if (showAiWallDetect && boxCollider != null) {
            Vector2 pos = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y + aiWallDetecHeight);
            Gizmos.DrawLine(pos, new Vector2(pos.x + aiWallDetectDepth, pos.y));
            pos = boxCollider.bounds.min + new Vector3(0, aiWallDetecHeight, 0);
            Gizmos.DrawLine(pos, new Vector2(pos.x - aiWallDetectDepth, pos.y));
        }
    }

    public bool AIGroundDetect(bool right) {
        RaycastHit2D[] detect;
        if (right) {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y), Vector2.down, aiGroundDetectDepth, aiGroundMask);
        } else {
            detect = Physics2D.RaycastAll(boxCollider.bounds.min, Vector2.down, aiGroundDetectDepth, aiGroundMask);
        }
        return detect.Length > 0;
    }

    public bool AIWallDetect(bool right) {
        RaycastHit2D[] detect;
        if (right) {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y + aiWallDetecHeight), Vector2.right, aiWallDetectDepth, aiGroundMask);
        } else {
            detect = Physics2D.RaycastAll(boxCollider.bounds.min + new Vector3(0, +aiWallDetecHeight, 0), Vector2.left, aiWallDetectDepth, aiGroundMask);
        }
        return detect.Length > 0;
    }
}

public class StateFight : StateBase {
    private StateMachine stateMachine;
    private EnemyController enemyController;
    private GameObject target;

    private BoxCollider2D boxCollider;

    public StateFight(StateMachine machine, EnemyController controller, GameObject target) {
        stateMachine = machine;
        enemyController = controller;
        boxCollider = enemyController.GetComponent<BoxCollider2D>();
        this.target = target;
    }

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void Update() {
        if (target.GetComponent<Hero>().health <= 0) {
            stateMachine.SwitchState(new StatePatrol(stateMachine, enemyController));
            return;
        }
        if (enemyController.AttackHitBoxTest(target)) {
            enemyController.Attack();
        } else if ((target.GetComponent<Collider2D>().bounds.min.y > boxCollider.bounds.max.y || target.GetComponent<Collider2D>().bounds.max.y < boxCollider.bounds.min.y) && Mathf.Abs(target.transform.position.x - enemyController.gameObject.transform.position.x) < boxCollider.bounds.size.x) {
            enemyController.Stop();
        } else {
            if (target.transform.position.x - enemyController.gameObject.transform.position.x > 0) {
                if (enemyController.AIGroundDetect(true) && !enemyController.AIWallDetect(true)) {
                    enemyController.Move(true);
                } else {
                    enemyController.Stop();
                }
            } else {
                if (enemyController.AIGroundDetect(false) && !enemyController.AIWallDetect(false)) {
                    enemyController.Move(false);
                } else {
                    enemyController.Stop();
                }
            }
        }
    }

}

public class StatePatrol : StateBase {
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private BoxCollider2D boxCollider;
    private bool movingRight = false;

    public StatePatrol(StateMachine machine, EnemyController controller) {
        stateMachine = machine;
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

        // Being attacked

        GameObject target = AIEnemyDetect();
        if (target != null) {
            stateMachine.SwitchState(new StateFight(stateMachine, enemyController, target));
            return;
        }

        if (!enemyController.AIGroundDetect(movingRight) || enemyController.AIWallDetect(movingRight)) {
            movingRight = !movingRight;
            enemyController.Move(movingRight);
        }
    }

    public override void OnDrawGizmos() {
        // enemy detect
        if (enemyController.showAiEnemyDetect && boxCollider != null) {
            if (movingRight) {
                Gizmos.DrawLine(boxCollider.bounds.center, new Vector2(boxCollider.bounds.center.x + enemyController.aiEnemyDetectLength, boxCollider.bounds.center.y));
            } else {
                Gizmos.DrawLine(boxCollider.bounds.center, new Vector2(boxCollider.bounds.center.x - enemyController.aiEnemyDetectLength, boxCollider.bounds.center.y));
            }
        }
    }

    public GameObject AIEnemyDetect() {
        RaycastHit2D[] detect;
        if (movingRight) {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.max.x + 0.1f, boxCollider.bounds.center.y), Vector2.right, enemyController.aiEnemyDetectLength, enemyController.aiEnemyDetectMask);
        } else {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.min.x - 0.1f, boxCollider.bounds.center.y), Vector2.left, enemyController.aiEnemyDetectLength, enemyController.aiEnemyDetectMask);
        }

        foreach (RaycastHit2D target in detect) {
            if (target.collider.gameObject.GetComponent<Hero>() != null && target.collider.gameObject.GetComponent<Hero>().group != enemyController.group) {
                return target.collider.gameObject;
            }
        }
        return null;
    }

}
