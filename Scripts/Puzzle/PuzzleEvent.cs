using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEvent : MonoBehaviour
{
    public enum EventType
    {
        None,
        HeadingToPlayer
    }

    [SerializeField]
    private EventType eventType = EventType.HeadingToPlayer;

    [SerializeField]
    private Animator animator;


    [SerializeField]
    private float detectionRange = 10.0f;

    [SerializeField]
    private float breakingTime = 1.0f;


    private bool bTriggerged = false;



    private Character player;

    // Update is called once per frame
    void Update()
    {
        if (bTriggerged)
        {
            //gameObject.SetActive(false);
            return;
        }

        if (eventType != EventType.HeadingToPlayer)
            return;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Character>();
            }
        }

        if (player == null)
            return;

        if (Vector3.Distance(player.transform.position, transform.position) < detectionRange)
        {
            animator.SetTrigger("Go");
            bTriggerged = true;
            StartCoroutine(GoingToBreak());
        }
    }

    IEnumerator GoingToBreak()
    {
        float currentTime = 0.0f;

        Vector3 startingPosition = transform.position;
        while (currentTime < breakingTime)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, player.transform.position, currentTime / breakingTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;

        gameObject.SetActive(false);
        EffectManager.Instance.OnEffect("StarPopup", transform.position);

        collision.SendMessage("AwakeFromSleep");
    }

}

