using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class FlowTest_Title : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            CameraManager.Instance.SetCameraActive(false);
            NMUI.SetUI(1);
            yield break;
        }
        public override void Enter()
        {
            base.Enter();
            _mono.StartCoroutine(CoTitle());
        }

        public IEnumerator CoTitle()
        {
            yield return new WaitForSeconds(1f);
            NMMain.LoadScene("FlowTest_Level01");
            yield break;
        }
    }
}
