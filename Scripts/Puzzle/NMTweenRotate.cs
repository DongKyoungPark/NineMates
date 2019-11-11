using DG.Tweening;
using UnityEngine;

public class NMTweenRotate : NMTweenBase
{
	public Ease ease;
	/// <summary>
	/// 돌아갈 양이 아니고 마지막 결과값임을 유의
	/// 45를 입력했다면, 45도 만큼 돌아가는 것이 아니라 트윈이 끝났을 때 값이 45임.
	/// </summary>
	public float[] targetDegree;

	public override Sequence ExcuteTween(Rigidbody2D rigidbody2D, float interval, float duration)
	{
		Sequence sequence = DOTween.Sequence();
		if (targetDegree.Length == 0)
			Debug.LogError("Target Degree not found. What direction should I turn?");

		for (int i = 0; i < targetDegree.Length; i++)
		{
			Tween tween = rigidbody2D.DORotate(targetDegree[i], duration);
			tween.SetEase(ease);

			sequence.Append(tween);
			sequence.AppendInterval(interval);
		}

		return sequence;
	}
}