using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using NM.GooglePlay;

namespace NM
{
    public class Title : NMScene
    {
        public override IEnumerator PreEnter(Scene nextScene)
        {
            NMUI.SetActiveParent(typeof(Title));                   
            yield break;
        }
        public override void Enter()
        {
            base.Enter();
            Debug.Log("login start");

            #region delete
            //    	if (User.IsGuest() == false && User.IsSocial() == false) // 처음들어옴~
            //         {
            //             User.Instance.SignIn((success) =>
            //               {
            //                   if (success)
            //                   {
            //                       Debug.Log("Title SUCCESS!!!!!");
            //                       //TitlePlay();
            //                   }
            //                   else
            //                   {
            //                       Debug.LogError("login fail");
            //                       //NMMain.ApplicationQuit();
            //                   }

            //                   TitlePlay();
            //               });
            //         }

            //         if (User.IsGuest() == true || User.IsSocial() == true)
            //         {
            //             Debug.Log("Restart!!");
            //             TitlePlay();
            //         }
            #endregion

            Debug.Log("login end");
            
			TitlePlay();
			
		}

		public void NextScene(int index)
        {
            Debug.Log("start loading Scene");
            NMMain.LoadScene(typeof(Zone01));
            NMUI.GetUI<AnimationHandler>("title_anim").UnRegiEvent(NextScene);
        }

        public void TitlePlay()
        {
            Animation anim = NMUI.GetUI<Animation>("title_anim");
            anim.Play();
            NMUI.Fade(0, 2);
            NMManager.Instance.ScoreClear();

            ParticleSystem rain = NMUI.GetUIEffect<ParticleSystem>("rain");
            rain.gameObject.SetActive(true);
            DOVirtual.Float(0, 400, 25, (value) => { rain.maxParticles = (int)value; });
            NMUI.GetUI<AnimationHandler>("title_anim").RegiEvent(NextScene);
        }
    }
}
