using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
	[SerializeField] private Sprite _bgSprite;
	private SpriteRenderer _mySpriteRenderer;

	private void Awake()
	{
		_mySpriteRenderer = GetComponent<SpriteRenderer>();
		StartCoroutine(ManipulateUV());
	}

	IEnumerator ManipulateUV()
	{

		Vector2[] originalUV = _bgSprite.uv;
		float timer = 0.0f;
		float speed = 0.5f;

		while (timer < 1.0f)
		{
			for (int i = 0; i < _bgSprite.uv.Length; i++)
			{
				_bgSprite.uv[i] = Vector2.Lerp(_bgSprite.uv[i], _bgSprite.pivot, timer);
			}
			timer += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
		_mySpriteRenderer.enabled = false;
		for (int i = 0; i < _bgSprite.uv.Length; i++)
		{
			_bgSprite.uv[i] = originalUV[i];
		}
	}
}
