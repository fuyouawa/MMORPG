//
//NOTES:
//
//I recommend everyone to create their own code for their own projects.
//THIS IS JUST A BASIC EXAMPLE PUT TOGETHER TO DEMONSTRATE VFX ASSETS.
//

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraShake : MonoBehaviour {

	private bool isRunning = false;
	private Animation anim;

	void Start () {
		anim = GetComponent<Animation> ();
	}

	public void ShakeCamera() {	
		if (anim != null)
			anim.Play (anim.clip.name);
		else
			ShakeCaller (0.25f, 0.1f);
	}

	//other shake option
	public void ShakeCaller (float amount, float duration){
		StartCoroutine (Shake(amount, duration));
	}

	IEnumerator Shake (float amount, float duration){
		isRunning = true;

		Vector3 originalPos = transform.localPosition;
		int counter = 0;

		while (duration > 0.01f) {
			counter++;

			var x = Random.Range (-1f, 1f) * (amount/counter);
			var y = Random.Range (-1f, 1f) * (amount/counter);

			transform.localPosition = Vector3.Lerp (transform.localPosition, new Vector3 (originalPos.x + x, originalPos.y + y, originalPos.z), 0.5f);

			duration -= Time.deltaTime;
			
			yield return new WaitForSeconds (0.1f);
		}

		transform.localPosition = originalPos;

		isRunning = false;
	}
}
