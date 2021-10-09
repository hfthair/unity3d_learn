using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    public float walkSpeed = 10.0f;
    public float jumpSpeed = 22.0f;
    public float jumpModifier = 0.5f;

    public int healthMax = 100;
    private int health;

    public Transform attackPos;
    public Vector2 attackSize;
    public LayerMask attackLayer;
    public float attackInterval = 1f;
    public int attackPower = 20;

    public bool showAttackArea = false;

    private float lastAttackTime = 0f;

    public float knockBackForce = 2f;
    public float knockBackDuration = 0.5f;
    private float knockBackTime = 0f;

    public float aiGroundDetectDepth = 0.1f;
    public bool showAiGroundDetect = false;
    public LayerMask aiGroundMask;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator anim;

    private Vector2 move = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        health = healthMax;
    }

    // Update is called once per frame
    void Update() {
        bool ground = IsGrounded();
        if (anim.GetBool("ground") != ground) {
            anim.SetBool("ground", ground);
        }

        if (move.x < 0) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        } else if (move.x > 0) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void FixedUpdate() {
        if (health <= 0) {
            if (Time.time - knockBackTime >= knockBackDuration) {
                body.velocity = new Vector2(0, body.velocity.y);
            }
            return;
        }
        if (Time.time - knockBackTime < knockBackDuration) {
            // Can not move when knocked or use force when knocked?
        } else {
            body.velocity = new Vector2(move.x, body.velocity.y);
            if (move.y > 0) {
                body.velocity = new Vector2(body.velocity.x, move.y);
                move.y = 0;
            }
        }
    }

    public void Move(bool right) {
        if (right) move.x = walkSpeed;
        else move.x = -walkSpeed;
        anim.SetFloat("speed", Mathf.Abs(move.x));
    }

    public void Stop() {
        move.x = 0f;
        anim.SetFloat("speed", 0);
    }

    public void Jump() {
        if (IsGrounded()) {
            move.y = jumpSpeed;
        }
    }

    public void SlowdownJump() {
        if (body.velocity.y > 0) move.y = body.velocity.y * jumpModifier;
    }

    public void TakeDamage(int[] damage) {
        health = health - damage[0];
        if (health > 0) {
            anim.SetTrigger("hurt");
            Debug.Log("Taking damage: " + damage + " Current health: " + health);
        } else {
            Die();
        }
        KnockBack(damage[1] > 0);
    }

    private void KnockBack(bool toRight) {
        knockBackTime = Time.time;
        body.velocity = new Vector2(0, body.velocity.y);
        if (toRight) {
            body.AddForce(new Vector2(knockBackForce, 0), ForceMode2D.Impulse);
        } else {
            body.AddForce(new Vector2(-knockBackForce, 0), ForceMode2D.Impulse);
        }
    }

    public void Die() {
        anim.SetTrigger("die");
        // Disable control (maybe seperate the move/input to another script)
    }

    public void Attack() {
        if (Time.time - lastAttackTime > attackInterval) {
            anim.SetTrigger("attack");
            lastAttackTime = Time.time;
        }
    }

    private void AttackHitBoxCheck() {
        Collider2D[] detected = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0, attackLayer);
        foreach (Collider2D target in detected) {
            Debug.Log("hited: " + target.gameObject.name);
            int[] attackMessage = new int[2];
            attackMessage[0] = attackPower;
            attackMessage[1] = 1;
            if (transform.eulerAngles.y > 1) {
                attackMessage[1] = -1;
            }
            target.transform.SendMessage("TakeDamage", attackMessage);
        }
    }

    private bool IsGrounded() {
        RaycastHit2D[] results = new RaycastHit2D[16];
        int count = body.Cast(Vector2.down, results, 0.1f);

        return count > 0;
    }

    // --------------- AI only functions -------------------------------------
    
    public bool AIGroundDetect() {
        Vector2 pos = GetAIGroundDetectPos();
        RaycastHit2D[] detect = Physics2D.RaycastAll(pos, Vector2.down, aiGroundDetectDepth, aiGroundMask);
        return detect.Length > 0;
    }

    private Vector2 GetAIGroundDetectPos() {
        if (body.velocity.x > 0) {
            return new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        } else if (body.velocity.x < 0) {
            return boxCollider.bounds.min;
        } else {
            return new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        }
    }

    private void OnDrawGizmos() {
        if (showAttackArea)
            Gizmos.DrawCube(attackPos.position, attackSize);
        if (showAiGroundDetect && boxCollider != null) {
            Vector2 pos = GetAIGroundDetectPos();
            Gizmos.DrawLine(pos, new Vector2(pos.x, pos.y - aiGroundDetectDepth));
        }
    }

}
