using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using OwlTawitTawoo;

public class TimerPanel : Panel
{
	[SerializeField]
	private int _baseTime = 20;
	[SerializeField]
	private int _levelTimerMultiplier = 3;
	[SerializeField]
	private TMP_Text _timerText;
	public static event UnityAction gameEndedEvent;

	private void Awake()
	{
		MatchManager.gameEndedEvent += InvokeGameEnded;
		StateManager.stateChangedEvent += InvokeStateToGame;
	}

	private void OnDestroy()
	{
		MatchManager.gameEndedEvent -= InvokeGameEnded;
		StateManager.stateChangedEvent -= InvokeStateToGame;
	}

	private void InvokeGameEnded(bool dump)
	{
		Stop();
		Toggle(false);
	}

	private void InvokeStateToGame(StateManager.State gameState)
	{
		if (gameState != StateManager.State.Game)
			return;

		Toggle(true);
		int time = _baseTime + ((int)(LevelManager.level / 4f) * _levelTimerMultiplier);
		SetTimerText(time);
		Stop();
		StartTimer(time);
	}

	private void StartTimer(int counter)
	{
		_timerCoroutine = TimerCoroutine(counter);
		StartCoroutine(_timerCoroutine);
	}

	private IEnumerator _timerCoroutine;
	private IEnumerator TimerCoroutine(int counter)
	{
		for (int i = counter; i >= 0; i--)
		{
			yield return new WaitForSeconds(1);
			SetTimerText(i);
		}

		MatchManager.GameEnd(false);
	}

	private void SetTimerText(int counter)
	{
		_timerText.text = string.Format("Time:{0}", counter);
	}

	private void Stop()
	{
		if (_timerCoroutine != null)
			StopCoroutine(_timerCoroutine);
	}

	public void OnInputReset()
	{
		MatchManager.Reset();
	}

	private static TimerPanel _instance;
	public static TimerPanel instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<TimerPanel>();
			return _instance;
		}
	}
}
