using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

	public Slider HealthBar;

	private PlayerController _player;

	// Use this for initialization
	void Start ()
	{
		_player = FindObjectOfType<PlayerController>();
		HealthBar.maxValue = _player.MaxHealth;
		HealthBar.value = _player.MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateHealthBar(int value)
	{
		HealthBar.value = value;
	}
}
