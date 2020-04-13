using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability
{
	protected Robot _target;

	public virtual void Init(Robot target)
	{
		_target = target;
	}

	public abstract void Activate();

	public abstract void Stop();
}
