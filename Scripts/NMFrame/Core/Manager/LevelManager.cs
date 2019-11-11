using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class LevelManager: MonoBehaviour, NM.IManager
    {
        static LevelManager _instance;
        public static LevelManager Instance { get { return _instance; } }
        static Level _curLevel;
        public static Level CurLevel { get { return _curLevel; } }        

        const string LevelPath = "Level";
        void Awake()
        {
            _instance = this;
        }

        public static void Init(Scene scene)
        {
            GameObject[] objs = scene.GetRootGameObjects();
            if (null != objs)
            {
                GameObject go = null;
                for (int i = 0; i < objs.Length; i++)
                {
                    go = objs[i];
                    if (go.name == LevelPath)
                    {
                        _curLevel = go.GetComponent<Level>();
                    }
                }
            }
        }

        public void Init()
        {
            
        }

        public void Release()
        {
            
        }
    }
}
