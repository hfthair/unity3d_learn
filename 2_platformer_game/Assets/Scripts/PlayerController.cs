using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public bool inputEnable = true;
    public float walkSpeed = 3.0f;
    public float jumpSpeed = 3.0f;
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

    private Rigidbody2D body;
    private Animator anim;

    private Vector2 move = new Vector2(0, 0);


    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        health = healthMax;
    }

    // Update is called once per frame
    void Update() {
        bool ground = IsGrounded();
        if (anim.GetBool("ground") != ground) {
            anim.SetBool("ground", ground);
        }

        if (inputEnable) {
            CheckInput(ground);
        }

        if (move.x < 0) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        } else if (move.x > 0) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    void CheckInput(bool ground) {
        // move
        move.x = Input.GetAxisRaw("Horizontal") * walkSpeed;
        anim.SetFloat("speed", Mathf.Abs(move.x));

        // jump
        if (Input.GetButton("Jump") && ground) {
            move.y = jumpSpeed;
        } else {
            // will leave as it is
            move.y = -0.001f;
        }
        if (Input.GetButtonUp("Jump") && body.velocity.y > 0) {
            move.y = body.velocity.y * jumpModifier;
        }

        // attack
        if (Input.GetKeyDown("j")) {
            Attack();
        }
    }

    public void FixedUpdate() {
        if (Time.time - knockBackTime < knockBackDuration) {
            // Can not move when knocked or use force when knocked?
        } else {
            body.velocity = new Vector2(move.x, body.velocity.y);
            if (move.y > 0) body.velocity = new Vector2(body.velocity.x, move.y);
        }
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

    private void OnDrawGizmos() {
        if (showAttackArea)
            Gizmos.DrawCube(attackPos.position, attackSize);
    }

    private bool IsGrounded() {
        RaycastHit2D[] results = new RaycastHit2D[16];
        int count = body.Cast(Vector2.down, results, 0.01f);

        return count > 0;
    }

}
