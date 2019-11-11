using DG.Tweening;
using UnityEngine;

public class NMTweenJump : NMTweenBase
{
	public float jumpPower;
	public Transform[] targetTransform;

	public override Sequence ExcuteTween(Rigidbody2D rigidbody2D, float interval, float duration)
	{
		Sequence sequence = DOTween.Sequence();
		if (targetTransform.Length == 0)
			Debug.LogError("Target transform not found. Where should I go?");

		for (int i = 0; i < targetTransform.Length; i++)
		{
			Tween tween = rigidbody2D.DOJump(targetTransform[i].position, jumpPower, 1, duration);

			sequence.Append(tween);
			sequence.AppendInterval(interval);
		}

		return sequence;
	}
}
