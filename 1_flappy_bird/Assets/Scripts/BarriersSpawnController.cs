using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarriersSpawnController : MonoBehaviour
{
    public GameObject obj;
    public float distance = 3.0f;

    private Vector2 screenBound;

    private GameObject lastBarrier;

    // Start is called before the first frame update
    void Start()
    {
        screenBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        float x = screenBound.x * 2 / 3;
        for (; x < screenBound.x; x += distance)
        {
            AddBarrier(x);
        }
        // Add one extra so we don't have to calculate the width of the barrier.
        AddBarrier(x);
    }

    void AddBarrier(float x)
    {
        GameObject barrier = Instantiate(obj, transform) as GameObject;
        barrier.transform.position = new Vector2(x, 0);

        if (lastBarrier == null || lastBarrier.transform.position.x < barrier.transform.position.x)
        {
            lastBarrier = barrier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastBarrier != null && lastBarrier.transform.position.x < screenBound.x)
        {
            AddBarrier(lastBarrier.transform.position.x + distance);
        }
    }
}
