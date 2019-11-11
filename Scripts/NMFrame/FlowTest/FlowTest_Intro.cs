using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class FlowTest_Intro : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            CameraManager.Instance.SetCameraActive(false);
            NMUI.SetUI(0);
            yield break;
        }
        public override void Enter()
        {
            base.Enter();            
            _mono.StartCoroutine(CoIntro());
        }

        public IEnumerator CoIntro()
        {
            yield return new WaitForSeconds(1f);
            NMMain.LoadScene("FlowTest_Title");
            yield break;
        }
    }
}
