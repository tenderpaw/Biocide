using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	[SerializeField] private GameObject _activeGO;
	[SerializeField] private GameObject _inactiveGO;

	public bool isActive
	{
		get
		{
			return _activeGO.activeInHierarchy;
		}
	}

	public void Toggle(bool toggle = true)
	{
		_activeGO.SetActive(toggle);
		_inactiveGO.SetActive(!toggle);
	}
}
