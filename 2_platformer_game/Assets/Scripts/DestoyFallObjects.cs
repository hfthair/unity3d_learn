using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyFallObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Fall");
        Hero hero = other.gameObject.GetComponent<Hero>();
        if (hero != null) {
            hero.Die();
            StartCoroutine(DestroyObject(other.gameObject));
        }
    }

    private IEnumerator DestroyObject(GameObject t) {
        yield return new WaitForSeconds(2.0f);
        // Destroy(t);
    }
}
