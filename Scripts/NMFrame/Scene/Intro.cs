using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NM
{
    public class Intro : NMScene
    { 
        public override IEnumerator PreEnter(Scene nextScene)
        {
            NMUI.SetActiveParent(typeof(Intro));
            yield return NMUI.GetUI<Image>("image").DOFade(0f, 0f).WaitForCompletion(false);
            yield return NMUI.Fade(0, 0);
            yield break;
        }
        public override void Enter()
        {
            base.Enter();


            //여기서 사운드가 재생되어야됨.
            //SoundManager.instance.PreEnter();

            CoManager.Start(this, CoIntro());

        }
        IEnumerator CoIntro()
        {   
            SoundManager sounds = SoundManager.instance;

            yield return NMUI.GetUI<Image>("image").DOFade(1f, 2f).WaitForCompletion(false);
            yield return new WaitForSeconds(1f);

            yield return NMUI.GetUI<Image>("image").DOFade(0f, 2f).WaitForCompletion(false);
            yield return NMUI.Fade(1, 0.3f);

            sounds.PlaySound(sounds.introBGM);
            sounds.FadeInSound(sounds.introBGM);

            NMMain.LoadScene(typeof(Title));
            yield break;
        }
    }
}
