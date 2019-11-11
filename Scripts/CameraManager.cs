using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager> , NM.IManager
{
    // Cashing
    private const string cameraMainName = "Main Camera"; // fix
    private Camera mainCamera = null;
    private CinemachineVirtualCamera vCam;
    public Transform starPoint;

    public void Enter()
    {
        FindCamera();
    }

    private void FindCamera()
    {
        if (Camera.allCamerasCount > 1)
        {
            foreach (Camera cam in Camera.allCameras)
            {
                if (cam.gameObject.tag != "MainCamera") continue;

                if (cam.gameObject.name == null || false == cam.gameObject.name.Equals(cameraMainName))
                {
                    // 이건 오류임 일단 오류임 오류라고 우김
                    Debug.LogWarning($"MainCamera \"{cam.gameObject.name}\" name is not matched.");

                    //cam.gameObject.SetActive(false); //혹시모름
                }
                else
                {
                    mainCamera = cam;
                }
            }
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        vCam = FindObjectOfType<CinemachineVirtualCamera>(); // vCam은 별도 Object임. 이 클래스는 단일한가? 
        Debug.Log("vCam FOV : " + vCam.m_Lens.FieldOfView); // "cameraDistance" 라는 이름의 변수가 없음. 찾아야됨.

        GameObject camMoveObj = GameObject.FindGameObjectWithTag("CameraMoveArea");
        if (camMoveObj != null)
        {
            PolygonCollider2D cameraMoveArea = camMoveObj.GetComponent<PolygonCollider2D>();
            CinemachineConfiner cameraConfiner = FindObjectOfType<CinemachineConfiner>();
            cameraConfiner.m_BoundingShape2D = cameraMoveArea;
        }
    }

    public Camera GetCamera()
    {
        if(mainCamera == null)
        {
            FindCamera();
        }
        return mainCamera;
    }

    public void SetFollow(GameObject player)
    {
        vCam.Follow = player.transform;
    }

    public void SetCameraActive(bool isActive)
    {
        if (mainCamera == null)
        {
            FindCamera();
        }

        if (mainCamera == null) return;

        mainCamera.gameObject.SetActive(isActive);
    }

    //--------------------------------------------------------------------
    public void ZoomIn()
    {
        StartCoroutine(SmoothZoom(3, 10));
    }

    public void ZoomOut()
    {
        StartCoroutine(SmoothZoom(3, 15));

    }

    public IEnumerator SmoothZoom(int time, int targetDistance)
    {
        float addingFOV = targetDistance - vCam.m_Lens.FieldOfView;
        float addingValue = addingFOV / 0.05f;

        //TimeController.
        while (true)
        {
            if (TimeController.Instance.deltaTime != 0)
            {
                vCam.m_Lens.FieldOfView += addingValue;

                if ((addingFOV > 0 && vCam.m_Lens.FieldOfView >= targetDistance)
                    || (addingFOV < 0 && vCam.m_Lens.FieldOfView <= targetDistance)
                    || vCam.m_Lens.FieldOfView == targetDistance)
                {
                    vCam.m_Lens.FieldOfView = targetDistance;
                    yield break;
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void Init()
    {
        
    }

    public void Release()
    {
        
    }

    //--------------------------------------------------------------------
}
