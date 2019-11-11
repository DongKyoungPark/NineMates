using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class AnimationHandler : MonoBehaviour
    {
        System.Action<int> _event;

        Animation _anim;        
        private void Awake()
        {
            _anim = this.gameObject.GetComponent<Animation>();
            for (int i = 0; i < _anim.clip.events.Length; i++)
            {
                _anim.clip.events[i].objectReferenceParameter = this;
            }
        }
        public void RegiEvent(System.Action<int> action)
        {
            _event += action;
        }
        public void UnRegiEvent(System.Action<int> action)
        {
            _event -= action;
        }
        void AnimEvent(int index)
        {
            _event?.Invoke(index);
        }
    }
}