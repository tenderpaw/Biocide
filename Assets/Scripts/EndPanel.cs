using UnityEngine;
using UnityEngine.Events;

public class EndPanel : Panel
{
	public static event UnityAction gameEndedEvent;
	[SerializeField] private GameObject _victoryGO;
	[SerializeField] private GameObject _loseGO;
	private bool _isVictory = false;

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

	public void OnInputGameEnded()
	{
		gameEndedEvent?.Invoke();
		HideResult();
		Toggle(false);

		if (!_isVictory)
			LevelManager.ResetLevel();
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
