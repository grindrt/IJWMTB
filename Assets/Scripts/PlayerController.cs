using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float MaxSpeed = 8;
	public float JumpForce = 400;
	public float ZMax;
	public float ZMin;
	public float XMin;
	public int MaxHealth = 100;

	//public GameObject AttackBox;
	//public Sprite AttackSprite;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private UIController _ui;
	//private Transform _groundCheck;
	//private SpriteRenderer _currentSpriteRenderer;

	private float _currentSpeed;

	private bool _onGround;
	private bool _isDead;
	private bool _facingRight = true;
	private bool _canMove = true;
	private bool _jump;
	private bool _isBlocking;
	private float _blockRatio = 1;

	private int _groundLayerMask;
	private int _currentHealth;

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_ui = FindObjectOfType<UIController>();
		//_currentSpriteRenderer = GetComponent<SpriteRenderer>();
		//_groundCheck = gameObject.transform.Find("GroundCheck");

		_currentSpeed = MaxSpeed;

		_groundLayerMask = LayerMask.NameToLayer("Ground");

		_currentHealth = MaxHealth;
	}

	// Update is called once per frame
	void Update ()
	{
		_animator.SetBool("OnGround", _onGround);
		_animator.SetBool("IsDead", _isDead);

		_jump = Input.GetButtonDown("Jump") && _onGround;

		//_animator.SetBool("IsAttack", Input.GetButtonDown("Fire1"));

		if (Input.GetButtonDown("Fire1"))
		{
			_animator.SetTrigger("Attack");
		}
	}

	void FixedUpdate()
	{
		if (_isDead) return;

		float x = Input.GetAxis("Horizontal");
		float z = _onGround ? Input.GetAxis("Vertical") : 0;
		
		_rigidbody.velocity = new Vector3(x * _currentSpeed, _rigidbody.velocity.y, z * _currentSpeed);

		if (_onGround)
		{
			_animator.SetFloat("Speed", Mathf.Abs(_rigidbody.velocity.magnitude));
		}

		_isBlocking = Input.GetKey(KeyCode.LeftAlt);
		_animator.SetBool("Block", _isBlocking);
		_blockRatio = _isBlocking ? 0.25f : 1;

		if ((x > 0 && !_facingRight && _canMove) || (x < 0 && _facingRight && _canMove))
		{
			Flip();
		}

		if (_jump)
		{
			_jump = false;
			_rigidbody.AddForce(Vector3.up * JumpForce);
		}

		var xMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x - 0.7f;
		var xMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10)).x + 0.7f;
		_rigidbody.position = new Vector3(
			Mathf.Clamp(_rigidbody.position.x, xMin + 1, xMax - 1),
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

	private void Flip()
	{
		_facingRight = !_facingRight;
		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
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
		if (_isDead) return;

		//_damaged = true;
		_currentHealth -= Mathf.RoundToInt(damage * _blockRatio);
		_ui.UpdateHealthBar(_currentHealth);

		_animator.SetTrigger("HitDamage");

		if (_currentHealth <= 0)
		{
			_isDead = true;
			_rigidbody.AddRelativeForce(new Vector3(3, 5, 0), ForceMode.Impulse);
		}
	}
}
