﻿using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    public int maximumScore;

    float timeAndHits, finalScore;

    // Display score based on time taken to clear the snooker table and the amount of hits needed to do so.
    // This score is generated by applying a linear mapping to decrease the score as the time gets larger. 
    // It will drop from a maximum value and asymptotically approach the limit of 0 as the time increases, 
    // without actually reaching the limit. Therefore as the time increases the score will decrease at a slower rate.
    void Update()
    {
        timeAndHits = PlayerPrefs.GetFloat("time") * PlayerPrefs.GetInt("hits");
        finalScore = maximumScore - ((maximumScore * timeAndHits) / (maximumScore + timeAndHits));
        gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(finalScore).ToString();
    }
}