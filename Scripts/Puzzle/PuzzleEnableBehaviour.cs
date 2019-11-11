using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컴포넌트등을 켜거나 끄는 퍼즐, 요긴하게 쓸 일이 있을 것.
/// 처음에는 컬라이더 때문에 작성.
/// </summary>
public class PuzzleEnableBehaviour : PuzzleBase
{
	public bool targetStatusIsEnable;
	public Behaviour[] targetBehaviour;
	public float interval = 0;
	public float duration = 0;

	public override void PuzzleStart(params GameObject[] args)
	{
		base.PuzzleStart(args);
		if (puzzleState != State.InStep) return;

		if (playerTarget)
		{
			targetBehaviour = new Behaviour[1];
			targetBehaviour[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
		}

		Invoke("EnableTarget", interval);
	}

	public void EnableTarget()
	{
		for (int i = 0; i < targetBehaviour.Length; i++)
		{
			targetBehaviour[i].enabled = targetStatusIsEnable;
		}

		Invoke("PuzzleEnd", duration);
	}

	public override void PuzzleEnd()
	{
		base.PuzzleEnd();

		if(duration > 0)
			for (int i = 0; i < targetBehaviour.Length; i++)
			{
				targetBehaviour[i].enabled = !targetStatusIsEnable;
			}
	}
}
