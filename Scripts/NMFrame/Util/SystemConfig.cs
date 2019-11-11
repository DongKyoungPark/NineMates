using UnityEngine;
using System.Text;

public class Config
{
    public const string LevelSceneName = "Zone";

    public static string GetZoneSceneName(int index)
    {
        return LevelSceneName + index.ToString("D2");
    }


}
