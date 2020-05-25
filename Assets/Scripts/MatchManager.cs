using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OwlTawitTawoo;

public class MatchManager : MonoBehaviour
{
	public static event UnityAction<bool> gameEndedEvent;
	public static event UnityAction matchClearedEvent;

	public int maxMatch = 3;

	private List<Robot> _selectedRobots = new List<Robot>();
	public static int selectedCount
	{
		get
		{
			return instance._selectedRobots.Count;
		}
	}

	private Color _targetColor = Color.white;

	private void Awake()
	{
		EnemyManager.enemiesClearedEvent += InvokeEnemiesCleared;
		Robot.onlineEvent += RobotInvokeOnline;
	}

	private void OnDesroy()
	{
		EnemyManager.enemiesClearedEvent -= InvokeEnemiesCleared;
		Robot.onlineEvent -= RobotInvokeOnline;
	}

	public void Add(Robot robot)
	{
		if (robot.offline)
			return;

		_selectedRobots.Add(robot);

		robot.Offline();

		if (_selectedRobots.Count <= 1)
		{
			_targetColor = _selectedRobots[0].currentColor;
		} 
		else
			Validate();
	}

	private void Validate()
	{
		bool matchFailed = false;

		List<Robot> robotsToCompare = new List<Robot>(_selectedRobots);
		int robotCount = robotsToCompare.Count;

		for (int i = 0; i < robotCount; i++)
		{
			if (robotsToCompare[i].currentColor != _targetColor)
			{
				matchFailed = true;
				break;
			}
		}

		if (robotCount >= maxMatch)
			_selectedRobots.Clear();

		bool matchCleared = robotCount >= maxMatch && !matchFailed;
		for (int i = robotCount - 1; i >= 0; i--)
		{
			if (matchFailed)
			{
				robotsToCompare[i].Online();
			} 
			else if (matchCleared)
			{
				robotsToCompare[i].ApplyDamage();

			}
		}

		if (matchCleared)
			matchClearedEvent?.Invoke();
		else if (matchFailed)
			LifeManager.DeductLife();
	}

	public static void Reset()
	{
		List<Robot> selected = new List<Robot>(instance._selectedRobots);
		instance._selectedRobots.Clear();
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].Online(false);
		}

		matchClearedEvent?.Invoke();
	}

	private void RobotInvokeOnline(Robot robot)
	{
		if (_selectedRobots.Contains(robot))
			_selectedRobots.Remove(robot);
	}

	private void InvokeEnemiesCleared()
	{
		GameEnd(true);
	}

	public static void GameEnd(bool victory)
	{
		StateManager.Set(StateManager.State.End);
		gameEndedEvent?.Invoke(victory);
	}

	private static MatchManager _instance;
	public static MatchManager instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<MatchManager>();
			return _instance;
		}
	}
}
