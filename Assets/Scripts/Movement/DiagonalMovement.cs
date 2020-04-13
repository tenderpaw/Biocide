using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Horizontal, Vertical movement
[System.Serializable]
public class DiagonalMovement : Movement
{
	private Vector2 _nextMoveDirection = Vector2.zero;
	private Vector2 _currentMoveDirection = Vector2.zero;
	public DiagonalMovement(int level, float speed) : base(level, speed) {}

	public override void Init(GameObject targetGO)
	{
		base.Init(targetGO);

		int rndIndex = Random.Range(0, 4);
		switch (rndIndex)
		{
			case 0:
				_currentMoveDirection = new Vector2(1, 1);
				break;

			case 1:
				_currentMoveDirection = new Vector2(-1, 1);
				break;

			case 2:
				_currentMoveDirection = new Vector2(-1, -1);
				break;

			default:
				_currentMoveDirection = new Vector2(1, -1);
				break;
		}
	}

	public override void Move()
	{
		_currentMoveDirection = _currentMoveDirection.normalized;
		Vector3 targetPos = Vector2.zero;
		Vector2 pos = _targetGO.transform.position;
		float moveDistance = 0;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, _currentMoveDirection, Mathf.Infinity, _ignoreLayer);
		if (raycastHit2D.collider != null)
		{
			moveDistance = raycastHit2D.distance - 0.5f;
			targetPos = pos + (_currentMoveDirection * moveDistance);
			_nextMoveDirection = Vector2.Reflect(raycastHit2D.point - pos, raycastHit2D.normal);
		}

		Debug.DrawRay(pos, _currentMoveDirection * 100, Color.red, _moveSpeed * moveDistance);
		LeanTween.move(_targetGO, targetPos, _moveSpeed * moveDistance).setOnComplete(()=> {
			_currentMoveDirection = _nextMoveDirection;
			Move();
		});
	}
}
