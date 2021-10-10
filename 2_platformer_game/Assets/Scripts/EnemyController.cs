using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Hero {
    public float aiGroundDetectDepth = 0.2f;
    public bool showAiGroundDetect = false;
    public LayerMask aiGroundMask;

    public bool printState = false;

    public float aiWallDetectDepth = 0.2f;
    public float aiWallDetecHeight = 0.1f;
    public bool showAiWallDetect = false;
    public float aiFireDetectLength = 10f;
    public bool showAiFireballDetect = false;

    public float aiEnemyDetectLength = 5f;
    public bool showAiEnemyDetect = false;
    public LayerMask aiEnemyDetectMask;

    private StateMachine stateMachine;
    private StatePatrol statePatrol;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        stateMachine = new StateMachine();

        statePatrol = new StatePatrol(stateMachine, this);
        stateMachine.Init(statePatrol, printState);
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
        // fireball check
        if (showAiFireballDetect && boxCollider != null) {
            if (!isFacingRight()) {
                Vector3 pos3 = boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0, 0);
                Gizmos.DrawLine(pos3, pos3 + new Vector3(-aiFireDetectLength, 0, 0));
            } else {
                Vector3 pos3 = boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0, 0);
                Gizmos.DrawLine(pos3, pos3 + new Vector3(aiFireDetectLength, 0, 0));
            }
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

    public bool AIFireballTest(GameObject t) {
        RaycastHit2D[] detect;
        if (!isFacingRight()) {
            detect = Physics2D.RaycastAll(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0, 0), Vector2.left, aiFireDetectLength, aiEnemyDetectMask);
        } else {
            detect = Physics2D.RaycastAll(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0, 0), Vector2.right, aiFireDetectLength, aiEnemyDetectMask);
        }
        foreach (RaycastHit2D hit in detect) {
            if (hit.collider.gameObject == t) return true;
        }
        return false;
    }
}

public class StateFight : StateBase {
    private StateMachine stateMachine;
    private EnemyController enemyController;
    private GameObject target;
    private int rangeAttackPreference;

    private BoxCollider2D boxCollider;


    public StateFight(StateMachine machine, EnemyController controller, GameObject target) {
        stateMachine = machine;
        enemyController = controller;
        boxCollider = enemyController.GetComponent<BoxCollider2D>();
        this.target = target;

        rangeAttackPreference = Random.Range(97, 99);
        rangeAttackPreference = 97;
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
            // Within attack range
            enemyController.Attack();
        } else if ((target.GetComponent<Collider2D>().bounds.min.y > boxCollider.bounds.max.y || target.GetComponent<Collider2D>().bounds.max.y < boxCollider.bounds.min.y) && Mathf.Abs(target.transform.position.x - enemyController.gameObject.transform.position.x) < boxCollider.bounds.size.x) {
            // target too high or too low
            enemyController.Stop();
        } else {
            if (enemyController.AIFireballTest(target)) {
                if (Random.Range(0, 100) > rangeAttackPreference) {
                    enemyController.Fire();
                    return;
                }
            }

            float distanceX = target.transform.position.x - enemyController.gameObject.transform.position.x;
            float width = enemyController.GetComponent<Collider2D>().bounds.size.x;
            // facing wrong direction
            if (!enemyController.isFacingRight() && distanceX > 0) {
                enemyController.MoveRight();
                return;
            }
            if (enemyController.isFacingRight() && distanceX < 0) {
                enemyController.MoveLeft();
                return;
            }
            // faceing correct
            if (!enemyController.AIGroundDetect(enemyController.isFacingRight()) || enemyController.AIWallDetect(enemyController.isFacingRight()) || Mathf.Abs(distanceX) < width) {
                enemyController.Stop();
                // enemyController.Fire();
                return;
            }
            if (distanceX > 0) {
                enemyController.MoveRight();
            } else {
                enemyController.MoveLeft();
            }
        }
    }

}

public class StatePatrol : StateBase {
    private StateMachine stateMachine;
    private EnemyController enemyController;

    private BoxCollider2D boxCollider;

    public StatePatrol(StateMachine machine, EnemyController controller) {
        stateMachine = machine;
        enemyController = controller;
        boxCollider = enemyController.GetComponent<BoxCollider2D>();
    }

    public override void OnEnter() {
        enemyController.MoveRight();
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

        if (!enemyController.AIGroundDetect(enemyController.isFacingRight()) || enemyController.AIWallDetect(enemyController.isFacingRight())) {
            if (enemyController.isFacingRight()) enemyController.MoveLeft();
            else enemyController.MoveRight();
        }
    }

    public override void OnDrawGizmos() {
        // enemy detect
        if (enemyController.showAiEnemyDetect && boxCollider != null) {
            if (enemyController.isFacingRight()) {
                Gizmos.DrawLine(boxCollider.bounds.center, new Vector2(boxCollider.bounds.center.x + enemyController.aiEnemyDetectLength, boxCollider.bounds.center.y));
            } else {
                Gizmos.DrawLine(boxCollider.bounds.center, new Vector2(boxCollider.bounds.center.x - enemyController.aiEnemyDetectLength, boxCollider.bounds.center.y));
            }
        }
    }

    public GameObject AIEnemyDetect() {
        RaycastHit2D[] detect;
        if (enemyController.isFacingRight()) {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.max.x + 0.1f, boxCollider.bounds.center.y), Vector2.right, enemyController.aiEnemyDetectLength, enemyController.aiEnemyDetectMask);
        } else {
            detect = Physics2D.RaycastAll(new Vector2(boxCollider.bounds.min.x - 0.1f, boxCollider.bounds.center.y), Vector2.left, enemyController.aiEnemyDetectLength, enemyController.aiEnemyDetectMask);
        }

        foreach (RaycastHit2D target in detect) {
            Hero h = target.collider.GetComponent<Hero>();
            if (h != null && h.Group() != enemyController.Group() && h.health > 0) {
                return target.collider.gameObject;
            }
        }
        return null;
    }

}
