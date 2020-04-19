using System.Collections;
using System.Collections.Generic;
using OwlTawitTawoo;
using UnityEngine;
using TMPro;

public class InGamePanel : Panel
{
	[SerializeField] private TMP_Text _lifePointsText;
	[SerializeField] private TMP_Text _levelText;

	private void Awake()
	{
		StateManager.stateChangedEvent += InvokeStateChange;
		MatchManager.gameEndedEvent += InvokeGameEnded;
		LevelManager.levelUpdatedEvent += UpdateLevel;
		LifeManager.lifeUpdatedEvent += UpdateLife;
	}

	private void OnDestroy()
	{
		StateManager.stateChangedEvent -= InvokeStateChange;
		MatchManager.gameEndedEvent -= InvokeGameEnded;
		LevelManager.levelUpdatedEvent -= UpdateLevel;
		LifeManager.lifeUpdatedEvent -= UpdateLife;
	}

	private void InvokeGameEnded(bool dump)
	{
		Toggle(false);
	}

	private void InvokeStateChange(StateManager.State state)
	{
		Toggle(state == StateManager.State.Game);
	}

	private void UpdateLife(int lifePoints, int maxLifePoints)
	{
		_lifePointsText.text = string.Format("Life:{0}/{1}", lifePoints, maxLifePoints);
	}

	private void UpdateLevel(int level)
	{
		_levelText.text = string.Format("Level:{0}", level);
	}

	public void OnInputReset()
	{
		MatchManager.Reset();
	}
}
