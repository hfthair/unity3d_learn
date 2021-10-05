using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 10.0f;

    public AudioClip audioJump;
    public AudioClip audioDie;

    private AudioSource audioSource;
    private Vector2 screenBound;
    private bool controlEnable;
    private bool restarting = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        controlEnable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlEnable && Input.GetKeyDown("space"))
        {
            rb.AddForce(new Vector2(0, speed), ForceMode2D.Impulse);
            audioSource.PlayOneShot(audioJump);
        }

        if (transform.position.y > screenBound.y || transform.position.y < -screenBound.y || transform.position.x < -screenBound.x)
        {
            controlEnable = false;
            rb.velocity = new Vector2(0, 0);

            GameObject textObj = GameObject.FindGameObjectWithTag("Text");
            textObj.GetComponent<TextController>().Disable();
            animator.SetBool("dead", true);

            StartCoroutine(DelayedRestart());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Barrier")
        {
            controlEnable = false;
            animator.SetBool("dead", true);

            StartCoroutine(DelayedRestart());
        }
    }

    IEnumerator DelayedRestart()
    {
        if (restarting)
        {
            yield break;
        }
        restarting = true;
        audioSource.PlayOneShot(audioDie);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SampleScene");
        restarting = false;
    }
}
