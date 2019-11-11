using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace NM
{
    public class NMUI : MonoBehaviour, IManager
    {
        public GameObject _uiCam;
        public GameObject[] _testGos;
        static NMUI _instance;
        public static NMUI Instance { get { return _instance; } }

        public static CustomYieldInstruction Fade(float v1, float v2)
        {
            return _instance._fade.DOFade(v1, v2).WaitForCompletion(false);
        }

        Transform _currentActiveParent;
        Transform _uiParent;
        Transform _effectParent;
        Image _fade;

        public GameObject _menuGo;
		public Text _scoreText;

        public void ActiveFalseMunu()
        {
            _menuGo.SetActive(false);
        }
        public void LoadTitleMunu()
        {
            Application.Quit();
        }

        public void Init()
        {
            _instance = this;
            _fade = GameObject.Find("Fade").GetComponent<Image>();
            _uiParent = GameObject.Find("UI_Canvas").transform;
            _effectParent = GameObject.Find("Effect_Canvas").transform;
        }

        public void Release()
        {
            _effectComponentDic.Clear();
        }

        public static void SetUI(int index)
        {
            if (null != _instance && null != _instance._testGos)
            {
                GameObject go = null;
                for (int i = 0; i < _instance._testGos.Length; i++)
                {
                    go = _instance._testGos[i];
                    if (null != go)
                    {
                        go.SetActive(i == index);
                    }
                }
            }
        }
        public static void SetActiveParent(System.Type sceneType)
        {
            SetActiveParent(sceneType == null ? null : sceneType.Name);
        }
        public static void SetActiveParent(string parentName)
        {
            int count = _instance._uiParent.childCount;
            Transform tr = null;
            bool isEqaulName = false;
            for (int i = 0; i < count; i++)
            {
                tr = _instance._uiParent.GetChild(i);
                if (null == parentName)
                {
                    if (tr != _instance._fade.transform)
                    {
                        tr.gameObject.SetActive(false);
                    }
                }
                else
                {

                    isEqaulName = tr.name == parentName;
                    if (isEqaulName)
                    {
                        _instance._currentActiveParent = tr;
                    }
                    if (tr != _instance._fade.transform)
                    {
                        tr.gameObject.SetActive(isEqaulName);
                    }
                }
            }
        }

        Dictionary<string, List<Component>> _uiComponentDic = new Dictionary<string, List<Component>>();
        public static T GetUI<T>(string name) where T : Component
        {
            T t = null;
            List<Component> componentList = null;
            if (null != _instance._currentActiveParent)
            {
                _instance._uiComponentDic.TryGetValue(name, out componentList);
                if (null != componentList)
                {
                    Component item = null;
                    for (int i = 0; i < componentList.Count; i++)
                    {
                        item = componentList[i];
                        if (item is T)
                        {
                            t = item as T;
                            break;
                        }
                    }
                    if (null == t)
                    {
                        t = item.GetComponent<T>();
                        if (null != t)
                        {
                            componentList.Add(t);
                        }
                    }
                }
                else
                {
                    t = GetComponent<T>(_instance._currentActiveParent, name);

                    if (null != t)
                    {
                        componentList = new List<Component>();
                        componentList.Add(t);
                        _instance._uiComponentDic.Add(name, componentList);
                    }
                }
            }
            return t;
        }

        [ContextMenu("@@")]
        void Test()
        {
            //Button btn = GetComponent<Button>("continue_btn");

        }

		public void ResetPlayer()
		{
			CharacterManager.Instance.ResetPlayer();
		}

		public void SetUIScore(int score)
		{
			_scoreText.text = score.ToString();
		}

        static T GetComponent<T>(Transform parent, string name) where T : Component
        {
            Transform tr = parent.Find(name);
            if (tr == null)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    tr = parent.GetChild(i).Find(name);
                    if (null != tr)
                    {
                        break;
                    }
                }
            }
            T t = null;
            if (null != tr)
            {
                t = tr.GetComponent<T>();
            }
            return t;
        }

        Dictionary<string, List<Component>> _effectComponentDic = new Dictionary<string, List<Component>>();
        public static T GetUIEffect<T>(string name) where T : Component
        {
            T t = null;
            List<Component> componentList = null;
            if (null != _instance._effectParent)
            {
                _instance._effectComponentDic.TryGetValue(name, out componentList);
                if (null != componentList)
                {
                    Component item = null;
                    for (int i = 0; i < componentList.Count; i++)
                    {
                        item = componentList[i];
                        if (item is T)
                        {
                            t = item as T;
                            break;
                        }
                    }
                }
                else
                {
                    t = GetComponent<T>(_instance._effectParent, name);

                    if (null != t)
                    {
                        componentList = new List<Component>();
                        componentList.Add(t);
                        _instance._effectComponentDic.Add(name, componentList);
                    }
                }
            }
            return t;
        }

    }
}