using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class SoundTool : EditorWindow
{
    public const int widthMiddle = 200;
    public const int widthLarge = 300;
    public const int widthXLarge = 450;
    private int selection = 0;
    private Vector2 scrollPoint1 = Vector2.zero;
    private Vector2 scrollPoint2 = Vector2.zero;

    private AudioClip soundSource;
    private static SoundData soundData;

    [MenuItem("Tools/Sound Tool")]
    static void Init()
    {
        soundData = ScriptableObject.CreateInstance<SoundData>();
        soundData.LoadData();

        SoundTool window = (SoundTool)EditorWindow.GetWindow<SoundTool>(false, "Sound Tool");
        window.Show();
    }

    private void OnGUI()
    {
        if (SoundTool.soundData == null)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        {
            //Add, Copy, Remove Button Area.
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add", GUILayout.Width(widthMiddle)))
                {
                    SoundTool.soundData.AddSound("NewSound");
                    selection = SoundTool.soundData.GetDataCount() - 1;
                    soundSource = null;
                    GUI.FocusControl("ID");
                }

                GUI.SetNextControlName("Copy");
                if (GUILayout.Button("Copy", GUILayout.Width(widthMiddle)))
                {
                    GUI.FocusControl("Copy");
                    SoundTool.soundData.CopyData(selection);
                    soundSource = null;
                    selection = SoundTool.soundData.GetDataCount() - 1;
                }

                if (SoundTool.soundData.GetDataCount() > 1)
                {
                    GUI.SetNextControlName("Remove");
                    if (GUILayout.Button("Remove", GUILayout.Width(widthMiddle)))
                    {
                        GUI.FocusControl("Remove");
                        soundSource = null;
                        SoundTool.soundData.RemoveData(selection);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //가운데부분 UI영역
            EditorGUILayout.BeginHorizontal();
            {
                //soundData List -> selection Grid
                EditorGUILayout.BeginVertical(GUILayout.Width(widthLarge));
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical("Box");
                    {
                        scrollPoint1 = EditorGUILayout.BeginScrollView(scrollPoint1);
                        {
                            if (SoundTool.soundData.GetDataCount() > 0)
                            {
                                int prevSelection = selection;
                                selection = GUILayout.SelectionGrid(selection, SoundTool.soundData.GetNameList(true), 1);
                                if (prevSelection != selection)
                                {
                                    soundSource = null;
                                }
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();

                //상세 설정 수정하는 UI
                EditorGUILayout.BeginVertical();
                {
                    scrollPoint2 = EditorGUILayout.BeginScrollView(scrollPoint2);
                    {
                        if (SoundTool.soundData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                GUI.SetNextControlName("ID");
                                EditorGUILayout.LabelField("ID", this.selection.ToString(), GUILayout.Width(widthLarge));
                                SoundTool.soundData.names[selection] = EditorGUILayout.TextField("Name", SoundTool.soundData.names[selection], GUILayout.Width(widthXLarge));
                                SoundTool.soundData.soundClips[selection].playType = (SoundPlayType)EditorGUILayout.EnumPopup("PlayType", SoundTool.soundData.soundClips[selection].playType, GUILayout.Width(widthLarge));
                                SoundTool.soundData.soundClips[selection].maxVolume = EditorGUILayout.FloatField("Volume", SoundTool.soundData.soundClips[selection].maxVolume, GUILayout.Width(widthXLarge));
                                SoundTool.soundData.soundClips[selection].isLoop = EditorGUILayout.Toggle("Loop Clip", SoundTool.soundData.soundClips[selection].isLoop, GUILayout.Width(widthLarge));

                                EditorGUILayout.Separator();
                                if (soundSource == null && SoundTool.soundData.soundClips[selection].clipName != string.Empty)
                                {
                                    soundSource = SoundTool.soundData.soundClips[selection].GetClip();
                                }
                                soundSource = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", this.soundSource,
                                    typeof(AudioClip), false, GUILayout.Width(widthLarge));
                                if (soundSource != null)
                                {
                                    SoundTool.soundData.soundClips[selection].clipPath = EditorHelper.GetPath(this.soundSource);
                                    SoundTool.soundData.soundClips[selection].clipName = this.soundSource.name;
                                    SoundTool.soundData.soundClips[selection].pitch = EditorGUILayout.Slider("Pitch", SoundTool.soundData.soundClips[selection].pitch, -3.0f, 3.0f, GUILayout.Width(widthXLarge));
                                    SoundTool.soundData.soundClips[selection].dopplerLevel = EditorGUILayout.Slider("Doppler Level", SoundTool.soundData.soundClips[selection].dopplerLevel, 0.0f, 5.0f, GUILayout.Width(widthXLarge));
                                    SoundTool.soundData.soundClips[selection].audioRolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("RollOffMode", SoundTool.soundData.soundClips[selection].audioRolloffMode, GUILayout.Width(widthXLarge));
                                    SoundTool.soundData.soundClips[selection].minDistance = EditorGUILayout.FloatField("MinDistance", SoundTool.soundData.soundClips[selection].minDistance, GUILayout.Width(widthXLarge));
                                    SoundTool.soundData.soundClips[selection].maxDistance = EditorGUILayout.FloatField("MaxDistance", SoundTool.soundData.soundClips[selection].maxDistance, GUILayout.Width(widthXLarge));
                                    SoundTool.soundData.soundClips[selection].spatialBlend = EditorGUILayout.Slider("Spatial Blend", SoundTool.soundData.soundClips[selection].spatialBlend, 0.0f, 1.0f, GUILayout.Width(widthXLarge));
                                }
                                else
                                {
                                    SoundTool.soundData.soundClips[selection].clipName = string.Empty;
                                    SoundTool.soundData.soundClips[selection].clipPath = string.Empty;
                                }
                                EditorGUILayout.Separator();
                                if (GUILayout.Button("Add Loop", GUILayout.Width(widthXLarge)))
                                {
                                    SoundTool.soundData.soundClips[selection].AddLoop();
                                }
                                for (int i = 0; i < SoundTool.soundData.soundClips[selection].checkTime.Length; i++)
                                {
                                    EditorGUILayout.BeginVertical("box");
                                    {
                                        GUILayout.Label("Loop Step " + i, EditorStyles.boldLabel);
                                        if (GUILayout.Button("Remove", GUILayout.Width(widthMiddle)))
                                        {
                                            SoundTool.soundData.soundClips[selection].RemoveLoop(i);
                                            return;
                                        }
                                        SoundTool.soundData.soundClips[selection].checkTime[i] = EditorGUILayout.FloatField("CheckTime " + i.ToString(), SoundTool.soundData.soundClips[selection].checkTime[i], GUILayout.Width(widthXLarge));
                                        SoundTool.soundData.soundClips[selection].setTime[i] = EditorGUILayout.FloatField("SetTime " + i.ToString(), SoundTool.soundData.soundClips[selection].setTime[i], GUILayout.Width(widthXLarge));
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        //하단 버튼 영역.
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        {
            GUI.SetNextControlName("Reload");
            if (GUILayout.Button("Reload"))
            {
                GUI.FocusControl("Reload");
                soundData = ScriptableObject.CreateInstance<SoundData>();
                soundData.LoadData();
                selection = 0;
                this.soundSource = null;
            }
            GUI.SetNextControlName("Save");
            if (GUILayout.Button("Save"))
            {
                GUI.FocusControl("Save");
                SoundTool.soundData.SaveData();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            if (SoundTool.soundData.soundClips.Length > 1)
            {
                GUI.SetNextControlName("Import");
                if (GUILayout.Button("Import"))
                {
                    GUI.FocusControl("Import");
                    CreateEnumStructure();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CreateEnumStructure()
    {
        string enumName = "SoundList";
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();
        for (int i = 0; i < SoundTool.soundData.names.Length; i++)
        {
            if (SoundTool.soundData.names[i].ToLower().Contains("none") == false)
            {
                builder.AppendLine("\t" + SoundTool.soundData.names[i] + " = " + i.ToString() + ",");
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }
}

