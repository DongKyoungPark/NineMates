using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컴포넌트등을 켜거나 끄는 퍼즐, 요긴하게 쓸 일이 있을 것.
/// 처음에는 컬라이더 때문에 작성.
/// </summary>
public class PuzzleActiveObject : PuzzleBase
{
	public bool isSetActive;
	public float interval = 0;
	public float duration = 1;

	public override void PuzzleStart(params GameObject[] args)
	{
		base.PuzzleStart(args);
		if (puzzleState != State.InStep) return;

		Invoke("SetTargetActive", interval);
	}

	public void SetTargetActive()
	{
		targetObject.gameObject.SetActive(isSetActive);

		Invoke("PuzzleEnd", duration);
	}

	public override void PuzzleEnd()
	{
		base.PuzzleEnd();

		if (duration > 0)
			targetObject.gameObject.SetActive(!isSetActive);
	}
}
