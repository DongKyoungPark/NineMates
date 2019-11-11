using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class FlowTest_Direction : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            NMUI.SetUI(3);
        CameraManager.Instance.SetCameraActive(false);
            yield break;
        }
        public override void Enter()
        {
            base.Enter();
            _mono.StartCoroutine(CoDirection());
        }
        public IEnumerator CoDirection()
        {
            yield return new WaitForSeconds(1f);
            NMMain.LoadScene("FlowTest_Level02");
            yield break;
        }
    }
}
