using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.Pattern;
using System;

public class CharacterManager : Singleton<CharacterManager>, NM.IManager
{
    public GameObject player;

    public Vector3 spawnPoint;

    private List<Character> followers = new List<Character>();

    public void PreEnter(InputManager inputManager, Vector3 spawnPoint)
    {
        // Player
        this.spawnPoint = spawnPoint;

        PreEnter_Player(inputManager);
        PreEnter_Followers();
    }

    private void PreEnter_Player(InputManager inputManager)
    {
        player = Instantiate(Resources.Load("Prefabs/Player"), spawnPoint, Quaternion.identity) as GameObject;
        player.GetComponent<Character>().PreEnter(inputManager);
    }

    private void PreEnter_Followers()
    {
        GameObject[] followerGOs = GameObject.FindGameObjectsWithTag("Follower");

        foreach (var followerGO in followerGOs)
        {
            Character follower = followerGO.GetComponent<Character>();
            follower.PreEnter(null);
            followers.Add(follower);
        }
    }

    // No longer used.
    public void Enter(InputManager inputManager = null)
    {
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;

        if (player == null)
        {
            player = Instantiate(Resources.Load("Prefabs/Player"), spawnPoint, Quaternion.identity) as GameObject;
            player.GetComponent<Character>().PreEnter(inputManager);

            SendMessage("FollowPlayerTransform", SendMessageOptions.DontRequireReceiver);

            CameraManager.Instance.SetFollow(player);
        }
    }

    public void ResetPlayer()
    {
		if (false == player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("run"))
			return;

		PuzzleCheckPoint checkPoint = NM.LevelManager.CurLevel.GetCheckPoint();
		if (checkPoint != null)
			player.transform.position = checkPoint.transform.position;
		else
			player.transform.position = spawnPoint;
    }

    public void Init()
    {
        
    }

    public void Release()
    {
        followers.Clear();
    }
}
