using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NM;

public class EndTrriger : MonoBehaviour
{
    public string _nextSceneName;
    public bool _isSleeper = false;
    public int _checkPointID = 0;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();
        if (null != character && character.IsPlayer)
        {
            if (character.IsSleeper() == _isSleeper)
            {
                if (_checkPointID != 0 && null != LevelManager.CurLevel)
                {
                    User.GameData data = User.GetPlayData();
                    if (data._triggerIndex != _checkPointID)
                    {
                        return;
                    }                                            
                }
                PassCondition();
            }
        }
    }

    void PassCondition()
    {
        NM.NMMain.LoadScene(_nextSceneName);
        NMUI.Fade(1, 2);
    }
}
