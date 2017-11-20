using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
	public float ZMax;
	public float ZMin;
	public GameObject[] Enemies;
	public int EnemiesCount;
	public float SpawnTime;

	private int _currentEnemiesCount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (_currentEnemiesCount >= EnemiesCount)
		{
			var enemies = FindObjectsOfType<EnemyController>().Length;
			if (enemies <= 0)
			{
				// value should be equals maxXY a default value of the CameraFollow
				var cameraFollow = FindObjectOfType<CameraFollow>();
				cameraFollow.maxXY.x = cameraFollow.defaultX;
				gameObject.SetActive(false);
			}
		}

	}
	
	void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			GetComponent<BoxCollider>().enabled = false;
			FindObjectOfType<CameraFollow>().maxXY.x = transform.position.x;
			SpawnEnemy();
		}
	}

	private void SpawnEnemy()
	{
		var spawnForward = Random.Range(0, 5) == 0;
		var x = spawnForward ? transform.position.x + 3 : transform.position.x - 3;
		var z = Random.Range(ZMin, ZMax);
		var spawnPosition = new Vector3(x, 0, z);

		Instantiate(Enemies[Random.Range(0, Enemies.Length)], spawnPosition, Quaternion.identity);
		_currentEnemiesCount++;

		if (_currentEnemiesCount <= EnemiesCount)
		{
			Invoke("SpawnEnemy", SpawnTime);
		}
	}
}
