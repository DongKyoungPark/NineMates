using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDrag : TriggerBase
{
	//개발예정
	//[Tooltip("드래그로 인식할 최소 거리")]
	//public float threshold = 0.3f;
	[Tooltip("드래그 했을 때 최대 거리")]
	public float maxDistance = 3;
	private Vector3 startPosition;
	private Vector3 calculatedPosition;
	private bool isDragStart = false;

	private void Update()
	{
		for (int i = 0; i < puzzles.Length; i++)
		{
			if (puzzles[i].isSleep) continue;
			if (puzzles[i].puzzleState != PuzzleBase.State.Ready) continue;

			if (Input.touchCount >= 1)
			{
				Touch touch = Input.GetTouch(0);

				switch (touch.phase)
				{
					case TouchPhase.Began:
						var ray = Camera.main.ScreenPointToRay(touch.position);
						RaycastHit2D[] rhit = Physics2D.GetRayIntersectionAll(ray);

						for (int j = 0; j < rhit.Length; j++)
						{
							if (rhit[j].collider.gameObject == this.gameObject)
							{
								isDragStart = true;
								startPosition = puzzles[i].targetObject.position;
							}
						}
						break;
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						if (isDragStart)
						{
							Vector3 touchPos = touch.position;
							touchPos.z = 15; // 15는 캐릭터상의 좌표. 필요시 상수값 만들어서 적용
							calculatedPosition = Camera.main.ScreenToWorldPoint(touchPos);
							Debug.Log(Vector3.Distance(startPosition, calculatedPosition));
							if (Vector3.Distance(startPosition,calculatedPosition) < maxDistance)
								puzzles[i].targetObject.position = calculatedPosition;
						}
						break;
					case TouchPhase.Ended:
					case TouchPhase.Canceled:
						if (isDragStart)
						{
							isDragStart = false;
							puzzles[i].PuzzleStart(this.gameObject);
						}
						break;
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D[] rhit = Physics2D.GetRayIntersectionAll(ray);

				for (int j = 0; j < rhit.Length; j++)
				{
					if (rhit[j].collider.gameObject == this.gameObject)
					{
						isDragStart = true;
						startPosition = puzzles[i].targetObject.position;
						break;
					}
				}
			}
			else if (Input.GetMouseButton(0)) // stay
			{
				if (isDragStart)
				{
					Vector3 mousePos = Input.mousePosition;
					mousePos.z = 15;
					calculatedPosition = Camera.main.ScreenToWorldPoint(mousePos);
					Debug.Log(Vector3.Distance(startPosition, calculatedPosition));
					if (Vector3.Distance(startPosition, calculatedPosition) < maxDistance)
						puzzles[i].targetObject.position = calculatedPosition;
				}
			}
			else if (isDragStart)
			{
				isDragStart = false;
				puzzles[i].PuzzleStart(this.gameObject);
			}
		}
	}
}
