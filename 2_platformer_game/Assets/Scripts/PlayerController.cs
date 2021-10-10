using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Hero
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // move
        float x = Input.GetAxisRaw("Horizontal");
        if (x > 0.01f) {
            MoveRight();
        } else if (x < -0.01f) {
            MoveLeft();
        } else {
            Stop();
        }

        // jump
        if (Input.GetButton("Jump")) {
            Jump();
        }
        if (Input.GetButtonUp("Jump")) {
            SlowdownJump();
        }

        // attack
        if (Input.GetKey("j") || Input.GetButton("Fire1")) {
            Attack();
        }
        if (Input.GetKey("k") || Input.GetButton("Fire2")) {
            Fire();
        }
    }

}
