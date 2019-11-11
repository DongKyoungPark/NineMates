using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class NMMain : MonoBehaviour
    {
        public static bool _useInput = true;

        [SerializeField] static MonoBehaviour _mainMono;
        [SerializeField] static GameObject _mainGo;
        [SerializeField] static NMMain _instance;
        [SerializeField] static NMScene _curScene;
        [SerializeField] static Transform _managerRoot;
        [SerializeField] IManager[] managers;

        public bool _isTestFlow = false;

        const string StartSceneName = "Intro";
        const string MainSceneName = "NMMain";
        const string SceneNameSpace = "NM.";
        public const string NMSceneGoName = "NMScene_";

        const float _sceneLoadStep = 2f;

        [SerializeField] static float _sceneLoadingProgress = 0f;


        [ContextMenu("Set Managers")]
        void SetManagers()
        {
            _managerRoot = GameObject.Find("Manager_root").transform;
            managers = _managerRoot.GetComponentsInChildren<IManager>();
        }

        void Awake()
        {
            _instance = this;
            Init();
        }
        
        void Init()
        {
            Application.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _mainMono = this;
            _mainGo = this.gameObject;

            InitManagers();

            bool isMain;
            Scene activeScene = GetActiveScene(out isMain);

            if (isMain)
            {
                LoadScene(_isTestFlow ? "FlowTest_Intro" : StartSceneName);
            }
            else
            {
                NMScene preLoadScene= GetNMScene(activeScene);
                LoadScene(preLoadScene._sceneName, preLoadScene);
            }
        }
        void InitManagers()
        {
            if (null == managers || managers.Length == 0)
            {
                SetManagers();
            }
            for (int i = 0; i < managers.Length; i++)
            {
                managers[i].Init();
            }
            CoManager.Init(_mainMono);            
        }
        static void ReleaseManagers()
        {
            for (int i = 0; i < _instance.managers.Length; i++)
            {
                _instance.managers[i].Release();
            }            
        }
        Scene GetActiveScene(out bool isMain)
        {
            Scene ss = SceneManager.GetActiveScene();
            Scene resultScene = SceneManager.GetSceneByName(MainSceneName);
            isMain = false;
            if (null != resultScene)
            {
                int sceneCount = SceneManager.sceneCount;
                if (sceneCount == 1)
                {
                    isMain = true;
                }
                else if (sceneCount == 2)
                {
                    for (int i = 0; i < sceneCount; i++)
                    {
                        resultScene = SceneManager.GetSceneAt(i).name == MainSceneName ? resultScene : SceneManager.GetSceneAt(i);
                    }
                }
            }
            return resultScene;
        }
        static NMScene GetNMScene(Scene scene)
        {
            NMScene nmScene = null;
            if (null != scene)
            {
                if (scene.name != MainSceneName)
                {
                    Type sceneType = Type.GetType(SceneNameSpace + scene.name);
                    GameObject newSceneGo = new GameObject(sceneType.Name);
                    nmScene = NMMonoAgent.NMMono.AddMono(newSceneGo, sceneType) as NMScene;
                    nmScene._sceneName = scene.name;
                }
            }
            return nmScene;
        }
        public static void LoadScene(Type sceneType, NMScene isPreLoadScene = null)
        {
            _instance.StartCoroutine(CoLoadScene(sceneType.Name, isPreLoadScene));
        }
        public static void LoadScene(string nextSceneName, NMScene isPreLoadScene = null)
        {
            _instance.StartCoroutine(CoLoadScene(nextSceneName, isPreLoadScene));
        }
        static IEnumerator CoLoadScene(string nextSceneName, NMScene preLoadScene = null)
        {
            _useInput = false;
            _sceneLoadingProgress = 0f;            
            if (null == preLoadScene)
            {
                yield return CoLoad(nextSceneName);
            }

            if (null != _curScene)
            {
                _curScene.Exit();
                yield return CoUnLoad();
            }
            Scene _nextScene = SceneManager.GetSceneByName(nextSceneName);

            _curScene = preLoadScene == null ? GetNMScene(_nextScene) : preLoadScene;

            ReleaseManagers();
            LevelManager.Init(_nextScene);

            yield return _instance.StartCoroutine(_curScene.PreEnter(_nextScene));
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_curScene._sceneName));
            _useInput = true;
            _curScene.Enter();
        }
        static IEnumerator CoLoad(string nextSceneName)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            ao.allowSceneActivation = false;
            while (true)
            {
                _sceneLoadingProgress += _sceneLoadingProgress - (ao.progress / 0.9f * 0.5f);
                if (0.9f <= ao.progress) break;
                yield return null;
            }
            ao.allowSceneActivation = true;
            while (!ao.isDone)
            {
                yield return null;
            }
        }
        static IEnumerator CoUnLoad()
        {
            CoManager.UnloadCo();
            System.GC.Collect();
            AsyncOperation ao = SceneManager.UnloadSceneAsync(_curScene._sceneName);
            ao.allowSceneActivation = false;
            while (true)
            {
                _sceneLoadingProgress += _sceneLoadingProgress - (ao.progress / 0.9f * 0.5f);
                if (0.9f <= ao.progress) break;
                yield return null;
            }
            ao.allowSceneActivation = true;
            while (!ao.isDone)
            {
                yield return null;
            }
        }
        public static void ApplicationQuit()
        {
            Application.Quit();
        }
    }
}