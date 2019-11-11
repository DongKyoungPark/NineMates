// 트리거 : 퍼즐을 시작하게 만드는, 해석 그대로 "방아쇠" (터치 지점, 이동 지점 등)
// 현재는 지점에 관해 정의, 추후에는 동작에도 적용 가능성 있음.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBase : MonoBehaviour
{
    public PuzzleBase[] puzzles;
	protected int innerCount = 0;

#if UNITY_EDITOR
	protected void OnDrawGizmos()
	{
		if (puzzles != null)
		{
			for (int i = 0; i < puzzles.Length; i++)
			{
				if (puzzles[i].triggerCount > 1) // Trigger는 Puzzle이 반드시 필요하기 때문에 널익셉션 나면 변수를 수정할 것.
					UnityEditor.Handles.Label(transform.position, "Touch Remain : " + (puzzles[i].triggerCount - innerCount).ToString());
			}
		}
	}
#endif
}
