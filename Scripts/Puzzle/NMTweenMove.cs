using DG.Tweening;
using UnityEngine;

public class NMTweenMove : NMTweenBase
{
	public Ease ease;
	public Transform[] targetTransform;
	public bool moveToPlayer;
	public bool moveOutPlayer;

	public override Sequence ExcuteTween(Rigidbody2D rigidbody2D, float interval, float duration)
	{
		Sequence sequence = DOTween.Sequence();

		if (moveToPlayer)
		{
			var playerTarget = GameObject.FindGameObjectWithTag("PlayerPick");
			targetTransform = new Transform[1];
			targetTransform[0] = playerTarget.transform;

			sequence.onComplete += () => {
				rigidbody2D.transform.parent = playerTarget.transform;
				rigidbody2D.transform.localPosition = Vector3.zero;
			};

			var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
			player.GetComponent<Character>().GoToState(CharacterState.Pick);
		}

		if (moveOutPlayer)
		{
			rigidbody2D.transform.parent = this.transform;

			var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
			player.GetComponent<Character>().GoToState(CharacterState.Idle);
		}

		if (targetTransform.Length == 0)
			Debug.LogError("Target transform not found. Where should I go?");

		for (int i = 0; i < targetTransform.Length; i++)
		{
			Tween tween = rigidbody2D.DOMove(targetTransform[i].position, duration);
			tween.SetEase(ease);

			sequence.Append(tween);
			sequence.AppendInterval(interval);
		}

		return sequence;
	}

    // 민경해 : ExcuteTween 에서 구현 의도대로 가정한다면 해당 Tween에서 실제 모든 엑션의 총 시간은 duration * targetTransform.Length 을 의미합니다.
    public override int GetTweenCount()
    {
        return Mathf.Max(0, targetTransform.Length);
    }
}
