using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
	public float WalkMovementSpeed;
	public float AttackMovementSpeed;
	public float XMin, XMax, ZMin, ZMax;
	public float KnockBackForce;
	public float FallTime;
	public GameObject AttackGameObject;
	public Sprite AttackSpriteFrame;
	
	private Rigidbody _rigidbody;
	private Animator _animator;
	private AnimatorStateInfo _currentStateInfo;
	private SpriteRenderer _currentSpriteRenderer;

	private float _movementSpeed;
	private bool _isBlocking;
	private bool _canMove = true;
	private bool _isRight = true;

	private static int _currentState;
	private static readonly int IdleState = Animator.StringToHash("Base Layer.Idle");
	private static readonly int WalkState = Animator.StringToHash("Base Layer.Walk");
	private static readonly int JumpState = Animator.StringToHash("Base Layer.Jump");
	private static readonly int AttackState = Animator.StringToHash("Base Layer.Attack");
	private static readonly int DamagedState = Animator.StringToHash("Base Layer.Damaged");
	private static readonly int BlockState = Animator.StringToHash("Base Layer.Block");
	private static readonly int FallState = Animator.StringToHash("Base Layer.Fall");
	private static readonly int ShootState = Animator.StringToHash("Base Layer.Shoot");

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_currentSpriteRenderer = GetComponent<SpriteRenderer>();
		_movementSpeed = WalkMovementSpeed;
	}

	// Update is called once per frame
	void Update()
	{
		_currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		_currentState = _currentStateInfo.fullPathHash;

		_movementSpeed = _currentState == IdleState || _currentState == WalkState
			? WalkMovementSpeed
			: AttackMovementSpeed;
	}

	void FixedUpdate()
	{
		float mHorizontal = Input.GetAxis("Horizontal"); // A D
		float mVertical = Input.GetAxis("Vertical"); // W S

		var movement = new Vector3(mHorizontal, 0.0f, mVertical);

		_rigidbody.velocity = movement * _movementSpeed;
		_rigidbody.position = new Vector3(
			Mathf.Clamp(_rigidbody.position.x, XMin, XMax),
			transform.position.y,
			Mathf.Clamp(_rigidbody.position.z, ZMin, ZMax)
		);

		if ((mHorizontal > 0 && !_isRight && _canMove) || (mHorizontal < 0 && _isRight && _canMove))
		{
			FLip();
		}

		_animator.SetFloat("Speed", _rigidbody.velocity.sqrMagnitude);

		Attack();

		Block();

		// Hit by Q button
		if (Input.GetKeyDown(KeyCode.Q))
		{
			_animator.SetBool("IsDamaged", true);
		}
		else
		{
			_animator.SetBool("IsDamaged", false);
		}

		//Fall by E button
		if (Input.GetKeyDown(KeyCode.E))
		{
			StartCoroutine(FallCorountine());
		} 
	}

	private void Block()
	{
		_isBlocking = Input.GetKey(KeyCode.X);
		_animator.SetBool("Block", _isBlocking);
	}

	private void Attack()
	{
		bool attack = Input.GetButtonDown("Fire1");
		_animator.SetBool("IsAttack", attack);

		AttackGameObject.SetActive(AttackSpriteFrame == _currentSpriteRenderer.sprite);
	}

	private IEnumerator FallCorountine()
	{
		_animator.Play("Fall");
		_canMove = false;

		if (_isRight)
			_rigidbody.AddForce(transform.right * (-1 * KnockBackForce));
		else
			_rigidbody.AddForce(transform.right * KnockBackForce);
		
		yield return new WaitForSeconds(FallTime);

		_animator.Play("Idle");
		_canMove = true;
	}

	private void FLip()
	{
		_isRight = !_isRight;
		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
