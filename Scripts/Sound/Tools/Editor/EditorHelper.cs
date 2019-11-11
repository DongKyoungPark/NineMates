using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class EditorHelper
{
    public static string GetPath(UnityEngine.Object resourceAsset)
    {
        string retString = string.Empty;
        retString = AssetDatabase.GetAssetPath(resourceAsset); //Assets/95.RTS/9.ResourceData/Resources/Sound/BGM.wav
        string[] pathNode = retString.Split('/');
        bool findResource = false;

        for (int i = 0; i < pathNode.Length - 1; i++)
        {
            if (findResource == false)
            {
                if (pathNode[i] == "Resources")
                {
                    findResource = true;
                    retString = string.Empty;
                }
            }
            else
            {
                retString += pathNode[i] + "/";
            }
        }
        //Sound/
        return retString;
    }

    public static void CreateEnumStructure(string _enumName, StringBuilder _data)
    {
        string templateFilePath = "Assets/Scripts/Tools/Editor/EnumTemplate.txt";

        string entityTemplate = File.ReadAllText(templateFilePath);

        entityTemplate = entityTemplate.Replace("$DATA$", _data.ToString());
        entityTemplate = entityTemplate.Replace("$ENUM$", _enumName);
        string folderPath = "Assets/Scripts/Sound/";
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = folderPath + _enumName + ".cs";
        if (File.Exists(filePath) == true)
        {
            File.Delete(filePath);
        }
        File.WriteAllText(filePath, entityTemplate);
        AssetDatabase.Refresh();
    }
}
