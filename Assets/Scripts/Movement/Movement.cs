using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Movement
{
	[SerializeField] protected float _moveSpeed = 3;
	[Range(1,3)]
	[SerializeField] protected int _moveLevel = 1;
	[SerializeField] protected bool _random = false;
	protected LayerMask _ignoreLayer = (1 << 8);
	protected GameObject _targetGO;
	protected int _moveIndex = 0; // Use only if not random

	public Movement(int level, float speed)
	{
		_moveSpeed = speed;
		_moveLevel = level;
	}

    public virtual void Init(GameObject targetGO)
    {
		_targetGO = targetGO;
    }

	public virtual void Move()
	{
		
	}

	public virtual void Pause()
	{
		LeanTween.pause(_targetGO);
	}

	public enum MovementType
	{
		None = 0,
		Orthogonal = 1,
		Diagonal = 2,
	}

	[System.Serializable]
	public struct MovementData
	{
		public MovementType movementType;
		public int variant;
	}

	public static Movement Create(GameObject targetGO, MovementData movementData)
	{
		switch (movementData.movementType)
		{
			case MovementType.Diagonal:

				return new DiagonalMovement(3, 3);

			default:
				switch (movementData.variant)
				{
					case 3:
						return new OrthogonalMovement(3, 3);

					case 2:
						return new OrthogonalMovement(2, 3);

					default:
						return new OrthogonalMovement(1, 3);

				}


		}
		
	}
}
