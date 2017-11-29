using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
	public AudioClip LevelSoung;
	private AudioSource _audioSource;

	// Use this for initialization
	void Start ()
	{
		_audioSource = GetComponent<AudioSource>();
		PlayAudioClip(LevelSoung);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayAudioClip(AudioClip clip)
	{
		_audioSource.clip = clip;
		_audioSource.Play();
	}
}
