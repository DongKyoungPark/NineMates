using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컴포넌트등을 켜거나 끄는 퍼즐, 요긴하게 쓸 일이 있을 것.
/// 처음에는 컬라이더 때문에 작성.
/// </summary>
public class PuzzleRendererAlpha : PuzzleBase
{
	public bool targetStatusIsEnable;
	public SpriteRenderer[] targetRenderer;
	[Range(0, 1)]
	public float targetAlpha;
	public float interval = 0;
	public float duration = 1;

	public override void PuzzleStart(params GameObject[] args)
	{
		base.PuzzleStart(args);
		if (puzzleState != State.InStep) return;

		if (playerTarget)
		{
			targetRenderer = new SpriteRenderer[1];
			targetRenderer[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
		}

		Invoke("ChangeAlpha", interval);
	}

	public void ChangeAlpha()
	{
		for (int i = 0; i < targetRenderer.Length; i++)
		{
			Color spriteColor = targetRenderer[i].color;
			spriteColor.a = targetAlpha;
			targetRenderer[i].color = spriteColor;
		}

		Invoke("PuzzleEnd", duration);
	}

	public override void PuzzleEnd()
	{
		base.PuzzleEnd();

		if(duration > 0)
			for (int i = 0; i < targetRenderer.Length; i++)
			{
				targetRenderer[i].enabled = !targetStatusIsEnable;
			}
	}
}
