using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public float speed = -9.0f;

    private GameObject bird;

    private Vector2 screenBound;

    private Vector3 barrierSquareSize;
    private float opening;

    private TextController textController;

    // Start is called before the first frame update
    void Start()
    {
        bird = GameObject.FindGameObjectWithTag("Player");
        screenBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        barrierSquareSize = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().bounds.size;
        float heightDiff = Mathf.Abs(transform.GetChild(0).position.y - transform.GetChild(1).position.y);
        opening = heightDiff - barrierSquareSize.y;

        GameObject textObj = GameObject.FindGameObjectWithTag("Text");
        textController = textObj.GetComponent<TextController>();

        RandomHeight();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(speed * Time.deltaTime, 0, 0);

        if (bird != null && bird.transform.position.x > transform.position.x)
        {
            bird = null;
            textController.Increase();
        }

        if (transform.position.x + barrierSquareSize.x / 2 < -screenBound.x)
        {
            Destroy(gameObject);
        }
    }

    void RandomHeight()
    {
        float range = screenBound.y - opening / 2;
        float y = Random.Range(-range, range);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
