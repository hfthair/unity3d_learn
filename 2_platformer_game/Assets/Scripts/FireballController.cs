using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float maxDistance = 10f;
    private int damage;
    private Hero owner;

    private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Fire(Vector2 direction, Hero hero) {
        this.owner = hero; 
        this.damage = hero.fireballDamage;
        this.startPos = transform.position;

        GetComponent<Rigidbody2D>().velocity = direction * hero.fireballSpeed;
        if (direction.x < 0) {
            // TODO: fire up?
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(startPos, transform.position) > maxDistance) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Hero hero = other.gameObject.GetComponent<Hero>();
        if (hero != null) {
            if (hero.Group() == owner.Group()) {
                return;
            }
            int[] message = new int[2];
            message[0] = damage;
            if (other.transform.position.x < transform.position.x) message[1] = 0;
            else message[1] = 1;
            hero.TakeDamage(message);
        }
        // TODO Add Anim
        Destroy(gameObject);
    }
}
