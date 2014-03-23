using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour {

//	public float speed;
//	public Vector3 direction;

	void Start(){
		Destroy (gameObject, 3f);
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("Hit something! - " + collision.gameObject.tag);
//		if (collision.gameObject.tag != "Antivirus") {
			if (collision.gameObject.tag == "Player") {
				Debug.Log ("Projectile hit the player!");
			}
			Destroy (gameObject);
//		}
	}
}
