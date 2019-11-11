using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>, NM.IManager
{
    public static bool useInput = true;

    public bool IsTouchOn { get { return isTouchOn; } }
    private bool isTouchOn = true;

    private PlayerInput playerInput;

    public Action OnFingerSetAction { get; set; }

    private Camera inGameCamera;

    public void PreEnter(Camera inGameCamera)
    {
        this.inGameCamera = inGameCamera;
    }

    public void Enter(Camera inGameCamera)
    {
        this.inGameCamera = inGameCamera;
    }

    public void OnFingerSet(Lean.Touch.LeanFinger finger)
    {
        if (useInput == false)
            return;

        if (playerInput == null)
            return;

        if(inGameCamera == null)
        {
            inGameCamera = GameManager.Instance.cameraManager.GetCamera();
        }

        Vector3 playerPos = playerInput.transform.position;
        Vector3 fingerPos = finger.GetWorldPosition(Vector3.Distance(playerPos, inGameCamera.transform.position), inGameCamera);

        Vector3 dir = (fingerPos - playerPos);
        playerInput.AddDirection(dir);

        if(OnFingerSetAction != null)
        {
            OnFingerSetAction.Invoke();
        }
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        playerInput.IsTouchOn = isTouchOn;
    }

    public void Init()
    {
        
    }

    public void Release()
    {
        
    }
}
