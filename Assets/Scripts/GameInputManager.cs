using System.Collections;
using UnityEngine;
using OwlTawitTawoo;

public class GameInputManager : MonoBehaviour
{
	// Update is called once per frame
	private bool _allowInput = false;

	private void Awake()
	{
		StateManager.stateChangedEvent += InvokeStateToGame;
	}

	private void OnDestroy()
	{
		StateManager.stateChangedEvent -= InvokeStateToGame;
	}

	// Prevent the player from hitting a enemy that's underneart the Start Game button
	private void InvokeStateToGame(StateManager.State gameState)
    {
		if (gameState != StateManager.State.Game)
			return;

		if (_delayAllowInputCoroutine != null)
			StopCoroutine(_delayAllowInputCoroutine);

		_delayAllowInputCoroutine = DelayAllowInputCoroutine();
		StartCoroutine(_delayAllowInputCoroutine);
	}

	private IEnumerator _delayAllowInputCoroutine;
	private IEnumerator DelayAllowInputCoroutine()
	{
		_allowInput = false;
		yield return null;
		_allowInput = true;
		_delayAllowInputCoroutine = null;
	}

	void Update()
	{
		if (!_allowInput || !StateManager.Compare(StateManager.State.Game))
			return;

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
			if (raycastHit2D)
			{
				IInGameInteractable interactable = raycastHit2D.transform.GetComponent<IInGameInteractable>();
				interactable?.Interact();
			}
		}
	}


	private static GameInputManager _instance;
	public static GameInputManager instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<GameInputManager>();
			return _instance;
		}
	}
}
