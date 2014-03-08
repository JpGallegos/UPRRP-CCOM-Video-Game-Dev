//Author: Gabriel E. Casillas
//Description: This program deals with any sound that would be found in game. Any program that
//wants to play a sound would have to "talk" to this program and request a certain sound to be
//played.

using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour {

	//We'll store the sounds individually so we know how to call them later on
	//Song when teleporting
	public AudioClip teleporting;
	public AudioClip noTeleport;

	//This function is activated while the object is "alive"
	void Awake()
	{
		//Since we want this object to control any song that would be played
		//we want it to be persistent (which means that it won't be destroyed
		//when loading another level) so we tell it to not be destroyed when
		//loading another level.
		DontDestroyOnLoad(transform.gameObject);
	}

	//This function is public so that other programs can use it to play a sound
	public void PlaySound(AudioClip sound)
	{	
		//We play the sound, and since we know it will not be looped
		//we tell it to "play just one shot" of the sound (easier way to do it).
		audio.PlayOneShot(sound);
	}
	
	void Start () 
	{

	}
	
	void Update()
	{
	
	}
}
