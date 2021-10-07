using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float walkSpeed = 3.0f;
    public float jumpSpeed = 3.0f;
    public float jumpModifier = 0.5f;

    public Transform attackPos;
    public Vector2 attackSize;
    public LayerMask attackLayer;

    public bool showAttackArea = false;

    private string state;
    private Rigidbody2D body;
    private Animator anim;

    private Vector2 move = new Vector2(0, 0);

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // move
        move.x = Input.GetAxisRaw("Horizontal") * walkSpeed;
        anim.SetFloat("speed", Mathf.Abs(move.x));

        // jump
        bool ground = IsGrounded();
        if (anim.GetBool("ground") != ground) {
            anim.SetBool("ground", ground);
        }

        if (Input.GetButtonDown("Jump") && ground) {
            move.y = jumpSpeed;
        } else {
            // will leave as it is
            move.y = -0.001f;
        }
        if (Input.GetButtonUp("Jump") && body.velocity.y > 0) {
            move.y = body.velocity.y * jumpModifier;
        }

        // attack
        if (Input.GetKeyDown("j") && ground) {
            Attack();
        }

        if (move.x < 0) {
            transform.eulerAngles = new Vector3(0, 180 ,0);
        } else if (move.x > 0) {
            transform.eulerAngles = new Vector3(0, 0 ,0);
        }
    }

    public void FixedUpdate() {
        body.velocity = new Vector2(move.x, body.velocity.y);
        if (move.y > 0) body.velocity = new Vector2(body.velocity.x, move.y);
    }

    private void Attack() {
        anim.SetTrigger("attack");
    }

    private void AttackHitBoxCheck() {
        Collider2D[] detected = Physics2D.OverlapBoxAll(attackPos.position, attackSize, 0, attackLayer);
        foreach(Collider2D target in detected) {
            Debug.Log("hited: " + target.gameObject.name);
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
