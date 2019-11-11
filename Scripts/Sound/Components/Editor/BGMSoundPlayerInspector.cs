using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGMSoundPlayer))]
public class BGMSoundPlayerInspector : BaseInspector
{
    public override void OnInspectorGUI()
    {
        BGMSoundPlayer soundPlayer = (BGMSoundPlayer)target;
        GUILayout.Label("Music Setup", EditorStyles.boldLabel);
        EventStartSettings(soundPlayer);
        EditorGUILayout.Separator();

        soundPlayer.playType = (MusicPlayType)EditorGUILayout.EnumPopup("Play Type", soundPlayer.playType);
        if (soundPlayer.playType.Equals(MusicPlayType.FADE_OUT) == false &&
            soundPlayer.playType.Equals(MusicPlayType.STOP) == false)
        {
            soundPlayer.musicClip_Index1 = EditorGUILayout.Popup("Music Clip", soundPlayer.musicClip_Index1,
                DataManager.Sound().GetNameList(true));
        }

        if (soundPlayer.playType.Equals(MusicPlayType.PLAY) == false &&
            soundPlayer.playType.Equals(MusicPlayType.STOP) == false)
        {
            soundPlayer.fadeTime = EditorGUILayout.FloatField("Fade Time (s)", soundPlayer.fadeTime);
            soundPlayer.easeType = (Interpolate.EaseType)EditorGUILayout.EnumPopup("Interpolation", soundPlayer.easeType);
        }

        EditorGUILayout.Separator();
        VariableSettings(soundPlayer);
        EditorGUILayout.Separator();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
