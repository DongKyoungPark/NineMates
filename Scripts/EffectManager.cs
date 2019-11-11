using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using ChallengeKit.Pattern;
using System;
using NM;

public class EffectManager : Singleton<EffectManager>, NM.IManager
{
    public GameManager gameManager;
    public ResourceManager resourceManager;

    [SerializeField]
    private List<GameObject> EffectList;

    private string categoryName = "Effect";
    private float collectStartDelayAfterUsingEffect = 5.0f;
    private float delayTime = 0.0f;

    private List<GameObject> activeEffects = new List<GameObject>();

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GetComponent<GameManager>();
        }
    }

    private void Update()
    {
        delayTime += Time.deltaTime;

        if (delayTime > collectStartDelayAfterUsingEffect)
        {
            foreach (var item in activeEffects)
            {
                resourceManager.CollectGameObject(categoryName, item);
            }
            activeEffects.Clear();
        }
    }

    public bool PreEnter()
    {
        if (gameManager != null)
        {
            resourceManager = gameManager.resourceManager;
        }

        foreach (var prefab in EffectList)
        {
            resourceManager.SetPrefab<GameObject>(categoryName, prefab.name, prefab, transform);
        }
        return true;
    }

    public void OnEffect(string EffectName, Vector3 position)
    {
        if (gameManager == null && EffectList.Find((x) => x.name == EffectName) == null)
        {
            return;
        }

        delayTime = 0.0f;

        var effectObject = resourceManager.GetObject<GameObject>(categoryName, EffectName);

        if (effectObject == null)
            return;

        effectObject.transform.position = position;

        activeEffects.Add(effectObject);
    }

    public void Init()
    {

    }

    public void Release()
    {

    }
}
