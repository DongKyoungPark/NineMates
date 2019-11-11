using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayer : TriggerBase
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
		for (int i = 0; i < puzzles.Length; i++)
		{
			if (puzzles[i].puzzleState != PuzzleBase.State.Ready) continue;
			if (false == collision.CompareTag("Player")) continue;

			// 예를 들면 "캐릭터들 몇 명이 안에 들어오면 작동" 같은것이 가능
			if (++innerCount >= puzzles[i].triggerCount)
			{
				puzzles[i].PuzzleStart(collision.gameObject);
			}
		}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (false == collision.CompareTag("Player")) return;

        //innerCount--;
    }
}
