using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Hero hero;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponent<Hero>();
    }

    // Update is called once per frame
    void Update()
    {
        // move
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0) {
            hero.Move(x > 0);
        } else {
            hero.Stop();
        }

        // jump
        if (Input.GetButton("Jump")) {
            hero.Jump();
        }
        if (Input.GetButtonUp("Jump")) {
            hero.SlowdownJump();
        }

        // attack
        if (Input.GetKey("j")) {
            hero.Attack();
        }
    }

}
