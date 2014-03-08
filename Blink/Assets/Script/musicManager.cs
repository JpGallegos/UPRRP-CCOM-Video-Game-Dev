//Author: Gabriel E. Casillas
//Description: This program deals with any music that would be found in game. Any program that
//wants to play a song would have to "talk" to this program and request a certain song to be
//played.

using UnityEngine;
using System.Collections;

public class musicManager : MonoBehaviour {

	//We'll store the songs individually so we know how to call them later on
	//Song for first level
	public AudioClip level1;

	//This function is activated while the object is "alive"
	void Awake()
	{
		//Since we want this object to control any song that would be played
		//we want it to be persistent (which means that it won't be destroyed
		//when loading another level) so we tell it to not be destroyed when
		//loading another level.
		DontDestroyOnLoad(transform.gameObject);
	}

	//This function is public so that other programs can use it to play a song.
	public void PlaySound(AudioClip sound)
	{	
		//We stop the sound for a moment to be able to change songs (if it is needed).
		//Then we assure that the song will be looped, and play the song.
		audio.Stop();
		audio.loop = true;
		audio.clip = sound;
		audio.Play();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
