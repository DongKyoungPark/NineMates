using NM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCheckPoint : PuzzleBase
{
	public enum CheckPointType
	{
		Save,
		Achievement
	}

	public CheckPointType type;
	public int checkPointIndex;

	public override void PuzzleStart(params GameObject[] args)
	{
		base.PuzzleStart(args);

		string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		LevelManager.CurLevel.SaveCheckPoint(sceneName, checkPointIndex);

		PuzzleEnd();
	}
}
