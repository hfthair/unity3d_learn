using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public float groundCheckLen = 0.1f;
    public LayerMask groundMask;
    private BoxCollider2D boxCollider;
    private Rigidbody2D body;
    private Hero hero;
    private Vector2 groundCheckPos;

    private bool movingRight;


    // Start is called before the first frame update
    void Start() {
        hero = GetComponent<Hero>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        movingRight = false;
        hero.Move(movingRight);
    }

    // Update is called once per frame
    void Update() {
        PatrolUpdate();
    }

    private void PatrolUpdate() {
        if (!GroundCheck()) {
            movingRight = !movingRight;
            hero.Move(movingRight);
        }
    }

    private bool GroundCheck() {
        Vector2 pos = GetGroundCheckPos();
        RaycastHit2D[] detect = Physics2D.RaycastAll(pos, Vector2.down, groundCheckLen, groundMask);
        return detect.Length > 0;
    }

    private Vector2 GetGroundCheckPos() {
        if (body.velocity.x > 0) {
            return new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        } else if (body.velocity.x < 0) {
            return boxCollider.bounds.min;
        } else {
            return new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        }
    }

    private void OnDrawGizmos() {
        if (boxCollider && body) {
            Vector2 pos = GetGroundCheckPos();
            Gizmos.DrawLine(pos, new Vector2(pos.x, pos.y - groundCheckLen));
        }
    }
}
