using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInput : MonoBehaviour
{
    public float HorizontalMove
    { get { return horizontalMove; }

        set
        {
            if(value != horizontalMove)
            {
                horizontalMove = value;
                onUpdateAction.Invoke(horizontalMove);
            }
        }
    }
    protected float horizontalMove = 0f;

    // HorizontalMove 값이 변경 되었을 때 호출할 엑션을 등록합니다.
    protected Action<float> onUpdateAction;

    protected string characterID;

    public virtual void PreEnter(string characterID, Action<float> updateSpeed)
    {
        this.characterID = characterID;
        onUpdateAction += updateSpeed;
    }
}
