using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float speed = -5.0f;
    public float extra = 2.0f;

    private Vector2 screenBound;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        screenBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        width = this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.x + width / 2) < -screenBound.x)
        {
            float max = transform.position.x;
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                max = Mathf.Max(transform.parent.GetChild(i).position.x, max);
            }
            transform.position = new Vector3(max + width - extra, 0, 0);
        }
        else
        {
            transform.position = transform.position + new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
