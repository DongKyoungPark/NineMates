using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class CharacterTest : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            NMManager manager = NMManager.Instance;

            manager.effect.PreEnter();
            manager.input.PreEnter(CameraManager.Instance.GetCamera());

            Vector3 SpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;

            manager.character.PreEnter(manager.input, SpawnPoint);

            SceneManager.MoveGameObjectToScene(manager.character.player, nextScene);
            CameraManager.Instance.SetCameraActive(true);
            CameraManager.Instance.SetFollow(manager.character.player);

            // 현재씬에서 UI 효과 빼버림.
            NMUI.Instance.gameObject.SetActive(false);

            //SoundManager.instance.PreEnter();
            //SoundManager.instance.PlaySound(SoundManager.instance.inGameBGM, true);

            

            yield break;
        }
        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
            CameraManager.Instance.SetCameraActive(false);
        }
    }
}
