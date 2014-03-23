using UnityEngine;
using System.Collections;

public class TurretBehavior : MonoBehaviour {
	public float FirePreparedTiming = 50f;
	public GameObject projectile;
	public float projectileSpeed;

	private float FirePreparation;
	private bool CanFire;

	// Use this for initialization
	void Start () {
		FirePreparation = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		FirePreparation += Time.deltaTime;
		if (FirePreparation >= FirePreparedTiming && !CanFire) {
			CanFire = true;
			FirePreparation = 0f;
		}
	}

	public void Fire(Vector3 direction){
		if (CanFire) {
			var bullet = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);
			Physics.IgnoreCollision(bullet.collider, gameObject.collider);
			bullet.rigidbody.velocity = direction * projectileSpeed;

			CanFire = false;
		}
	}
}
