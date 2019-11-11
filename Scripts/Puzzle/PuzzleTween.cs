using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTween : PuzzleBase
{
	[Tooltip("사용할 트윈")]
	public NMTweenBase nmTween;
	[Tooltip("대상에게 트윈을 할 상대를 붙인다")]
	public bool targetAttach;
	[Tooltip("트윈이 여러개 일때 그 사이의 간격")]
	public float interval;
	[Tooltip("트윈이 재생될 시간")]
	public float duration = 1;

	private float totalDuration;

	public override void RestoreState()
	{
		base.RestoreState();
	}

	public override void PuzzleStart(params GameObject[] args)
	{
		base.PuzzleStart(args);
		if (puzzleState != State.InStep) return;

		Rigidbody2D player = null;

		if (targetAttach)
		{
			targetObject.transform.parent = this.transform;
		}

		Sequence sequence = DOTween.Sequence();
		Sequence playerSequence = DOTween.Sequence();

		if (playerTarget)
		{
			// transform 관련 명령어 동작이 원하는대로 동작하기 어려움. 플레이어 별도 분리
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

			if (targetAttach)
			{
				// transform 플레이어는 Tween 실행전에 피벗역할을 하는 위치에 따로 붙어야 한다.
				Tween tween = player.DOMove(targetObject.transform.position, duration);

				//플레이어 애니메이션 변경
				//불 안붙은 성냥이 일 때 Land를 어떻게 할 것인지 고민해야한다.
				//player.GetComponent<Character>().GoToState(CharacterState.Jump);

				Character[] followers = GameObject.FindObjectsOfType<Character>();
				Rigidbody2D[] followersRigidbody = new Rigidbody2D[followers.Length];
				Animator followersAnimator;
				Sequence followerSequence = DOTween.Sequence();
				Tween followerTween;

				followerSequence.SetDelay(0);
				for (int i = 0; i < followers.Length; i++)
				{
					followersRigidbody[i] = followers[i].GetComponent<Rigidbody2D>();
					followersAnimator = followers[i].GetComponent<Animator>();

					if (false == followers[i].IsPlayer
						&& false == followersAnimator.GetCurrentAnimatorStateInfo(0).IsName("run_sleeper")
						&& false == followersAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle_sleeper")
						&& false == followersAnimator.GetCurrentAnimatorStateInfo(0).IsName("jump_sleeper"))
					{
						followersRigidbody[i].bodyType = RigidbodyType2D.Kinematic;

						followerTween = followersRigidbody[i].DOMove(targetObject.transform.position, duration);
						followerSequence.Append(followerTween);
						followerSequence.Append(nmTween.ExcuteTween(followersRigidbody[i], interval, duration));
					}
				}
				followerSequence.OnComplete(() =>
				{
					for (int i = 0; i < followersRigidbody.Length; i++)
						followersRigidbody[i].bodyType = RigidbodyType2D.Dynamic;
				});

				playerSequence.Append(tween);
				sequence.AppendInterval(duration);
			}
			else if (targetObject == null)
			{
				targetObject = player.transform;
			}

			player.bodyType = RigidbodyType2D.Kinematic;
			playerSequence.Append(nmTween.ExcuteTween(player, interval, duration));
			playerSequence.OnComplete(() =>
			{
				player.bodyType = RigidbodyType2D.Dynamic;
				player.velocity = Vector2.zero;
			});
			player.GetComponent<Character>().GoToState(characterState, playerTarget && targetAttach ? playerSequence.Duration() : duration);
		}

		sequence.Append(nmTween.ExcuteTween(targetObject.GetComponent<Rigidbody2D>(), interval, duration));

		totalDuration = sequence.Duration();

		sequence.OnComplete(() => DetachTarget(targetObject.gameObject));
	}

	public void DetachTarget(GameObject target)
	{
		if (targetAttach && false == playerTarget)
			target.transform.parent = null;

		if (isReusable)
			RestoreState();
		else
			puzzleState = State.End;

		PuzzleEnd();
	}

	public override void PuzzleEnd()
	{
		base.PuzzleEnd();
	}
}
