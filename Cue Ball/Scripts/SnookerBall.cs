using UnityEngine;
using UnityEngine.SceneManagement;

public class SnookerBall : MonoBehaviour
{
    public Vector3 defaultPosition;

    Rigidbody rb;
    Vector3 offscreen;
    static int pottedBalls = 0;
    static bool redBallPotted = false;
    string[] colouredSequence = { "Yellow", "Green", "Brown", "Blue", "Pink", "Black" };
    static int sequenceIndex = 0;

    // Start is called before the first frame update.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        offscreen = defaultPosition;
        offscreen.y = 30;
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

    // Sets the snooker ball's initial position on the snooker table and reset other behaviors.
    void Initialise(Scene scene, LoadSceneMode mode)
    {
        transform.position = defaultPosition;
        pottedBalls = 0;
        redBallPotted = false;

        if (rb != null)
            rb.useGravity = true;
    }

    // Update is called once per frame.
    void FixedUpdate()
    {
        // If ball falls off table, place it back on its initial position on the snooker table and apply penalty.
        if (transform.position.y <= -1)
        {
            ResetPosition(defaultPosition);
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerMovement>().Penalty(10f));          
        }
    }

    // Moves the ball to a different position and resets its velocity.
    void ResetPosition(Vector3 position)
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // If ball gets potted, move it off screen, or handle what happens if the ball is potted in the wrong order.
    void OnCollisionEnter(Collision collision)
    {
        // Ball is potted.
        if (collision.gameObject.name.Equals("Table Bottom"))
        {
            // There are still red balls to be potted.
            if (pottedBalls < 15)
            {
                if (gameObject.tag.Equals("Coloured"))
                {
                    // If previously potted ball was red, reset the coloured ball onto the table, but
                    // if this is the second consecutive coloured ball to be potted, go to game over screen.
                    if (redBallPotted)
                        ResetPosition(defaultPosition);
                    else
                        SceneManager.LoadScene(4);

                    // Don't include this ball as a potted ball or place it off the snooker table.
                    redBallPotted = false;
                    return;
                }
                else
                    redBallPotted = true;
            }
            // All the red balls are potted.
            else
            {                
                // If the coloured ball is being potted in the right order, keep track of the the current position in the sequence.
                if (gameObject.name.Contains(colouredSequence[sequenceIndex]))
                {
                    sequenceIndex++;
                    
                    if (sequenceIndex >= colouredSequence.Length)
                        sequenceIndex = 0;
                }
                // If the coloured ball is potted in the wrong order, go to game over screen.
                else
                {
                    sequenceIndex = 0;
                    SceneManager.LoadScene(4);
                }
            }

            ResetPosition(offscreen);
            rb.useGravity = false;
            pottedBalls++;

            // If all balls are potted, go to game finished screen.
            if (pottedBalls >= 21)
                SceneManager.LoadScene(3);
        }
    }
}