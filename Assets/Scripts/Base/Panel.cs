using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
	[SerializeField] protected GameObject _targetGO;
	protected GameObject _toggleGO
	{
		get
		{
			if (_targetGO == null)
				return gameObject;

			return _targetGO;
		}
	}

	private void Awake()
	{

	}

	public void Toggle(bool show)
	{
		if (_toggleGO.activeInHierarchy == show)
			return;

		_toggleGO.SetActive(show);
	}
}
