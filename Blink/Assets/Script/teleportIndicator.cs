//Author: Gabriel E. Casillas
//Description: This program will help the player teleport by indicating with an image
//if a teleport can be achieved.

using UnityEngine;
using System.Collections;

public class teleportIndicator : MonoBehaviour {

	public static RaycastHit hit;

	public Texture2D canT;
	public Texture2D cannotT;

	public bool canTeleport;

	public Vector3 mousePos;
	public Vector3 mousePos2;

	public float distance;

	public GameObject thePlayer;

	public Transform playerTransform;
	public Vector3 position;

	// Use this for initialization
	void Start () 
	{
		thePlayer = GameObject.Find ("Blink");
		playerTransform = thePlayer.transform;
		position = playerTransform.position;

		canTeleport = true;

		distance = 10.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mousePos = Input.mousePosition;

		playerTransform = thePlayer.transform;
		position = playerTransform.position;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit) && hit.transform.name=="Background")
		{
			if(position.x - hit.point.x < distance && position.x - hit.point.x > (distance * -1))
			{
				if(position.y - hit.point.y < distance && position.y - hit.point.y > (distance * -1))
				{
					canTeleport = true;
				}
				else
				{
					canTeleport = false;
				}
			}
			else
			{
				canTeleport = false;
			}
		}
		else
		{
			canTeleport = false;
		}

		mousePos2.y = Screen.height - mousePos.y;
	}

	void OnGUI()
	{
		if(canTeleport == true)
		{
			GUI.DrawTexture(new Rect(mousePos.x - 16,mousePos2.y - 16,32,32),canT);
		}
		else
		{
			GUI.DrawTexture(new Rect(mousePos.x - 16,mousePos2.y - 16,32,32),cannotT);
		}
	}
}
