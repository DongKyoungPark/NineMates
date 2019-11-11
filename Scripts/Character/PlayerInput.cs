using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : BaseInput
{
    public bool IsTouchOn = false;

    [SerializeField]
    private float stopTime = 1.0f;

    private Coroutine decreaseMoveCoroutine;

    // Update is called once per frame
    void Update () {

        if (InputManager.useInput == false)
            return;

        if (IsTouchOn)
            return;

        HorizontalMove = Input.GetAxisRaw("Horizontal");       
	}

    IEnumerator SlowdownHorizontalMove()
    {
        float currentTime = 0.0f;
        while(currentTime < stopTime)
        {
            HorizontalMove = Mathf.Lerp(horizontalMove, 0.0f, currentTime / stopTime);
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    // 폴리싱 todo : 함수 프레임 별 재호출 하지말고, 한번 호출에서 코루틴에서 커브 그래프를 이용하여
    // 기획자가 제어할 수 있도록 변경.
    
    public void AddDirection(Vector3 dir)
    {
        if (IsTouchOn == false)
            return;
        
        if (Main.CurState == Main.State.GameMenu)
            return;

        HorizontalMove = dir.x;

        if(decreaseMoveCoroutine != null)
        {
            StopCoroutine(decreaseMoveCoroutine);
        }
        
        decreaseMoveCoroutine = StartCoroutine(SlowdownHorizontalMove());
    }
}