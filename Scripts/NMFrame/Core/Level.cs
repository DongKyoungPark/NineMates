using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace NM
{
    public class Level : MonoBehaviour
    {        
		public User.GameData playData;
		public PuzzleCheckPoint[] checkpoints;

        [SerializeField] GameObject _camPosgo;
        public Vector3 StartPlayerPos
        {
            get {
                if (null != _camPosgo)
                {
                    return _camPosgo.transform.position;
                }
                return Vector3.zero;
            }
        }

		public void Init()
		{
			checkpoints = GameObject.FindObjectsOfType<PuzzleCheckPoint>();
		}

		public void SaveCheckPoint(string zoneName, int checkPointIndex)
		{
			string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			User.SetPlayData(int.Parse(sceneName.Substring(4, 2)), checkPointIndex);
		}

		public PuzzleCheckPoint GetCheckPoint()
		{
			for (int i = 0; i < checkpoints.Length; i++)
			{
				if (checkpoints[i].checkPointIndex == User.CurPlayData._triggerIndex)
					return checkpoints[i];
			}

			return null;
		}
    }
}
