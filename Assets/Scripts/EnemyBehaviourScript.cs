using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Timer = System.Timers.Timer;

public class EnemyBehaviourScript : MonoBehaviour
{
	public float MaxSpeed;
	public float ZMax;
	public float ZMin;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private Transform _groundCheckTransform;
	private Transform _target;
	private bool _onGround;
	private bool _isDead;
	private bool _isBlocking;
	private bool _canMove = true;
	private bool _isRight = true;
	private float _currentSpeed;
	private float _zForce;
	private float _walkTimer;

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_groundCheckTransform = gameObject.transform.Find("GroundCheck");
		_target = FindObjectOfType<PlayerBehaviourScript>().transform;
		//_currentSpeed = MaxSpeed;

	}
	
	// Update is called once per frame
	void Update () {

		_onGround = Physics.Linecast(transform.position, _groundCheckTransform.position, 1 << LayerMask.NameToLayer("Ground"));

		_animator.SetBool("OnGround", _onGround);
		_animator.SetBool("IsDead", _isDead);
		
		_isRight = _target.position.x >= transform.position.x;

		transform.eulerAngles = _isRight ? new Vector3(0,180,0) : new Vector3(0,0,0);

		_walkTimer += Time.deltaTime;
	}

	private void FixedUpdate()
	{
		if (!_isDead)
		{
			var targetDistance = _target.position - transform.position;
			float xForce = targetDistance.x / Mathf.Abs(targetDistance.x);

			if (_walkTimer >= Random.Range(1f, 2f))
			{
				_zForce = Random.Range(-1, 2);
				_walkTimer = 0;
			}

			if (Mathf.Abs(targetDistance.x) < 1.5f)
			{
				xForce = 0;
			}

			_rigidbody.velocity = new Vector3(xForce * _currentSpeed, 0, _zForce * _currentSpeed);

			_animator.SetFloat("Speed", Mathf.Abs(_currentSpeed));
		}

		_rigidbody.position = new Vector3(
			_rigidbody.position.x,
			_rigidbody.position.y,
			Mathf.Clamp(_rigidbody.position.z, ZMin, ZMax)
		);
	}

	private void SetToZeroSpeed()
	{
		_currentSpeed = 0;
	}

	private void ResetSpeed()
	{
		_currentSpeed = MaxSpeed;
	}
}
