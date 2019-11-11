using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private bool isBurnable = true;

	private GameManager gameManager;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
	private Transform starPoint;

    public enum State
    {
        Default,
        Burning,
        Ash
    }

    public State InteractableState {  get { return state; } }
    private State state = State.Default;

    [SerializeField]
    private float burningAnimationTime = 3.0f;

    [SerializeField]
    private Color burningColor = Color.red;


    private void Start()
    {
		gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        starPoint = CameraManager.Instance.starPoint;
	}

	private void OnTriggerEnter2D(Collider2D collision)
    {
        OnGeneralTriggerHandle(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnGeneralTriggerHandle(collision);
    }

    private void OnGeneralTriggerHandle(Collider2D collision)
    {
        bool burnOn = false;
        if (collision.CompareTag("Player")
            || collision.CompareTag("Follower"))
        {
            burnOn = true;
        }
        else if (collision.CompareTag("Spreadable") && collision.gameObject.GetComponent<NPC>() != null && collision.gameObject.GetComponent<NPC>().InteractableState == State.Burning)
        {
            burnOn = true;
        }

        if (state == State.Default && isBurnable && burnOn)
        {
            state = State.Burning;
            animator.SetTrigger("Burn");
            //spriteRenderer.DOColor(burningColor, burningAnimationTime);
            StartCoroutine(Burning());
        }
    }

    IEnumerator Burning()
    {
        float currentTime = 0.0f;
        spriteRenderer.color = burningColor;
		Vector3 start = this.transform.position;
		Vector3 middle = start + new Vector3(Random.Range(0f, 1f) < 0.5 ? 30 : -30, Random.Range(-1, 3), 0);

		Collider2D[] colliders = GetComponents<Collider2D>();
		for (int i = 0; i < colliders.Length; i++)
			colliders[i].enabled = false;

		GetComponent<Rigidbody2D>().simulated = false;

		while (currentTime < burningAnimationTime)
        {

			transform.position = Vector3.Lerp(start, Vector3.Lerp(middle, starPoint.position, currentTime / burningAnimationTime * 3), currentTime / burningAnimationTime * 3);

            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        state = State.Ash;
        gameObject.SetActive(false);

		//날아가는 이펙트
		EffectManager.Instance.OnEffect("Explosion", transform.position);
    }
}
