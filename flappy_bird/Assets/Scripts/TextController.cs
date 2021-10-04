using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    private TextMesh textMesh;
    private int score;
    private bool scoreEnable;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "0";
        scoreEnable = true;
        score = 0;
    }

    public void Disable()
    {
        scoreEnable = false;
    }
    public void Increase()
    {
        if (!scoreEnable)
        {
            return;
        }
        score += 1;
        textMesh.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
