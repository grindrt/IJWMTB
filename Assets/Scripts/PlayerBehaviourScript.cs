using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
	public float MaxSpeed = 2;
	public float JumpForce = 200;
	public float ZMax;
	public float ZMin;

	private Rigidbody _rigidbody;
	private Animator _animator;
	private Transform _groundCheckTransform;
	private bool _onGround;
	private bool _isDead;
	private bool _isBlocking;
	private bool _canMove = true;
	private bool _isRight = true;

	private float _currentSpeed;
	private bool _jump;

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_groundCheckTransform = gameObject.transform.Find("GroundCheck");
		_currentSpeed = MaxSpeed;
	}

	// Update is called once per frame
	void Update()
	{
		_onGround = Physics.Linecast(transform.position, _groundCheckTransform.position, 1 << LayerMask.NameToLayer("Ground"));

		_animator.SetBool("OnGround", _onGround);
		_animator.SetBool("IsDead", _isDead);

		if (Input.GetButtonDown("Jump") && _onGround)
		{
			_jump = true;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			_animator.SetTrigger("Attack");
		}
	}

	private void FixedUpdate()
	{
		if (_isDead) return;

		float mHorizontal = Input.GetAxis("Horizontal"); 
		float mVertical = _onGround ? Input.GetAxis("Vertical") : 0; 
			
		_rigidbody.velocity = new Vector3(mHorizontal * _currentSpeed, _rigidbody.velocity.y, mVertical * _currentSpeed);

		if (_onGround)
		{
			_animator.SetFloat("Speed", Mathf.Abs(_rigidbody.velocity.magnitude));
		}

		if ((mHorizontal > 0 && !_isRight && _canMove) || (mHorizontal < 0 && _isRight && _canMove))
		{
			FLip();
		}

		if (_jump)
		{
			_jump = false;

			_rigidbody.AddForce(Vector3.up * JumpForce);
			_animator.SetTrigger("Jump");
		}

		var xMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x;
		var xMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10)).x;
		_rigidbody.position = new Vector3(
				Mathf.Clamp(_rigidbody.position.x, xMin + 1, xMax - 1),
				_rigidbody.position.y, 
				Mathf.Clamp(_rigidbody.position.z, ZMin, ZMax)
			);
	}

	private void FLip()
	{
		_isRight = !_isRight;
		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
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
