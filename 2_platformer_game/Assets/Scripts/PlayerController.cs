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
        if (x != 0) {
            Move(x > 0);
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
        if (Input.GetKey("j")) {
            Attack();
        }
    }

}
