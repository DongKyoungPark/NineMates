using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 개별 메니저의 초기화 같은거 안합니다. 접근만 하는거 도와줍니다.
public class NMManager : Singleton<NMManager>
{
    public InputManager input;
    public CharacterManager character;
    public EffectManager effect;
    public ResourceManager resource;
    public SoundManager sound;
    public AudioManager audio;

	private int score = 0;

	public void ScoreUP()
	{
		score++;
		NM.NMUI.Instance.SetUIScore(score);
	}

    public void ScoreClear()
    {
        if(score != 0 )
        {
            NM.NMUI.Instance.SetUIScore(score = 0);
        }
           
    }
}
