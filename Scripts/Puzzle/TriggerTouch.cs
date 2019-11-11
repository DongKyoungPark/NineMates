using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTouch : TriggerBase
{
	private void Update()
	{
		for (int i = 0; i < puzzles.Length; i++)
		{
			if (puzzles[i].isSleep) continue;
			if (puzzles[i].puzzleState != PuzzleBase.State.Ready) continue;

			if (Input.touchCount >= 1)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase != TouchPhase.Began) continue;

				//Debug.Log("Casting Touch Ray To Point X:" + touch.position.x + " Y:" + touch.position.y);

				var ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit2D[] rhit = Physics2D.GetRayIntersectionAll(ray);

				for (int j = 0; j < rhit.Length; j++)
				{
					if (rhit[j].collider.gameObject == this.gameObject)
					{
						Debug.Log("Ray Shot and hit! " + rhit[j].collider.gameObject.name);

						if (++innerCount >= puzzles[i].triggerCount)
						{
							puzzles[i].PuzzleStart(rhit[j].collider.gameObject);
							innerCount = 0;
						}

						break;
					}
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				//Debug.Log("Casting Mouse Ray To Point X:" + Input.mousePosition.x + " Y:" + Input.mousePosition.y);
				//Debug.Log("Screen To World Point X:" + Camera.main.ScreenToWorldPoint(Input.mousePosition).x + " Y:" + Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D[] rhit = Physics2D.GetRayIntersectionAll(ray);

				for (int j = 0; j < rhit.Length; j++)
				{
					if (rhit[j].collider.gameObject == this.gameObject)
					{
						if (++innerCount >= puzzles[i].triggerCount)
						{
							puzzles[i].PuzzleStart(rhit[j].collider.gameObject);
							innerCount = 0;
						}

						break;
					}
				}
			}
		}
	}
}
