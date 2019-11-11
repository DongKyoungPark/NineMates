// 씬 구동을 위한 최최소한의 코드. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NM
{
    public class SceneBase : NMScene
    {

        
        Button continueBtn;
        Button golobbyBtn;
        GameObject menu;
        // 체크포인트 리스트업 할 것. 
        public override IEnumerator PreEnter(Scene nextScene)
        {
            NMUI.Fade(0, 2);

            //인트로 사운트 페이트 아웃 시점
            SoundManager sounds = SoundManager.instance;
            sounds.FadeOutSound(sounds.introBGM);

            sounds.PlaySound(sounds.mainBGM);
            sounds.FadeInSound(sounds.mainBGM);

            string empty = null;
            NMUI.SetActiveParent("Game");


            //exitBtn = NMUI.GetUI<Button>("exit_btn");
            continueBtn = NMUI.GetUI<Button>("continue_btn");
            golobbyBtn = NMUI.GetUI<Button>("golobby_btn");
            menu = NMUI.GetUI<Transform>("exit_menu").gameObject;
            menu.SetActive(false);
            //exitBtn.onClick.RemoveAllListeners();
            //exitBtn.onClick.AddListener(ActiveMunu);

            NMManager manager = NMManager.Instance;

            manager.effect.PreEnter();
            manager.input.PreEnter(CameraManager.Instance.GetCamera());

            // 레벨에서 체크포인트 번호 가져와서 위치 세팅할 것.
			string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

			PuzzleCheckPoint checkpoint = null;			
            
			if (sceneName.Contains("Zone"))
			{
				LevelManager.CurLevel.Init();

				if (LevelManager.CurLevel.playData._zoneNumber != int.Parse(sceneName.Substring(4, 2)))
				{
					Debug.LogWarning("ZoneNumber Not Matched!!!");
				}
				else
				{
					checkpoint = LevelManager.CurLevel.GetCheckPoint();
				}
			}

            if (checkpoint != null)
            {
                manager.character.PreEnter(manager.input, checkpoint.transform.position);
            }
            else
            {
                GameObject startPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
                if (null == startPoint)
                {
                    GameObject[] objs = nextScene.GetRootGameObjects();
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i].name == "SpawnPoint")
                        {
                            startPoint = objs[i];
                            break;
                        }
                    }
                }
                if (null != startPoint)
                {
                    manager.character.PreEnter(manager.input, startPoint.transform.position);
                }
            }

            SceneManager.MoveGameObjectToScene(manager.character.player, nextScene);

            CameraManager.Instance.SetCameraActive(true);
            CameraManager.Instance.SetFollow(manager.character.player);
            yield break;
        }

        public override void Enter()
        {
            base.Enter();
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                    menu.SetActive(menu.activeSelf == false);
                                    
            }
        }

        public void ActiveMunu()
        {
            NMMain.LoadScene(typeof(Zone01));
            return;
            menu.SetActive(menu.activeSelf == false);
            //continueBtn.gameObject.SetActive(menu.activeSelf);
            //golobbyBtn.gameObject.SetActive(menu.activeSelf);
        }
        public void ActiveFalseMunu()
        {
            menu.SetActive(false);
            //continueBtn.gameObject.SetActive(false);
            //golobbyBtn.gameObject.SetActive(false);
        }
        public void LoadTitleMunu()
        {
            NMMain.LoadScene(typeof(Title));
        }
        public override void Exit()
        {
            base.Exit();
            CameraManager.Instance.SetCameraActive(false);
        }

		protected override void OnApplicationPause(bool pause)
		{
			base.OnApplicationPause(pause);
			User.Save();
		}
	}
}