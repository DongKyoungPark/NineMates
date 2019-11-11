using UnityEngine;
using System.Text;

public static class NMDebug
{
    static bool _isActiveMainEventLog = true;
    const string MainEventDebugLine = "__________________________________";
    static bool _isActiveMonoEventLog = true;
    const string MonoEventDebugLine = "________";

    static StringBuilder builder = new StringBuilder();

    public static void MainLog(object owner, string method, params string[] text)
    {
        if (_isActiveMainEventLog)
        {
            builder.Clear();
            builder.Append(owner.ToString());
            builder.Append(MainEventDebugLine);
            builder.Append(method);
            for (int i = 0; i < text.Length; i++)
            {
                builder.Append(text[i]);
            }
            Debug.Log(builder.ToString());
        }
    }
    public static void MonoLog(object owner, string method, params string[] text)
    {
        if (_isActiveMonoEventLog)
        {
            builder.Clear();
            builder.Append(owner.ToString());
            builder.Append(MonoEventDebugLine);
            builder.Append(method);
            for (int i = 0; i < text.Length; i++)
            {
                builder.Append(text[i]);
            }
            Debug.Log(builder.ToString());
        }
    }
}
