using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>, NM.IManager
{
    public EntityTable entityData = null;
    //key, data model..
    public Dictionary<int, EntityModel> entityTable = new Dictionary<int, EntityModel>();
    //soundData
    public SoundData soundData = null;
    private void Start()
    {
        entityData = (EntityTable)Resources.Load("Data/EntityTable");

        foreach (EntityTable.Sheet sheet in entityData.sheets)
        {
            foreach (EntityTable.Param param in sheet.list)
            {
                Debug.Log(param.ToString());
                entityTable.Add(param.ID, new EntityModel(param.ID, param.HP));

            }
        }

        if (soundData == null)
        {
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();
        }

    }

    public EntityModel GetEntityData(int ID)
    {
        if (entityTable.Count > 0)
        {
            if (entityTable.ContainsKey(ID) == true)
            {
                return entityTable[ID];
            }
        }
        return null;
    }

    public static SoundData Sound()
    {
        if (DataManager.Instance.soundData == null)
        {
            DataManager.Instance.soundData = ScriptableObject.CreateInstance<SoundData>();
            DataManager.Instance.soundData.LoadData();
        }
        return DataManager.Instance.soundData;
    }

    public void Init()
    {

    }

    public void Release()
    {

    }
}
