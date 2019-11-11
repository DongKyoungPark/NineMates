using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMTweenBase : MonoBehaviour
{
	public virtual Sequence ExcuteTween(Rigidbody2D rigidbody2D, float interval, float duration)
	{
		throw new System.NotImplementedException("Do Not Use Base NMTween");
	}

    // 민경해 : NMTween 이 사용하는 Tween 개수를 구하는데 씁니다. 상세 설명은 NMTweenMove에서 하겠습니다.
    public virtual int GetTweenCount()
    {
        return 0;
    }
}
