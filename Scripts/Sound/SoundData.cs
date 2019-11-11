using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

public class SoundData : BaseData
{
    public SoundClip[] soundClips = new SoundClip[0];

    public string clipPath = "GameResources/Sound/";
    public const string dataDirectory = "/Scripts/Sound/Data/";

    private string xmlFilePath = string.Empty;
    private string xmlFileName = "soundData.xml";
    private string dataPath = "Data/soundData";

    private const string SOUND = "sound";
    private const string CLIP = "clip";
    private const string LOOPS = "loops";
    private const string MAXVOL = "maxvol";
    private const string PITCH = "pitch";
    private const string DOPPLERLEVEL = "dopplerLevel";
    private const string ROLLOFFMODE = "rolloffmode";
    private const string MINDISTANCE = "mindistance";
    private const string MAXDISTANCE = "maxdistance";
    private const string SPATIALBLEND = "spatialBlend";
    private const string LOOP = "loop";
    private const string CLIPPATH = "clipPath";
    private const string CLIPNAME = "clipName";
    private const string CHECKTIMECOUNT = "checktimecount";
    private const string CHECKTIME = "checktime";
    private const string SETTIMECOUNT = "settimecount";
    private const string SETTIME = "settime";
    private const string TYPE = "type";

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(SOUND);
            xml.WriteElementString(LENGTH, names.Length.ToString());
            xml.WriteWhitespace(NEWLINE);

            for (int i = 0; i < names.Length; i++)
            {
                SoundClip clip = soundClips[i];
                xml.WriteStartElement(CLIP);

                xml.WriteElementString(ID, i.ToString());
                xml.WriteElementString(NAME, names[i]);
                xml.WriteElementString(LOOPS, clip.checkTime.Length.ToString());
                xml.WriteElementString(MAXVOL, clip.maxVolume.ToString());
                xml.WriteElementString(PITCH, clip.pitch.ToString());
                xml.WriteElementString(DOPPLERLEVEL, clip.dopplerLevel.ToString());
                xml.WriteElementString(ROLLOFFMODE, clip.audioRolloffMode.ToString());
                xml.WriteElementString(MINDISTANCE, clip.minDistance.ToString());
                xml.WriteElementString(MAXDISTANCE, clip.maxDistance.ToString());
                xml.WriteElementString(SPATIALBLEND, clip.spatialBlend.ToString());
                if (clip.isLoop == true)
                {
                    xml.WriteElementString(LOOP, "true");
                }
                xml.WriteElementString(CLIPPATH, clip.clipPath);
                xml.WriteElementString(CLIPNAME, clip.clipName);
                xml.WriteElementString(CHECKTIMECOUNT, clip.checkTime.Length.ToString());

                string times = string.Empty;
                foreach (float t in clip.checkTime)
                {
                    times += t.ToString() + "/";
                }
                xml.WriteElementString(CHECKTIME, times);

                times = string.Empty;
                foreach (float t in clip.setTime)
                {
                    times += t.ToString() + "/";
                }
                xml.WriteElementString(SETTIME, times);

                xml.WriteElementString(TYPE, clip.playType.ToString());

                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    void SetLoopTime(bool _isCheck, SoundClip _clip, string _times)
    {
        if (_times == string.Empty)
        {
            return;
        }
        string timeString = _times; //    3.0f/10.0f/13.0f
        string[] time = timeString.Split('/');
        for (int i = 0; i < time.Length; i++)
        {
            if (_isCheck == true)
            {
                _clip.checkTime[i] = float.Parse(time[i]);
            }
            else
            {
                _clip.setTime[i] = float.Parse(time[i]);
            }
        }
    }

    public void AddSound(string _name, string _clipPath = "", string _clipName = "")
    {
        if (names == null)
        {
            names = new string[] { _name };
            soundClips = new SoundClip[] { new SoundClip(_clipPath, _clipName) };
        }
        else
        {
            names = ArrayHelper.Add(_name, names);
            soundClips = ArrayHelper.Add<SoundClip>(new SoundClip(), soundClips);
        }
    }

    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;

        TextAsset asset = (TextAsset)Resources.Load(dataPath, typeof(TextAsset));
        if (asset == null || asset.text == null)
        {
            AddSound("NewSound");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case LENGTH:
                            int length = int.Parse(reader.ReadString());
                            names = new string[length];
                            soundClips = new SoundClip[length];
                            break;
                        case ID:
                            currentID = int.Parse(reader.ReadString());
                            soundClips[currentID] = new SoundClip
                            {
                                realID = currentID
                            };
                            break;
                        case NAME:
                            names[currentID] = reader.ReadString();
                            break;
                        case LOOPS:
                            int count = int.Parse(reader.ReadString());
                            soundClips[currentID].checkTime = new float[count];
                            soundClips[currentID].setTime = new float[count];
                            break;
                        case MAXVOL:
                            soundClips[currentID].maxVolume = float.Parse(reader.ReadString());
                            break;
                        case PITCH:
                            soundClips[currentID].pitch = float.Parse(reader.ReadString());
                            break;
                        case DOPPLERLEVEL:
                            soundClips[currentID].dopplerLevel = float.Parse(reader.ReadString());
                            break;
                        case ROLLOFFMODE:
                            soundClips[currentID].audioRolloffMode = (AudioRolloffMode)Enum.Parse(typeof(AudioRolloffMode), reader.ReadString());
                            break;
                        case MINDISTANCE:
                            soundClips[currentID].minDistance = float.Parse(reader.ReadString());
                            break;
                        case MAXDISTANCE:
                            soundClips[currentID].maxDistance = float.Parse(reader.ReadString());
                            break;
                        case SPATIALBLEND:
                            soundClips[currentID].spatialBlend = float.Parse(reader.ReadString());
                            break;
                        case LOOP:
                            soundClips[currentID].isLoop = true;
                            break;
                        case CLIPPATH:
                            soundClips[currentID].clipPath = reader.ReadString();
                            break;
                        case CLIPNAME:
                            soundClips[currentID].clipName = reader.ReadString();
                            break;
                        case TYPE:
                            soundClips[currentID].playType = (SoundPlayType)Enum.Parse(typeof(SoundPlayType), reader.ReadString());
                            break;
                        case CHECKTIME:
                            SetLoopTime(true, soundClips[currentID], reader.ReadString());
                            break;
                        case SETTIME:
                            SetLoopTime(false, soundClips[currentID], reader.ReadString());
                            break;
                    }
                }
            }
        }

        foreach (SoundClip clip in soundClips)
        {
            clip.PreEnter();
        }
    }

    public override void RemoveData(int _index)
    {
        names = ArrayHelper.Remove(_index, names);
        if (names.Length == 0)
        {
            names = null;
        }
        soundClips = ArrayHelper.Remove<SoundClip>(_index, soundClips);
    }

    public void ClearData()
    {
        foreach (SoundClip clip in soundClips)
        {
            if (clip.GetClip() != null)
            {
                clip.ReleaseClip();
            }
        }
        soundClips = new SoundClip[0];
        names = null;
    }

    public SoundClip GetCopy(int _index)
    {
        if (_index < 0 || _index >= soundClips.Length)
        {
            return null;
        }

        SoundClip newClip = new SoundClip();
        newClip.realID = _index;
        newClip.clipPath = soundClips[_index].clipPath;
        newClip.clipName = soundClips[_index].clipName;
        newClip.maxVolume = soundClips[_index].maxVolume;
        newClip.pitch = soundClips[_index].pitch;
        newClip.dopplerLevel = soundClips[_index].dopplerLevel;
        newClip.audioRolloffMode = soundClips[_index].audioRolloffMode;
        newClip.minDistance = soundClips[_index].minDistance;
        newClip.maxDistance = soundClips[_index].maxDistance;
        newClip.spatialBlend = soundClips[_index].spatialBlend;
        newClip.isLoop = soundClips[_index].isLoop;
        newClip.checkTime = new float[soundClips[_index].checkTime.Length];
        newClip.setTime = new float[soundClips[_index].setTime.Length];
        for (int i = 0; i < newClip.checkTime.Length; i++)
        {
            newClip.checkTime[i] = soundClips[_index].checkTime[i];
            newClip.setTime[i] = soundClips[_index].setTime[i];
        }
        newClip.PreEnter();
        return newClip;
    }

    public override void CopyData(int _index)
    {
        names = ArrayHelper.Add(names[_index], names);
        soundClips = ArrayHelper.Add<SoundClip>(GetCopy(_index), soundClips);
    }
}
