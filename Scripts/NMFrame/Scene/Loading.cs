using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class Loading : NMScene
    {
        public static string _nextScene = null;
        public override void Enter()
        {
            base.Enter();
            if (string.IsNullOrEmpty(_nextScene))
            {
                NMMain.LoadScene(_nextScene);
            }
        }
        public override void Exit()
        {
            base.Enter();            
        }
    }
}
