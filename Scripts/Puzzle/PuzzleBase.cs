//퍼즐 : 오브젝트 안쪽에 있는 세부 단계 하나, 동작의 최소 단위. (이동 1회, 점프 1회 등)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBase : MonoBehaviour
{
    public enum State
    {
        Ready,
        InStep,
        End
    }
	[Tooltip("Ready - 대기중\nInStep - 작동중\nEnd - 끝(더이상 작동하지 않음)")]
    public State puzzleState;
	[Tooltip("퍼즐이 끝나면 State가 End가 아닌 Ready로 돌아감")]
	public bool isReusable = false;
	[Tooltip("재우기 설정. 모든 상태를 무시하고 동작하지 않음.")]
	public bool isSleep = false;
	[Tooltip("isSleep이 꺼지면 자동으로 활성화")]
	public bool isAwakeAndStart = false;

	[Tooltip("작동할 물체가 플레이어일 때")]
	public bool playerTarget;
	[Tooltip("작동할 바로 그 물체")]
	public Transform targetObject;
	[Tooltip("(터치) 몇 번 터치할 때 작동할 것인지")]
	public int triggerCount = 1;

	// 캐릭터가 바뀌어야할 스테이터스. 바로 눈에 보이는 것은 애니메이션.
	[Tooltip("작동시 플레이어의 애니메이션")]
	public CharacterState characterState = CharacterState.Idle;

	/// <summary>
	/// 퍼즐이 실행되면 이 배열안에 들어있는 퍼즐들을 Sleep 상태로 만든다.
	/// </summary>
	[Tooltip("퍼즐이 시작할 때 이 필드에 있는 모든 퍼즐을 재움")]
	public PuzzleBase[] conflictStep;

	/// <summary>
	/// 퍼즐이 끝났을 때 이 배열안에 들어있는 퍼즐이 Sleep 상태이면 깨운다.
	/// 위의 sleepStep과 동시에 사용하면 퍼즐이 실행되는 동안에만 재울 수 있다.
	/// </summary>
	[Tooltip("퍼즐이 끝날 때 이 필드에 있는 모든 퍼즐을 깨움(isSleep 해제)")]
	public PuzzleBase[] nextStep;

	/// <summary>
	/// 현재는 함수를 직접 실행하는 형태지만,
	/// 트윈과 같은 동작을 퍼즐 클래스에서 분리하면
	/// 하나의 퍼즐 클래스가 퍼즐 내 모든 스텝을 통제하는
	/// 작은 매니저로서의 역할을 할 수 있다. 시간이 난다면 고려해볼 것.
	/// </summary>
	/// <param name="args"></param>
	public virtual void PuzzleStart(params GameObject[] args)
	{
		if (false == isSleep)
		{
			puzzleState = State.InStep;

			SleepConflictStep();
		}
	}

	public virtual void PuzzleEnd()
	{
		WakeNextStep();
	}

	//리스타트 할 때 원상태로 되돌리기 위한 코드.
	public virtual void RestoreState()
	{
		puzzleState = State.Ready;
	}

	public virtual void SleepConflictStep()
	{
		for (int i = 0; i < conflictStep.Length; i++)
		{
			conflictStep[i].isSleep = true;
		}
	}

	public virtual void WakeNextStep()
	{
		for (int i = 0; i < nextStep.Length; i++)
		{
			if (nextStep[i].isSleep)
				nextStep[i].isSleep = false;
			if (nextStep[i].isAwakeAndStart)
				nextStep[i].PuzzleStart();
		}
	}
}
