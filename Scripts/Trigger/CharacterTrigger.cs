using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CharacterTrigger : MonoBehaviour
{
    private bool isTrigger = false;
    public string parameter;

    void Start()
    {
        isTrigger = GetComponent<Collider2D>().isTrigger;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTrigger)
            return;

        Character character = collision.gameObject.GetComponent<Character>();

        if (character == null)
            return;

        character.SetParameter(parameter, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isTrigger)
            return;

        Character character = collision.gameObject.GetComponent<Character>();

        if (character == null)
            return;

        character.SetParameter(parameter, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTrigger == false)
            return;

        Character character = collision.gameObject.GetComponent<Character>();

        if (character == null)
            return;

        character.SetParameter(parameter, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTrigger == false)
            return;

        Character character = collision.gameObject.GetComponent<Character>();

        if (character == null)
            return;

        character.SetParameter(parameter, false);
    }
}
