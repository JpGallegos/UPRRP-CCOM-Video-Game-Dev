﻿//Author: Gabriel E. Casillas
//Description: This program controls the main aspects of the level. It will keep count of
//any score, life, or other quantities as well as play the song of the level. It will also
//be in charge of pause features and in-game GUI displays.

using UnityEngine;
using System.Collections;

public class levelControl : MonoBehaviour {

	//These will help us "talk" with the object that controls music
	public GameObject theSong;
	musicManager song;

	// Use this for initialization
	void Start() 
	{
		//Looks for the object named Music so we can send messages and prompt actions in that object
		theSong = GameObject.Find ("Music");
		song = theSong.GetComponent<musicManager>();

		//We send a message to the music controller and tell it to play the first song
		song.PlaySound(song.level1);
	}
	
	// Update is called once per frame
	void Update() {
	
	}
}
