﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Robot : MonoBehaviour, IInGameInteractable
{
	#region Events

	public static event UnityAction<Robot> onlineEvent;
	public static event UnityAction<Robot> offlineEvent;

	#endregion

	private GameObject _mGO;
	public GameObject myGO
	{
		get
		{
			if (_mGO == null)
				_mGO = gameObject;

			return _mGO;
		}
	}

	private Collider2D _mCollider;
	public Collider2D myCollider
	{
		get
		{
			if (_mCollider == null)
				_mCollider = GetComponent<Collider2D>();

			return _mCollider;
		}
	}

	[Header("Stats")]
	[SerializeField] private int _hitpoints = 1;
	public int hitpoints
	{
		get
		{
			return _hitpoints;
		}

		private set
		{
			_hitpoints = value;
		}
	}

	private Health[] _healthList;

	[SerializeField] private float _colourChangeInterval = 1;
	[SerializeField] private int _unselectableOfflineCount = -1;
	public int unselectableOfflineCount
	{
		get
		{
			return _unselectableOfflineCount;
		}
	}

	public bool selectable
	{
		get
		{
			return unselectableOfflineCount == -1 || unselectableOfflineCount != MatchManager.selectedCount;
		}
	}

	private Transform _mTF;
	private Transform _myTF
	{
		get
		{
			if (_mTF == null)
				_mTF = transform;
			return _mTF;
		}
	}

	[SerializeField]
	private float _rotateSpeed = 5;
	private Vector3 _rotation = Vector3.forward;

	[Header("Abilities")]
	[SerializeField]
	private Reboot _reboot;

	[Space]
	[Space]

	[SerializeField]
	private List<Color> _colors = new List<Color>
	{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.green
	};

	[SerializeField]
	private Collider2D _selectableCollider;
	[SerializeField]
	private GameObject _blockerGO;

	[SerializeField]
	private int _cIndex = 0;
	private int _colorIndex
	{
		get
		{
			return _cIndex;
		}

		set
		{
			if (value >= _colors.Count)
				value = 0;
			_cIndex = value;
		}
	}

	public Color currentColor
	{
		get
		{
			return _colors[_colorIndex];
		}
	}

	private int _spriteOrderIndex;

	private SpriteRenderer[] _sRenderers;
	private SpriteRenderer[] _spriteRenderers
	{
		get
		{
			if (_sRenderers == null)
			{
				_sRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			}
			return _sRenderers;
		}
	}

	private SpriteRenderer _sRenderer;
	private SpriteRenderer _mySpriteRenderer
	{
		get
		{
			if (_sRenderer == null)
			{
				_sRenderer = transform.Find("Model/Color").GetComponent<SpriteRenderer>();
				//_sRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
			}
			return _sRenderer;
		}
	}

	[SerializeField]
	private Movement.MovementData movementInfo;
	public Movement movePattern { get; private set; }

	public bool offline { get; private set; } = true;
	public bool isDead
	{
		get
		{
			return hitpoints <= 0;
		}
	}

	private void Awake()
	{
		ToggleCollider(false);
		Robot.onlineEvent += InvokeOnline;
		Robot.offlineEvent += InvokeOffline;
		MatchManager.matchClearedEvent += SetSelectable;
		_myTF.Rotate(Vector3.forward, Random.Range(0, 360));
	}

	private void OnDestroy()
	{
		Robot.onlineEvent -= InvokeOnline;
		Robot.offlineEvent -= InvokeOffline;
		MatchManager.matchClearedEvent -= SetSelectable;
	}

	private void InvokeOnline(Robot robot)
	{
		if (robot != this)
			SetSelectable();
	}

	private void InvokeOffline(Robot robot)
	{
		if (robot != this)
			SetSelectable();
	}

	private EnemyInfo enemyInfo;
	public void Init(EnemyInfo enemyInfo, Vector3 spawnPosition, int sortingOrder)
	{
		_spriteOrderIndex = sortingOrder;
		_rotation = Random.value > 0.5f ? Vector3.forward : Vector3.back;
		this.enemyInfo = enemyInfo;
		hitpoints = enemyInfo.health;
		_unselectableOfflineCount = enemyInfo.unselectableOfflineCount;

		EnemyManager.Register(this);
		_colorIndex = Random.Range(0, _colors.Count);
		_reboot.Init(this);

		movementInfo.movementType = enemyInfo.movementType;
		if (movementInfo.movementType != Movement.MovementType.None)
		{
			movePattern = Movement.Create(myGO, movementInfo);
			movePattern.Init(myGO);
		}

		_healthList = GetComponentsInChildren<Health>();

		SetOrtingOrder(_spriteOrderIndex);
		ChangeColor();
		SetSelectable(true);
		SpawnSequence(spawnPosition);
	}

	private IEnumerator _changeColourCoroutine;
	public void StartChangeColor()
	{
		SetSelectable();
		StopColorChange();
		_changeColourCoroutine = ChangeColorCoroutine();
		StartCoroutine(_changeColourCoroutine);

		if (movementInfo.movementType != Movement.MovementType.None)
			movePattern.Move();
	}

	private void StopColorChange()
	{
		if (_changeColourCoroutine != null)
			StopCoroutine(_changeColourCoroutine);

		if (movementInfo.movementType != Movement.MovementType.None)
			movePattern.Pause();
	}

	private IEnumerator ChangeColorCoroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(_colourChangeInterval);
			_colorIndex++;
			ChangeColor();
		}
	}

	public void Online(bool broadcast = true)
	{
		if (!offline)
			return;

		if (broadcast)
			onlineEvent?.Invoke(this);

		SetOrtingOrder(_spriteOrderIndex);
		offline = false;
		StartChangeColor();
		//SetSelectable(true);
		LeanTween.scale(myGO, Vector3.one, 0.25f);
	}

	public void Offline(bool broadcast = true)
	{
		if (offline)
			return;

		if (broadcast)
			offlineEvent?.Invoke(this);

		SetOrtingOrder();
		offline = true;
		SetSelectable();
		StopColorChange();

		LeanTween.scale(myGO, Vector3.one * 0.8f, 0.25f);
		//if (_reboot != null)
		//	_reboot.Activate();
	}

	private void SetOrtingOrder(int sortingOrder = 0)
	{
		for (int i = 0; i < _spriteRenderers.Length; i++)
		{
			_spriteRenderers[i].sortingOrder = i == 0 ? sortingOrder : i == 2 ? sortingOrder + 3 : sortingOrder + 2;
		}
	}

	private void ChangeColor()
	{
		ChangeColor(_colors[_colorIndex]);
	}

	private void ChangeColor(Color color)
	{
		for (int i = 0; i < _spriteRenderers.Length; i++)
		{
			if (!_selectableCollider.enabled)
				_spriteRenderers[i].color = Color.black;
			else
				_spriteRenderers[i].color = color;

		}
		//_mySpriteRenderer.color = color;
	}

	public void Hurt()
	{
		hitpoints--;
		Shake(ApplyDamage);
	}

	private void ApplyDamage()
	{
		if (isDead)
		{
			DeathSequence();
		}
		else
		{
			for (int i = 0; i < _healthList.Length; i++)
			{
				if (_healthList[i].isActive)
				{
					_healthList[i].Toggle(false);
					break;
				}
			}

			Online();
		}
	}

	private void SpawnSequence(Vector3 spawnPosition)
	{
		LTSeq alphaSeq = LeanTween.sequence();
		alphaSeq.append(LeanTween.alpha(myGO, 0, 0));
		alphaSeq.append(() =>
		{
			_myTF.position = spawnPosition;
		});
		alphaSeq.append(LeanTween.alpha(myGO, 1, 0.4f));
		alphaSeq.append(() =>
		{
			ToggleCollider(true);
			Online(false);
		});
	}

	private void DeathSequence()
	{
		float scaleTime = 0.1f;
		LeanTween.alpha(myGO, 0, 0.4f);
		LTSeq tweenSeq = LeanTween.sequence();
		tweenSeq.append(LeanTween.scale(myGO, Vector3.one, scaleTime));
		tweenSeq.append(LeanTween.scale(myGO, Vector3.one * 0.8f, scaleTime));
		tweenSeq.append(LeanTween.scale(myGO, Vector3.one, scaleTime));
		tweenSeq.append(LeanTween.scale(myGO, Vector3.one * 0.8f, scaleTime));
		tweenSeq.append(() =>
		{
			Destroy(myGO);
		});
	}

	public void ToggleCollider(bool e)
	{
		myCollider.enabled = e;
	}

	public void SetSelectable()
	{
		ToggleSelectable(selectable);
	}

	public void SetSelectable(bool force)
	{
		ToggleSelectable(selectable, force);
	}

	public void Toggle(bool e)
	{
		myGO.SetActive(e);
	}

	public void ToggleSelectable(bool sel, bool force = false)
	{
		if (offline && !force)
			return;

		if (_selectableCollider.enabled == sel)
			return;

		_blockerGO.SetActive(!sel);
		_selectableCollider.enabled = sel;
		ChangeColor();
	}

	private void Shake(UnityAction callBack = null)
	{
		Vector3 pos = _myTF.position;
		Vector3 shakePos = new Vector3(0.1f, 0, 0);
		LTSeq tweenSeq = LeanTween.sequence();
		tweenSeq.append(LeanTween.move(myGO, pos + shakePos, 0.1f).setEase(LeanTweenType.easeShake));
		tweenSeq.append(LeanTween.move(myGO, pos, 0.1f).setEase(LeanTweenType.easeShake));
		tweenSeq.append(() =>
		{
			callBack?.Invoke();
		});
	}

	void IInGameInteractable.Interact()
	{
		if (selectable)
			MatchManager.instance.Add(this);
	}

	private void Update()
	{
		if (!offline)
			_myTF.Rotate(_rotation, Time.deltaTime * _rotateSpeed);
	}
}
