using ChallengeKit.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    public bool isPlay;
    public float deltaTime;

    // Update is called once per frame
    void Update()
    {
        if (isPlay)
        {
            deltaTime = Time.deltaTime;
        }
        else
        {
            deltaTime = 0;
        }
    }
}
