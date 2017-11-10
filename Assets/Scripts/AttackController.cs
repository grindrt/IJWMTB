using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

	public int Damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider)
	{
		var enemy = collider.GetComponent<EnemyController>();
		var player = collider.GetComponent<PlayerController>();
		
		if (enemy != null)
		{
			enemy.TookDamage(Damage);
		}

		if (player != null)
		{
			player.TookDamage(Damage);
		}
	}
}
