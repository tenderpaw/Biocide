using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OwlTawitTawoo;
using UnityEngine.Events;

[System.Serializable]
public class Level
{
	public List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();
}

public class LevelManager : MonoBehaviour
{
	public static event UnityAction<int> levelUpdatedEvent;

	[SerializeField] private List<Level> _levels = new List<Level>();
	[SerializeField] private int _loadTestLevel = -1;
	private static int _level = 1;
	public static int level {
		get
		{
			return _level;
		}
		private set {
			_level = value;
			levelUpdatedEvent?.Invoke(level);
		}

	}

	private List<Vector2> _spawnPositionList = new List<Vector2>();

	private void Awake()
	{
		for (int x = -2; x <= 2; x++)
		{
			for (int y = -4; y <= 4; y++)
			{
				_spawnPositionList.Add(new Vector2(x, y));
			}
		}

		if (_loadTestLevel > 0)
			level = _loadTestLevel;
		else
			level = 1;

		MatchManager.matchClearedEvent += CheckMissing;
		StateManager.stateChangedEvent += InvokeStateToGame;
		EnemyManager.enemiesClearedEvent += LevelUp;
	}

	private void OnDestroy()
	{
		MatchManager.matchClearedEvent -= CheckMissing;
		StateManager.stateChangedEvent -= InvokeStateToGame;
		EnemyManager.enemiesClearedEvent -= LevelUp;
	}

	private void LevelUp()
	{
		level++;
	}

    public static void ResetLevel()
	{
		level = 1;
    }

	private void InvokeStateToGame(StateManager.State gameState)
	{
		if (gameState != StateManager.State.Game)
			return;

		LoadLevel();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F))
			EnemyManager.ForceVictory();
	}

	private void LoadLevel()
	{
		bool oneOff = level < 3;
		List<EnemyInfo> enemyToSpawnList = new List<EnemyInfo>();
		int unselectableCtr = 0;
		for (int i = 1; i <= level; i++)
		{
			if (i == 3)
			{
				for (int j = 0; j < 3; j++)
				{
					Movement.MovementType mType = Movement.MovementType.Diagonal;
					if (j == 1)
						mType = Movement.MovementType.Orthogonal;

					enemyToSpawnList.Add(new EnemyInfo()
					{
						health = 1,
						movementType = mType,
						unselectableOfflineCount = -1
					});
				}

			} else if (i == 2 && level == 2)
			{
				for (int j = 0; j < 3; j++)
				{
					Movement.MovementType mType = Movement.MovementType.Orthogonal;
					if (j == 1)
						mType = Movement.MovementType.None;

					enemyToSpawnList.Add(new EnemyInfo()
					{
						health = 1,
						movementType = mType,
						unselectableOfflineCount = -1
					});
				}

			} else if (i == 1 && level == 1)
			{
				for (int j = 0; j < 3; j++)
				{

					enemyToSpawnList.Add(new EnemyInfo()
					{
						health = 1,
						movementType = Movement.MovementType.None,
						unselectableOfflineCount = -1
					});
				}

			} else if (i % 4 == 0)
			{
				// Add new enemy
				enemyToSpawnList.Add(new EnemyInfo()
				{
					health = 1,
					movementType = Movement.MovementType.None,
					unselectableOfflineCount = -1
				});

				Dictionary<int, List<EnemyInfo>> enemyDict = new Dictionary<int, List<EnemyInfo>>();

				for (int j = 0; j < enemyToSpawnList.Count; j++)
				{
					if (enemyToSpawnList[j].health < 3)
					{
						if (!enemyDict.ContainsKey(enemyToSpawnList[j].health))
							enemyDict.Add(enemyToSpawnList[j].health, new List<EnemyInfo>());

						enemyDict[enemyToSpawnList[j].health].Add(enemyToSpawnList[j]);
					}
				}

				// Sort by highest hp
				var items = from pair in enemyDict
							orderby pair.Key descending
							select pair;

				// Promote 2 monsters HP by +1
				int numToPromote = 2;
				foreach (KeyValuePair<int, List<EnemyInfo>> entry in items)
				{
					for (int j = 0; j < entry.Value.Count; j++)
					{
						if (entry.Value[j].health < 3)
						{
							entry.Value[j].health++;
							numToPromote--;
						}

						if (numToPromote <= 0)
							break;
					}

					if (numToPromote <= 0)
						break;
				}

			} else if (i % 1 == 0)
			{
				for (int j = 0; j < enemyToSpawnList.Count; j++)
				{
					if (enemyToSpawnList[j].unselectableOfflineCount <= 0)
					{
						if (unselectableCtr >= 3)
							unselectableCtr = 0;
						enemyToSpawnList[j].unselectableOfflineCount = unselectableCtr++;
						break;
					}
				}

			} else if (i % 2 == 0)
			{
				for (int j = 0; j < enemyToSpawnList.Count; j++)
				{
					if (enemyToSpawnList[j].movementType == Movement.MovementType.None)
					{
						enemyToSpawnList[j].movementType = (Movement.MovementType)Random.Range(1, 3);
						break;
					}
				}

			} else if (i % 3 == 0)
			{
				for (int j = 0; j < enemyToSpawnList.Count; j++)
				{
					if (!enemyToSpawnList[j].reboot)
					{
						enemyToSpawnList[j].reboot = true;
						break;
					}
				}
			}
		}

		List<Vector2> availSpawnPosList = new List<Vector2>(_spawnPositionList);
		for (int j = 0; j < enemyToSpawnList.Count; j++)
		{
			Vector2 spawnPos = availSpawnPosList[Random.Range(0, availSpawnPosList.Count)];
			availSpawnPosList.Remove(spawnPos);
			EnemyAssembler.instance.Spawn(enemyToSpawnList[j], spawnPos);
		}
	}

	private void CheckMissing()
	{
		int count = EnemyManager.spawnedList.Count;
		List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();
		if (count == 1)
		{
			for (int i = 0; i < 2; i++)
			{
				enemyInfoList.Add(new EnemyInfo()
				{
					health = EnemyManager.spawnedList[0].hitpoints,
					unselectableOfflineCount = -1
				});
			}
		}
		else if (count == 2)
		{
			if (EnemyManager.spawnedList[0].hitpoints == EnemyManager.spawnedList[1].hitpoints)
			{
				enemyInfoList.Add(new EnemyInfo()
				{
					health = EnemyManager.spawnedList[0].hitpoints,
					unselectableOfflineCount = -1
				});

			}
			else if ((EnemyManager.spawnedList[0].hitpoints == 2 && EnemyManager.spawnedList[1].hitpoints == 1) ||
                (EnemyManager.spawnedList[0].hitpoints == 1 && EnemyManager.spawnedList[1].hitpoints == 2))
			{
				for (int i = 0; i < 3; i++)
				{
					enemyInfoList.Add(new EnemyInfo()
					{
						health = 1,
						unselectableOfflineCount = -1
					});
				}
			}
			else if ((EnemyManager.spawnedList[0].hitpoints == 3 && EnemyManager.spawnedList[1].hitpoints == 2) ||
                (EnemyManager.spawnedList[0].hitpoints == 2 && EnemyManager.spawnedList[1].hitpoints == 3))
			{
				for (int i = 0; i < 2; i++)
				{
					enemyInfoList.Add(new EnemyInfo()
					{
						health = 1,
						unselectableOfflineCount = -1
					});

					enemyInfoList.Add(new EnemyInfo()
					{
						health = 2,
						unselectableOfflineCount = -1
					});
				}
			}

			List<Vector2> availSpawnPosList = new List<Vector2>(_spawnPositionList);
            for (int i = 0; i < EnemyManager.spawnedList.Count; i++)
            {
				Vector3 pos = EnemyManager.spawnedList[i].transform.position;
				if (availSpawnPosList.Contains(pos))
					availSpawnPosList.Remove(pos);
            }

            for (int i = 0; i < enemyInfoList.Count; i++)
            {
				int rndIndex = Random.Range(0, availSpawnPosList.Count);
				EnemyAssembler.instance.Spawn(enemyInfoList[i], availSpawnPosList[rndIndex]);
				availSpawnPosList.Remove(availSpawnPosList[rndIndex]);
            }
            
		}
	}
}
