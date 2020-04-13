using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reboot : Ability
{
	[SerializeField]
	protected float _rebootTime = 3f;

	public override void Activate()
	{
		Stop();
		_rebootCoroutine = RebootCoroutine(_rebootTime);
		_target.StartCoroutine(_rebootCoroutine);
	}

	public override void Stop()
	{
		if (_rebootCoroutine != null)
			_target.StopCoroutine(_rebootCoroutine);
	}

	protected IEnumerator _rebootCoroutine;
	protected IEnumerator RebootCoroutine(float rebootTime)
	{
		yield return new WaitForSeconds(rebootTime);
		_target.Online();
	}
}
