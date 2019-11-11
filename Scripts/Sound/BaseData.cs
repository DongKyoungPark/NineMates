using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData : ScriptableObject
{

    public string[] names = null;
    protected const string ID = "id";
    protected const string NAME = "name";
    protected const string LENGTH = "length";
    protected const string NEWLINE = "\n";

    public int GetDataCount()
    {
        int retVal = 0;
        if (names != null)
        {
            retVal = names.Length;
        }
        return retVal;
    }

    public string[] GetNameList(bool ShowID, string filterWord = "")
    {
        string[] retList = new string[0];

        if (names != null)
        {
            retList = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                if (filterWord != "")
                {
                    if (names[i].ToLower().Contains(filterWord.ToLower()) == false)
                    {
                        continue;
                    }
                }

                if (ShowID == true)
                {
                    retList[i] = i.ToString() + ":" + names[i];
                }
                else
                {
                    retList[i] = names[i];
                }
            }
        }

        return retList;

    }

    public virtual void RemoveData(int index)
    {

    }

    public virtual void CopyData(int index)
    {

    }
}
