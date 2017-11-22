using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float MaxSpeed;
	//public float JumpForce = 400;
	public float ZMax;
	public float ZMin;
	public float DamageTime = 0.5f;
	public int MaxHealth;
	public float AttackRate = 1f;

	//public GameObject AttackBox;
	//public GameObject HitBox;
	//public Sprite AttackSprite;

	private Rigidbody _rigidbody;
	private Animator _animator;
	//private Transform _groundCheck;
	//private SpriteRenderer _currentSpriteRenderer;

	private Transform _target;

	private float _currentSpeed;
	private float _zForce;
	private float _walkTimer;
	private float _damageTimer;
	private float _nextAttack;

	private int _currentHealth;

	private bool _onGround;
	private bool _isDead;
	private bool _facingRight;
	private bool _canMove = true;
	private bool _jump;
	private bool _damaged;

	private int _groundLayerMask;

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		//_currentSpriteRenderer = GetComponent<SpriteRenderer>();
		//_groundCheck = gameObject.transform.Find("GroundCheck");
		_target = FindObjectOfType<PlayerController>().transform;

		_currentSpeed = MaxSpeed;
		_currentHealth = MaxHealth;

		_groundLayerMask = LayerMask.NameToLayer("Ground");

	}
	
	// Update is called once per frame
	void Update () {
		_animator.SetBool("OnGround", _onGround);
		_animator.SetBool("IsDead", _isDead);

		_facingRight = _target.position.x > transform.position.x;
		transform.eulerAngles = new Vector3(0, _facingRight ? 180 : 0, 0);

		if (_damaged && !_isDead)
		{
			_damageTimer += Time.deltaTime;
			if (_damageTimer >= DamageTime)
			{
				_damaged = false;
				_damageTimer = 0;
			}
		}

		_walkTimer += Time.deltaTime;
	}

	void FixedUpdate()
	{
		if (!_isDead)
		{
			var targetDistance = _target.position - transform.position;
			float xForce = targetDistance.x / Mathf.Abs(targetDistance.x);

			if (_walkTimer > Random.Range(1, 3))
			{
				_zForce = Random.Range(-1, 2);
				_walkTimer = 0;
			}

			if (Mathf.Abs(targetDistance.x) < 1.5f)
			{
				xForce = 0;
			}

			if (!_damaged)
			{
				_rigidbody.velocity = new Vector3(xForce * _currentSpeed, 0, _zForce * _currentSpeed);
			}
			_animator.SetFloat("Speed", Mathf.Abs(_currentSpeed));

			if (Mathf.Abs(targetDistance.x) < 1.5f && Mathf.Abs(targetDistance.z) < 1.5f && Time.time > _nextAttack)
			{
				_animator.SetBool("IsAttack", true);
				_animator.SetTrigger("Attack");
				_currentSpeed = 0;
				_nextAttack = Time.time + AttackRate;
			}
			else
			{
				_animator.SetBool("IsAttack", false);
				_currentSpeed = MaxSpeed;
			}

		}

		_rigidbody.position = new Vector3(
			_rigidbody.position.x,
			_rigidbody.position.y,
			Mathf.Clamp(_rigidbody.position.z, ZMin, ZMax)
		);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == _groundLayerMask && !_onGround)
		{
			_onGround = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer == _groundLayerMask && _onGround)
		{
			_onGround = false;
		}
	}

	void ResetSpeed()
	{
		_currentSpeed = MaxSpeed;
	}

	void SpeedToZero()
	{
		_currentSpeed = 0;
	}

	public void TookDamage(int damage)
	{
		if(_isDead) return;

		_damaged = true;
		_currentHealth -= damage;

		_animator.SetTrigger("HitDamage");

		if (_currentHealth <= 0)
		{
			_isDead = true;
			_rigidbody.AddRelativeForce(new Vector3(1,2,0), ForceMode.Impulse);

			DestroyAfterAnimation();
		}
	}

	private void DestroyAfterAnimation()
	{
		RuntimeAnimatorController ac = _animator.runtimeAnimatorController;
		float time = 0.5f + ac.animationClips.Where(clip => clip.name == "Dead").Sum(clip => clip.length);
		Destroy(gameObject, time);
	}

	public void Die()
	{
		gameObject.SetActive(false);
	}
}
