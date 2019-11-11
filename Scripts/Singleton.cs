using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                _instance = null;
                applicationIsQuitting = false;
            }

            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                        " - there should never be more than 1 singleton!" +
                                        " Reopening the scene might fix it.");
                        return _instance;
                    }
                }
                if (_instance == null)
                {
                    Debug.LogWarning("can't find Singleton " + typeof(T).Name);
                }
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
        _instance = null;
    }
}
