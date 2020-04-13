using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Horizontal, Vertical movement
[System.Serializable]
public class OrthogonalMovement : Movement
{
	public OrthogonalMovement(int level, float speed) : base(level, speed) {}

	public override void Move()
	{
		Vector3 targetPos = Vector2.zero;
		Vector3 dir = Vector2.zero;

		int rndIndex = Random.Range(0, 4);
		switch (rndIndex)
		{
			case 0:
				dir = Vector2.up;
				break;

			case 1:
				dir = Vector2.right;
				break;

			case 2:
				dir = Vector2.down;
				break;

			case 3:
				dir = Vector2.left;
				break;
		}

		float distance = 0;
		switch(_moveLevel)
		{
			case 1:
				distance = 1;
				break;

			case 2:
				distance = 5;
				break;

			case 3:
				distance = Random.Range(2, 20);
				break;
		}

		distance *= 0.5f;

		Vector3 pos = _targetGO.transform.position;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, dir, distance, _ignoreLayer);
		if (raycastHit2D.collider != null)
		{
			distance = raycastHit2D.distance - 0.5f;
		}

		targetPos = pos + (dir * distance);
		Debug.DrawLine(pos, targetPos, Color.red, _moveSpeed * distance);
		LeanTween.move(_targetGO, targetPos, _moveSpeed * distance).setOnComplete(Move);
	}
}
