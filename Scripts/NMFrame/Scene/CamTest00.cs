using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class Cam : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            yield break;
        }
        public override void Enter()
        {
            base.Enter();            
        }
        public override void Exit()
        {
            base.Enter();
        }
    }
}
