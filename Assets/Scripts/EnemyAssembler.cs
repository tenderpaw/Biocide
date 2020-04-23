using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInfo
{
	public int health;
	public Movement.MovementType movementType;
	public int unselectableOfflineCount = -1;
	public bool reboot;
}

public class EnemyAssembler : MonoBehaviour
{
	[SerializeField] private GameObject _bodyPrefabGO;
	[SerializeField] private GameObject _nonePrefabGO;
	[SerializeField] private GameObject _healthPrefabGO;
	[SerializeField] private GameObject _orthogonalMovementPrefabGO;
	[SerializeField] private GameObject _diagonalMovementPrefabGO;
	[SerializeField] private GameObject _offline1PrefabGO;
	[SerializeField] private GameObject _offline2PrefabGO;
	[SerializeField] private GameObject _offline3PrefabGO;
	[SerializeField] private GameObject _rebootPrefabGO;

	[SerializeField] private int _minNumOfParts = 8;

	public Robot Spawn(EnemyInfo enemyInfo, Vector2 spawnPosition = default(Vector2))
	{
		GameObject bodyGO = Instantiate(_bodyPrefabGO, Vector3.one * 1000, Quaternion.identity, transform);
		Transform bodyTF = bodyGO.transform;
		int partsCount = 0;

		List<Transform> partsTFList = new List<Transform>();

		for (int i = 1; i < enemyInfo.health; i++)
		{
			//Vector2 pos = GetPointsFromAngle(45 * i);
			GameObject healthGO = Instantiate(_healthPrefabGO, bodyTF);
			//healthGO.transform.localEulerAngles = new Vector3(0, 0, GetEulerZ(pos));
			partsTFList.Add(healthGO.transform);
			partsCount++;
		}

		if (enemyInfo.movementType != Movement.MovementType.None)
		{
			partsCount++;
			switch (enemyInfo.movementType)
			{
				case Movement.MovementType.Diagonal:
					//Vector2 pos = GetPointsFromAngle(200);
					GameObject movmentGO = Instantiate(_diagonalMovementPrefabGO, bodyTF);
					//movmentGO.transform.localEulerAngles = new Vector3(0, 0, GetEulerZ(pos));
					partsTFList.Add(movmentGO.transform);
					break;

				case Movement.MovementType.Orthogonal:
					//pos = GetPointsFromAngle(200);
					movmentGO = Instantiate(_orthogonalMovementPrefabGO, bodyTF);
					//movmentGO.transform.localEulerAngles = new Vector3(0, 0, GetEulerZ(pos));
					partsTFList.Add(movmentGO.transform);
					break;
			}
		}

		if (enemyInfo.unselectableOfflineCount >= 0)
		{
			partsCount++;
			GameObject partsGO = null;
			switch (enemyInfo.unselectableOfflineCount)
			{
				case 0:
					partsGO = Instantiate(_offline1PrefabGO, bodyTF);
					break;

				case 1:
					partsGO = Instantiate(_offline2PrefabGO, bodyTF);
					break;

				case 2:
					partsGO = Instantiate(_offline3PrefabGO, bodyTF);
					break;
			}

			partsTFList.Add(partsGO.transform);
		}

		if (enemyInfo.reboot)
		{
			partsCount++;
			partsTFList.Add(Instantiate(_rebootPrefabGO, bodyTF).transform);
		}

		if (partsCount < _minNumOfParts)
		{
			int numMissingParts = _minNumOfParts - partsCount;
			for (int i = 0; i < numMissingParts; i++)
			{
				partsCount++;
				partsTFList.Add(Instantiate(_nonePrefabGO, bodyTF).transform);
			}
		}

		if (partsCount > 0)
		{
			float angleSpacing = 360 / partsCount;
			for (int i = 0; i < partsTFList.Count; i++)
			{
				Vector2 pos = GetPointsFromAngle((angleSpacing + 1) * i);
				partsTFList[i].localPosition = pos;
				partsTFList[i].localEulerAngles = new Vector3(0, 0, GetEulerZ(pos));
			}
		}

		Robot robot = bodyGO.GetComponent<Robot>();
		robot.Init(enemyInfo, spawnPosition);
		return robot;
	}

	private Vector2 GetPointsFromAngle(float angle, float distance = 0.65f)
	{
		angle *= Mathf.PI / 180;
		return new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));
	}

	private float GetEulerZ(Vector2 pos)
	{
		float eulerZ = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - 90;
		return eulerZ;
	}

	private static EnemyAssembler _instance;
	public static EnemyAssembler instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<EnemyAssembler>();
			return _instance;
		}
	}
}
