using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryTeller : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro displayer;
    //private BoxCollider2D collider;

    private void Start()
    {
        displayer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;


        displayer.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;

        displayer.enabled = false;
    }
}
