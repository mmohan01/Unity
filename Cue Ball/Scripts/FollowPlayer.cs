using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float rotateSpeed = 100f;
    public bool topDown = false;

    float currentRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        // Increment or decrement rotation value in relation to whether the left or right buttons are pressed.
        currentRotation += Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
    }

    // Called after all Update functions have been called.
   void LateUpdate()
    {
        // Toggles bird's eye view of snooker table on or off.
        if (topDown)
        {
            // Moves camera position to above the snooker table changes orientation of the camera to face down.
            Vector3 birdsEye = new Vector3(0, 27, 0);
            transform.position = birdsEye;
            transform.LookAt(Vector3.down);
        }
        else
        {
            // Sets the camera's default position to be behind the cue ball and sets it to rotate 
            // around the ball depending on the rotation value set by the left and right buttons.
            transform.position = player.position + offset;
            transform.LookAt(player.position + Vector3.up);
            transform.RotateAround(player.position, Vector3.up, currentRotation);
        }
    }
}