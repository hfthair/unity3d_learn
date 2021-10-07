using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingController : MonoBehaviour
{
    public Transform cam;
    public float modifier = 0.7f;

    private float width;

    // Start is called before the first frame update
    void Start()
    {
        width = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        float pos = cam.position.x * modifier;
        pos = pos + (int)((cam.position.x - pos)/width) * width;
        transform.position  = new Vector2(pos, transform.position.y);
    }
}
