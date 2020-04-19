using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OwlTawitTawoo;
using UnityEngine.Events;

public static class LifeManager
{
	public const int STARTING_LIFE_POINTS = 3;
	public const int MAX_LIFE_POINTS = 3;
	public static int lifePoints = STARTING_LIFE_POINTS;

	public static event UnityAction<int, int> lifeUpdatedEvent;

	public static void Reset()
	{
		SetLife(STARTING_LIFE_POINTS);
	}

	public static void SetLife(int amount)
	{
		lifePoints = amount;
		UpdateLifeUI();
	}

	public static void DeductLife(int amount = 1)
	{
		lifePoints -= amount;
		if (lifePoints <= 0)
			Defeat();
		else
			UpdateLifeUI();
	}

	public static void Defeat()
	{
		if (lifePoints > 0)
			lifePoints = 0;

		UpdateLifeUI();
		MatchManager.GameEnd(false);
	}

	private static void UpdateLifeUI()
	{
		lifeUpdatedEvent?.Invoke(lifePoints, MAX_LIFE_POINTS);
	}
}
