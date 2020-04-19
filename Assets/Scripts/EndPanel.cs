using OwlTawitTawoo;
using UnityEngine;
using UnityEngine.Events;

public class EndPanel : Panel
{
	public static event UnityAction gameEndedEvent;
	[SerializeField] private GameObject _victoryGO;
	[SerializeField] private GameObject _loseGO;
	private bool _isVictory = false;

	public static event UnityAction nextLevelStartedEvent;

	private void Awake()
	{
		MatchManager.gameEndedEvent += InvokeGameEnded;
	}

	private void OnDestroy()
	{
		MatchManager.gameEndedEvent -= InvokeGameEnded;
	}

	private void InvokeGameEnded(bool isVictory)
	{
		if (isVictory)
			ShowVictory();
		else
			ShowLose();

		_isVictory = isVictory;
		Toggle(true);
	}

	private void ShowVictory()
	{
		_victoryGO.SetActive(true);
		_loseGO.SetActive(false);
	}

	private void ShowLose()
	{
		_victoryGO.SetActive(false);
		_loseGO.SetActive(true);
	}

	private void HideResult()
	{
		_victoryGO.SetActive(false);
		_loseGO.SetActive(false);
	}

	public void OnInputNextLevel()
	{
		HideResult();
		nextLevelStartedEvent?.Invoke();
		StateManager.Set(StateManager.State.Game);
		Toggle(false);
	}

	public void OnInputPlayAgain()
	{
		EnemyManager.DestroyAll();
		LevelManager.ResetLevel();
		LifeManager.Reset();
		StateManager.Set(StateManager.State.Game);
		Toggle(false);
	}

	private static EndPanel _instance;
	public static EndPanel instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<EndPanel>();
			return _instance;
		}
	}
}
