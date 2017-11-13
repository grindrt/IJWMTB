using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float xMargin = 1f;
	public float xSmooth = 8f;
	public float ySmooth = 8f;
	public Vector2 maxXY;
	public Vector2 minXY;

	private Transform _player;
	public float defaultX;

	// Use this for initialization
	void Awake()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		defaultX = maxXY.x;
	}

	private bool CheckXMArgin()
	{
		var positionX = transform.position.x;
		return (positionX - _player.position.x) < xMargin;
	}

	// Update is called once per frame
	void Update()
	{
		TrackPlayer();
	}

	private void TrackPlayer()
	{
		float targetX = transform.position.x;

		if (CheckXMArgin())
		{
			targetX = Mathf.Lerp(transform.position.x, _player.position.x, xSmooth * Time.deltaTime);
		}

		targetX = Mathf.Clamp(targetX, minXY.x, maxXY.x);

		transform.position= new Vector3(targetX, transform.position.y, transform.position.z);
	}
}
