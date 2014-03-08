//Author: Gabriel E. Casillas
//Description: This program deals with the movement and teleporting features of the main Character.
//Since both characters (the virus and Blink) have the same abilities, it can be used for both.

using UnityEngine;
using System.Collections;

public class ballTeleport : MonoBehaviour {

	//This timer is used to determine how much time will the teleport animation take
	public float teleTimer;

	//These variables each will be used to capture the oroginal scale of the character (x, y and z respectively)
	public float oScaleX;
	public float oScaleY;
	public float oScaleZ;

	//This will help us determine if the teleport can be achieved
	public bool canTeleport;

	//These flag variables will be used to determine the progress of the teleport
	public bool isTeleporting;
	public bool firstStep;
	public bool secondStep;

	//Creates a "ray" and gets the information of what it hits 
	public static RaycastHit hit;

	//These will help us "talk" with the object that controls sounds
	public GameObject theSound;
	soundManager sound;

	//These will connect the player with the "teleport help indicator"
	public GameObject helpTel;
	teleportIndicator help;

	// This function will start as soon as the object is created
	void Start () 
	{
		//Looks for the object named Sounds so we can send messages and prompt actions in that object
		theSound = GameObject.Find ("Sounds");
		sound = theSound.GetComponent<soundManager>();

		//Looks for the object named Teleport Help so we can send messages and prompt actions in that object
		helpTel = GameObject.Find ("TeleportHelp");
		help = helpTel.GetComponent<teleportIndicator>();

		//Get the actual scale of the character (will be used to be able to restore the original 
		//scale when the teleport animation is over)
		oScaleX = transform.localScale.x;
		oScaleY = transform.localScale.y;
		oScaleZ = transform.localScale.z;

		//Determine the time the teleport animation is going to last
		teleTimer = 0.1f;

		//Establish the current settings of teleports
		isTeleporting = false;
		firstStep = true; //It's in the first step by default
		secondStep = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//This will check with the "help indicator" to see if a teleport is permited
		canTeleport = help.canTeleport;

		// For Movement-------------------------------------------------------------------
		// The character will only be able to mnove while it is not in an animation state
		if(isTeleporting == false)
		{
			//If the "a" keyboard button is pressed, the character moves to his left (refer to Inputs for more information)
			if(Input.GetButton("Left"))
			{
				transform.position = new Vector3(transform.position.x - 0.05f,transform.position.y,0);
			}
			//If the "d" keyboard button is pressed, the character moves to his right (refer to Inputs for more information)
			else if(Input.GetButton("Right"))
			{
				transform.position = new Vector3(transform.position.x + 0.05f,transform.position.y,0);
			}
			else
			{
				transform.position = new Vector3(transform.position.x,transform.position.y,0);
			}
		}
		// End Movement Programming--------------------------------------------------------

		// For Teleport--------------------------------------------------------------------
		// The teleport will be activated when the player clicks the left mouse button
		if(isTeleporting == false)
		{
			if(Input.GetMouseButtonDown(0))
			{
				//We cast the "ray" at the position where the mouse is clicked and see where it hits
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				//If the "ray" hits the object named "Background" then hit.point will contain the point where it hit
				if (Physics.Raycast(ray, out hit) && hit.transform.name=="Background")
				{
					if(canTeleport == true)
					{
						//We tell the object in charge of sounds to play the sound that corresponds to teleporting
						sound.PlaySound(sound.teleporting);

						//Activate teleport animation
						isTeleporting = true;
						rigidbody.useGravity = false; //We deactivate gravity in the main object so it doesnt interfere with the teleporting process
					}
					else
					{
						//We tell the player he can't teleport with a sound.
						sound.PlaySound(sound.noTeleport);
					}
				}
				else
				{
					//We tell the player he can't teleport with a sound.
					sound.PlaySound(sound.noTeleport);
				}
			}
		}

		//This is only activated if teleport animation was previously activated
		if(isTeleporting == true)
		{
			//The first step is making the character shrink and dissapear
			if(firstStep == true)
			{
				//The time it takes for the animation to be played is determined at the Start function by variable teletimer
				//While the timer is larger than 0, make the character shrink a tenth of its size at each Update
				if(teleTimer > 0.0f)
				{
					//We substract the computer time to the timer, so it is more reliable
					teleTimer -= Time.deltaTime;
					transform.localScale = new Vector3(transform.localScale.x - transform.localScale.x/10, transform.localScale.y - transform.localScale.y/10, transform.localScale.z - transform.localScale.z/10);
				}
				//If the timer goes below 0 (and it will, since we are substracting computer time), we reset it to 0
				if(teleTimer < 0.0f)
				{
					teleTimer = 0.0f;
				}
				//Once the timer reaches 0, we make the character dissapear and move it to the hit position, 
				// and prompt the second stage of the teleport animation
				if(teleTimer == 0.0f)
				{
					transform.localScale = new Vector3(0,0,0);
					transform.position = new Vector3(hit.point.x, hit.point.y,0);
					firstStep = false;
					teleTimer = 0.1f; //We restore the timer to its initial value
					secondStep = true;
				}
			}
			//The second step of the animation, where the character appears again and resumes
			if(secondStep == true)
			{
				//The time it takes for the animation to be played is determined at the Start function by variable teletimer
				//While the timer is larger than 0, make the character recover a tenth of its size at each Update
				if(teleTimer > 0.0f)
				{
					//We substract the computer time to the timer, so it is more reliable
					teleTimer -= Time.deltaTime;
					transform.localScale = new Vector3(transform.localScale.x + oScaleX/10, transform.localScale.y + oScaleY/10, transform.localScale.z + oScaleZ/10);
				}
				//If the timer goes below 0 (and it will, since we are substracting computer time), we reset it to 0
				if(teleTimer < 0.0f)
				{
					teleTimer = 0.0f;
				}
				//Once the timer reaches 0, we restore the characters original scale and stop the teleport animation.
				//We also restore the use of gravity
				if(teleTimer == 0.0f)
				{
					isTeleporting = false;
					transform.localScale = new Vector3(oScaleX,oScaleY,oScaleZ);
					rigidbody.useGravity = true;
					secondStep = false;
					teleTimer = 0.1f; //We restore the timer to its initial value for a future teleport
					firstStep = true;
				}
			}
		}
		//For Teleport---------------------------------------------------------------------------------
	}
}
