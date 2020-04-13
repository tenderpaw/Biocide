using OwlTawitTawoo;

public class MainMenuPanel : Panel
{
	private void Awake()
	{
		EndPanel.gameEndedEvent += InvokeGameEnded;
		StateManager.stateChangedEvent += InvokeStateChanged;
	}

	private void OnDestroy()
	{
		EndPanel.gameEndedEvent -= InvokeGameEnded;
		StateManager.stateChangedEvent -= InvokeStateChanged;
	}

    private void InvokeStateChanged(StateManager.State state)
	{
		if (state != StateManager.State.Main)
			return;

		Toggle(true);
    }

	private void InvokeGameEnded()
	{
		Toggle(true);
	}

	public void OnInputStartGame()
	{
		EnemyManager.DestroyAll();
		StateManager.Set(StateManager.State.Game);
		Toggle(false);
	}

	private static MainMenuPanel _instance;
	public static MainMenuPanel instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<MainMenuPanel>();
			return _instance;
		}
	}
}
