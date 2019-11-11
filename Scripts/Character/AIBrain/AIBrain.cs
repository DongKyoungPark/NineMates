using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AIBrain", menuName = "NineMates/AIBrain")]
public class AIBrain : ScriptableObject
{
    public float tickRate = 0.5f;

    public float teleportDistance = 4.0f;

    public float awayDistFromPlayer = 1f;

    public bool canFly = false;
}
