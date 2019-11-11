using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

namespace NM
{
    public interface IManager
    {
        void Init();
        void Release();
    }
    public abstract class NMMonoAgent
    {
        public class NMMono : MonoBehaviour
        {
            public NMMonoAgent _agent;
            string _name;

            void Start() { _agent.Start(); }
            void OnEnable() { if (null != _agent) _agent.OnEnable(); }
            void OnDisable() { _agent.OnDisable(); }
            void OnDestroy() { _agent.OnDestroy(); }
            void OnApplicationPause(bool pause) { _agent.OnApplicationPause(pause); }
            void OnApplicationQuit() { _agent.OnApplicationQuit(); }

            void Update()
            {
                _agent.Update();
            }

            public static T AddMono<T>(GameObject go) where T : NMMonoAgent, new()
            {
                NMMono mono = go.AddComponent<NMMono>();
                mono._name = typeof(T).Name;
                mono._agent = new T();
                mono._agent.mono = mono;
                mono._agent.Awake();
                return mono._agent as T;
            }
            public static NMMonoAgent AddMono(GameObject go, Type t)
            {
                NMMono mono = go.AddComponent<NMMono>();
                mono._name = t.Name;
                mono._agent = Activator.CreateInstance(t) as NMMonoAgent;
                mono._agent.mono = mono;
                mono._agent.Awake();
                return mono._agent;
            }
        }

        protected virtual void Update()
        {
            
        }

        protected NMMono mono
        {
            set {
                _mono = value;
                _tr = value.transform;
                _go = value.gameObject;
            }
        }

        protected MonoBehaviour _mono;
        protected Transform _tr;
        protected GameObject _go;

        protected virtual void Awake() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
        protected virtual void Start() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
        protected virtual void OnEnable() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
        protected virtual void OnDisable() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
        protected virtual void OnDestroy() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
        protected virtual void OnApplicationPause(bool pause) { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name, pause.ToString()); }
        protected virtual void OnApplicationQuit() { NMDebug.MonoLog(this, MethodBase.GetCurrentMethod().Name); }
    }

    public abstract class NMScene : NMMonoAgent
    {
        string _typeName = null;
        public string TypeName
        {
            get {
                if (string.IsNullOrEmpty(_typeName))
                {
                    _typeName = this.GetType().Name;
                }
                return _typeName;
            }
        }
        public string _sceneName = null;
        public virtual IEnumerator PreEnter(Scene nextScene) { NMDebug.MainLog(this, MethodBase.GetCurrentMethod().Name); yield return null; }
        public virtual void Enter() { NMDebug.MainLog(this, MethodBase.GetCurrentMethod().Name); }
        public virtual void ReEnter() { NMDebug.MainLog(this, MethodBase.GetCurrentMethod().Name); }
        public virtual void Exit() { NMDebug.MainLog(this, MethodBase.GetCurrentMethod().Name); }
    }
}
