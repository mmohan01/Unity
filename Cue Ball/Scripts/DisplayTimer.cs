using UnityEngine;
using UnityEngine.UI;

public class DisplayTimer : MonoBehaviour
{
    public Text scoreText;
    public /*static*/ float time;
    
    string minutes, seconds;

    // Update is called once per frame.
    void Update()
    {
        // Display the time since the game has started at the top of the level.
        time += Time.deltaTime;
        PlayerPrefs.SetFloat("time", time);
        minutes = Mathf.Floor(time / 60).ToString("00");
        seconds = Mathf.Floor(time % 60).ToString("00");
        scoreText.text = minutes + ":" + seconds;
    }
}