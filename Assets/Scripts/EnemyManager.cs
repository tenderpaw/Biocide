using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EnemyManager
{
	public static List<Robot> spawnedList { get; private set; } = new List<Robot>();
	public static event UnityAction enemiesClearedEvent;

	static EnemyManager()
	{
		Robot.destroyedEvent += Remove;
	}

	public static bool hasEnemies
	{
		get
		{
			return spawnedList.Count > 0;
		}
	}

	public static void Init()
	{
		DestroyAll();
	}

	public static void Register(Robot robot)
	{
		spawnedList.Add(robot);
	}

	public static void Remove(Robot robot)
	{
		spawnedList.Remove(robot);
		if (spawnedList.Count <= 0)
			enemiesClearedEvent?.Invoke();
	}

	public static void ForceVictory()
	{
		DestroyAll();
		if (spawnedList.Count <= 0)
			enemiesClearedEvent?.Invoke();
	}

	public static void DestroyAll()
	{
		if (spawnedList.Count <= 0)
			return;

		for (int i = spawnedList.Count - 1; i >= 0; i--)
		{
			GameObject.Destroy(spawnedList[i].gameObject);
			spawnedList.Remove(spawnedList[i]);
		}
	}
}
