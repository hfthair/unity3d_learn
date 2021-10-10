using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    public float walkSpeed = 10.0f;
    public float jumpSpeed = 22.0f;
    public float jumpModifier = 0.5f;

    public int healthMax = 100;
    public int health {get; set;}

    public Transform attackPos;
    public Transform firePos;
    public Vector2 attackSize;
    public LayerMask attackLayer;
    public float attackInterval = 1f;
    public int attackPower = 20;

    public GameObject fireball;
    public float fireballSpeed = 10f;
    public int fireballDamage = 10;

    public bool showAttackArea = false;

    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    public float knockBackForce = 2f;
    public float knockBackDuration = 0.5f;
    private float knockBackTime = 0f;

    protected Rigidbody2D body;
    protected Animator anim;
    protected BoxCollider2D boxCollider;

    private Vector2 move = new Vector2(0, 0);

    // Start is called before the first frame update
    protected virtual void Start() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        health = healthMax;
    }

    // Update is called once per frame
    protected virtual void Update() {
        bool ground = IsGrounded();
        if (health > 0 && anim.GetBool("ground") != ground) {
            anim.SetBool("ground", ground);
        }

        if (move.x < 0) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        } else if (move.x > 0) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    protected virtual void FixedUpdate() {
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

    protected virtual void OnDrawGizmos() {
        if (showAttackArea)
            Gizmos.DrawCube(attackPos.position, attackSize);
    }

    // --------------------------- controls ---------------------------------------
    public void MoveLeft() {
        if (health <= 0) return;
        if (isAttacking) return;
        move.x = -walkSpeed;
        anim.SetFloat("speed", Mathf.Abs(move.x));
    }

    public void MoveRight() {
        if (health <= 0) return;
        if (isAttacking) return;
        move.x = walkSpeed;
        anim.SetFloat("speed", Mathf.Abs(move.x));
    }

    public void Stop() {
        if (health <= 0) return;
        move.x = 0f;
        anim.SetFloat("speed", 0);
    }

    public void Jump() {
        if (health <= 0) return;
        if (isAttacking) return;
        if (IsGrounded()) {
            move.y = jumpSpeed;
        }
    }

    public void SlowdownJump() {
        if (health <= 0) return;
        if (body.velocity.y > 0) move.y = body.velocity.y * jumpModifier;
    }

    public void TakeDamage(int[] damage) {
        if (health <= 0) return;
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
        isAttacking = false;
        knockBackTime = Time.time;
        if (toRight) {
            body.velocity = new Vector2(knockBackForce, body.velocity.y);
        } else {
            body.velocity = new Vector2(-knockBackForce, body.velocity.y);
        }
    }

    public void Attack() {
        if (health <= 0) return;
        if (Time.time - lastAttackTime > attackInterval) {
            Stop();
            anim.SetTrigger("attack");
            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    public void Fire() {
        if (health <=0) return;
        if (Time.time - lastAttackTime > attackInterval && firePos != null) {
            Stop();
            anim.SetTrigger("fire");

            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    public void Die() {
        health = 0;
        anim.SetFloat("speed", 0);
        anim.SetTrigger("die");
    }

    // --------------------------- controls ends ----------------------------------

    public bool isFacingRight() {
        return transform.eulerAngles.y <= 1f;
    }

    public GameObject Group() {
        return transform.parent.gameObject;
    }

    public void OnAttackAnimEnd() {
        isAttacking = false;
    }

    private void FireballShoot() {
        GameObject ball = Instantiate(fireball, firePos.position, Quaternion.identity);
        Collider2D[] colliders = transform.parent.GetComponentsInChildren<Collider2D>();
        foreach(Collider2D collider in colliders) {
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), collider);
        }

        if (!isFacingRight()) {
            ball.GetComponent<FireballController>().Fire(Vector2.left, this);
        }
        else ball.GetComponent<FireballController>().Fire(Vector2.right, this);
    }

    private void AttackHitBoxCheck() {
        Collider2D[] detected = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0, attackLayer);
        foreach (Collider2D target in detected) {
            if (target.GetComponent<Hero>() == null || target.GetComponent<Hero>().Group() == Group()) {
                continue;
            }
            int[] attackMessage = new int[2];
            attackMessage[0] = attackPower;
            attackMessage[1] = 1;
            if (transform.eulerAngles.y > 1) {
                attackMessage[1] = -1;
            }
            target.SendMessage("TakeDamage", attackMessage);
        }
    }

    public bool AttackHitBoxTest(GameObject obj) {
        Collider2D[] detected = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0, attackLayer);
        foreach (Collider2D target in detected) {
            if (target.gameObject == obj) {
                return true;
            }
        }
        return false;
    }

    private bool IsGrounded() {
        RaycastHit2D[] results = new RaycastHit2D[16];
        int count = body.Cast(Vector2.down, results, 0.1f);

        return count > 0;
    }


}
