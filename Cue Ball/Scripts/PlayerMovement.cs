using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float countdownAmount = 300f;
    public Text countdownText;
    public GameObject konamiOnInitial;
    public GameObject konamiOn;
    public GameObject konamiOffInitial;
    public GameObject konamiOff;
    public float force = 800f;

    Rigidbody rb;
    DisplayTimer timer;
    int hits;
    float countdown;
    Vector3 defaultPosition = new Vector3(0f, 0.5f, -10f);
    bool forward = false;
    bool birdsEye = false;
    FollowPlayer script;
    bool collided = true;
    string minutes, seconds;
    int konamiSequenceIndex = 0;
    bool konami = false;
    bool konamiOnFirst = true;
    bool konamiOffFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = GameObject.Find("Time Text").GetComponent<DisplayTimer>();
    }

    // When level is loaded initialise snooker table to set ball positions.
    void OnEnable()
    {
        SceneManager.sceneLoaded += Initialise;
    }

    // "Initialise" function will stop listening for the scene to be loaded once the script is disabled.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= Initialise;
    }

    // Sets the cue ball's initial position on the snooker table and sets the timer to begin at 0 as well as setting other behaviors.
    void Initialise(Scene scene, LoadSceneMode mode)
    {
        transform.position = defaultPosition;

        if (timer != null)
            timer.time = 0f;

        hits = 0;
        countdown = countdownAmount;
    }

    // Update is called once per frame.
    void Update()
    {
        KonamiCode();

        // The ball can only be hit if it's (more or less) stationary and bird's eye view isn't enabled.
        if (rb.velocity.magnitude < 0.1f)
        {
            GameObject.Find("Ball Stopped").GetComponent<Canvas>().enabled = true;

            // If cue ball didn't hit another snooker ball, apply penalty.
            if (!collided)
            {
                StartCoroutine(Penalty(5f));
                collided = true;
            }

            if ((!birdsEye) && (Input.GetKey("space")))
            {
                forward = true;
                GameObject.Find("Ball Stopped").GetComponent<Canvas>().enabled = false;
            }
        }

        if (Input.GetKeyUp("t"))
            birdsEye = !birdsEye;
    }

    // If the Konami Code is entered, the game will toggle between "Normal" and "Time Trial" mode. In Time Trail mode you
    // have 5 minutes to clear the table. If you do not complete the game in that time, you will lose. The countdown timer will begin
    // at the start of the level and continue even if you go back to normal mode, so if you toggle it on again the countdown timer will
    // have decreased during the intervening time. If you trigger time trail mode after 5 minutes you will automatically lose the game.
    void KonamiCode()
    {
        // Update the countdown timer.
        countdown -= Time.deltaTime;
        seconds = Mathf.Floor(countdown % 60).ToString("00");
        minutes = Mathf.Floor(countdown / 60).ToString("00");
        countdownText.text = minutes + ":" + seconds;

        // If Time Trail is activated and the countdown timer has finished, go to game over screen.
        if ((konami) && (countdown <= 0))
        {
            countdownText.GetComponent<Text>().enabled = false;
            SceneManager.LoadScene(5);
        }

        KeyCode[] konamiSequence = { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow,
                                     KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A, KeyCode.Return };

        // If the enter bar is pressed, the pop-up information about the Konami Code being enabled/disabled will be removed.
        if ((konamiOnInitial.activeSelf == true) && (Input.GetKeyDown(KeyCode.Return)))
            konamiOnInitial.SetActive(false);

        if ((konamiOn.activeSelf == true) && (Input.GetKeyDown(KeyCode.Return)))
            konamiOn.SetActive(false);

        if ((konamiOffInitial.activeSelf == true) && (Input.GetKeyDown(KeyCode.Return)))
            konamiOffInitial.SetActive(false);

        if ((konamiOff.activeSelf == true) && (Input.GetKeyDown(KeyCode.Return)))
            konamiOff.SetActive(false);

        // Check that the Konami sequence is entered in order.
        if (Input.GetKeyDown(konamiSequence[konamiSequenceIndex]))
        {
            konamiSequenceIndex++;

            // Implement Konami Code functionality.
            if (konamiSequenceIndex >= konamiSequence.Length)
            {
                konamiSequenceIndex = 0;
                konami = !konami;

                if (konami)
                {
                    // If first time Konami Code has been enabled, trigger explanation of what  
                    // it means. Otherwise, give a brief notification that it has been enabled.
                    if (konamiOnFirst)
                        konamiOnInitial.SetActive(true);
                    else
                        konamiOn.SetActive(true);

                    // Replace normal timer with coundown timer.
                    GameObject.Find("Time Text").GetComponent<Text>().enabled = false;
                    countdownText.GetComponent<Text>().enabled = true;
                }
                else
                {
                    // If first time Konami Code has been disabled, trigger explanation that this  
                    // has happened. Otherwise, give a brief notification that it has been disabled.
                    if (konamiOffFirst)
                        konamiOffInitial.SetActive(true);
                    else
                        konamiOff.SetActive(true);

                    // Replace countdown timer with normal timer.
                    countdownText.GetComponent<Text>().enabled = false;
                    GameObject.Find("Time Text").GetComponent<Text>().enabled = true;
                }

                // Set flags to false to indicate that the next time the
                // Konami Code is triggered, a less verbose pop-up will appear.
                if (!konamiOnFirst)
                    konamiOffFirst = false;

                konamiOnFirst = false;
            }
        }
        else if (Input.anyKeyDown)
            konamiSequenceIndex = 0;
    }

    // Adds a set of seconds to the timer and displays text to the screen that 
    // notifies that a penalty has been given. Text will then disappear after 2 seconds.
    public IEnumerator Penalty(float time)
    {
        timer.time += time;
        GameObject.Find(time.ToString() + " Second Penalty").GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(2f);
        GameObject.Find(time.ToString() + " Second Penalty").GetComponent<Canvas>().enabled = false;
    }

    // Used for physics updates.
    // Movement for the cue ball is implemented here.
    void FixedUpdate()
    {
        if (forward)
        {
            rb.AddForce((force * Time.deltaTime * Camera.main.transform.forward), ForceMode.Impulse);
            hits++;
            PlayerPrefs.SetInt("hits", hits);
            forward = false;
            collided = false;
        }

        // Toggle bird's eye view of snooker table in FollowPlayer script.
        Camera.main.GetComponent<FollowPlayer>().topDown = birdsEye ? true : false;

        // If cue ball falls off table, reset it to its inital position and reset its velocity and apply penalty.
        if (transform.position.y <= -1)
        {
            ResetPosition();
            StartCoroutine(Penalty(10f));
        }
    }

    // Places the ball at its original position on the snooker table and resets its velocity.
    void ResetPosition()
    {
        transform.position = defaultPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // If cue ball gets potted, reset it to its initial position and reset its velocity and apply penalty.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Ball"))
            collided = true;

        if (collision.gameObject.name == "Table Bottom")
        {
            ResetPosition();

            // Penalty is already applied for not hitting another ball so doesn't need to be applied again in that case.
            if (collided)
                StartCoroutine(Penalty(5f));
        }
    }
}