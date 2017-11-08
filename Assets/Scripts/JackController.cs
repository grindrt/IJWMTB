using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class JackController : MonoBehaviour
{
	public float MaxSpeed = 8;
	public float JumpForce = 400;
	public float ZMax;
	public float ZMin;

	public GameObject AttackBox;
	public Sprite AttackSprite;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private Transform _groundCheck;
	private SpriteRenderer _currentSpriteRenderer;

	private float _currentSpeed;

	public bool _onGround;
	private bool _isDead;
	private bool _facingRight = true;
	private bool _canMove = true;
	private bool _jump;

	private int _groundLayerMask;

	// Use this for initialization
	void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_currentSpriteRenderer = GetComponent<SpriteRenderer>();
		_groundCheck = gameObject.transform.Find("GroundCheck");

		_currentSpeed = MaxSpeed;

		_groundLayerMask = LayerMask.NameToLayer("Ground");
	}

	// Update is called once per frame
	void Update ()
	{
		_animator.SetBool("OnGround", _onGround);
		_animator.SetBool("IsDead", _isDead);

		_jump = Input.GetButtonDown("Jump") && _onGround;

		_animator.SetBool("IsAttack", Input.GetButtonDown("Fire1"));
		AttackBox.SetActive(AttackSprite == _currentSpriteRenderer.sprite);
	}

	void FixedUpdate()
	{
		if (_isDead) return;

		if (_jump)
		{
			_jump = false;
			_rigidbody.AddForce(Vector3.up * JumpForce);
		}

		float x = Input.GetAxis("Horizontal");
		float z = _onGround ? Input.GetAxis("Vertical") : 0;
		
		_rigidbody.velocity = new Vector3(x * _currentSpeed, _rigidbody.velocity.y, z * _currentSpeed);

		if (_onGround)
		{
			_animator.SetFloat("Speed", Mathf.Abs(_rigidbody.velocity.magnitude));
		}

		if ((x > 0 && !_facingRight && _canMove) || (x < 0 && _facingRight && _canMove))
		{
			Flip();
		}

		var xMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x;
		var xMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10)).x;
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
}
