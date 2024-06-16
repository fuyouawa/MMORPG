//
//NOTES:
//
//I recommend everyone to create their own code for their own projects.
//THIS IS JUST A BASIC EXAMPLE PUT TOGETHER TO DEMONSTRATE VFX ASSETS.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnAbilities : MonoBehaviour {

	[System.Serializable]
	public class ShakeParameters {
		public bool shake;
		public List<float> delays = new List<float>();
	}

	public Text effectName;
	public GameObject cameras;
	public List<GameObject> VFXs = new List<GameObject> ();
	public List<ShakeParameters> VFXsShakeParameters = new List<ShakeParameters> ();

	private int count = 0;
	private GameObject effectToSpawn;
	private ShakeParameters effectShakeParameters;
	private List<Camera> camerasList = new List<Camera> ();
	private Camera singleCamera;

	void Start () {

		if (cameras.transform.childCount > 0) {
			for (int i = 0; i < cameras.transform.childCount; i++) {
				camerasList.Add (cameras.transform.GetChild (i).gameObject.GetComponent<Camera> ());
			}
			if(camerasList.Count == 0){
				Debug.Log ("Please assign one or more Cameras in inspector");
			}
		} else {
			singleCamera = cameras.GetComponent<Camera> ();
			if (singleCamera != null)
				camerasList.Add (singleCamera);
			else
				Debug.Log ("Please assign one or more Cameras in inspector");
		}

		if (VFXs.Count > 0)
			effectToSpawn = VFXs [0];
		else
			Debug.Log ("No Effects added to the VFXs List");
		
		if(VFXsShakeParameters.Count > 0)
			effectShakeParameters = VFXsShakeParameters [0];
		else
			Debug.Log ("No Delays added to the ShakeDelays List");
		
		if (effectName != null) 
			effectName.text = effectToSpawn.name;
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown (0) )
			SpawnVFX ();
		if (Input.GetKeyDown (KeyCode.D))
			Next ();
		if (Input.GetKeyDown (KeyCode.A)) 
			Previous ();	
		if (Input.GetKeyDown (KeyCode.C))
			SwitchCamera ();	
		if (Input.GetKeyDown (KeyCode.X))
			ZoomIn ();
		if (Input.GetKeyDown (KeyCode.Z))
			ZoomOut ();
	}

	public void SpawnVFX () {
		GameObject vfx;

		if (effectShakeParameters != null)
		{
			if (effectShakeParameters.shake && cameras != null)
				StartCoroutine(ShakeDelay(effectShakeParameters.delays));
		}

		vfx = Instantiate (effectToSpawn);

		var ps = GetFirstPS (vfx);

		Destroy (vfx, ps.main.duration + ps.main.startLifetime.constantMax + 1);
	}

	public void Next () {
		count++;

		if (count > VFXs.Count)
			count = 0;

		for(int i = 0; i < VFXs.Count; i++){
			if (count == i) {
				effectToSpawn = VFXs [i];
                if (effectShakeParameters != null)
                    effectShakeParameters = VFXsShakeParameters [i];
			}
			if (effectName != null)	effectName.text = effectToSpawn.name;
		}
	}

	public void Previous () {
		count--;

		if (count < 0)
			count = VFXs.Count;

		for (int i = 0; i < VFXs.Count; i++) {
			if (count == i) {
				effectToSpawn = VFXs [i];
                if (effectShakeParameters != null)
                    effectShakeParameters = VFXsShakeParameters [i];
			}
			if (effectName != null)	effectName.text = effectToSpawn.name;
		}
	}

	public void ZoomIn () {
		if (camerasList.Count > 0) {
			if (!camerasList [0].orthographic) {
				if (camerasList [0].fieldOfView < 101) {
					for (int i = 0; i < camerasList.Count; i++) {
						camerasList [i].fieldOfView += 5;
					}
				}
			} else {
				if (camerasList [0].orthographicSize < 10) {
					for (int i = 0; i < camerasList.Count; i++) {
						camerasList [i].orthographicSize += 0.5f;
					}
				}
			}
		}
	}

	public void ZoomOut () {
		if (camerasList.Count > 0) {
			if (!camerasList [0].orthographic) {
				if (camerasList [0].fieldOfView > 20) {
					for (int i = 0; i < camerasList.Count; i++) {
						camerasList [i].fieldOfView -= 5;
					}
				}
			} else {
				if (camerasList [0].orthographicSize > 4) {
					for (int i = 0; i < camerasList.Count; i++) {
						camerasList [i].orthographicSize -= 0.5f;
					}
				}
			}
		}
	}

	public void SwitchCamera () {
		if (camerasList.Count > 0) {
			for (int i = 0; i < camerasList.Count; i++) {
				if (camerasList [i].gameObject.activeSelf) {
					camerasList [i].gameObject.SetActive (false);
					if ((i + 1) == camerasList.Count) {
						camerasList [0].gameObject.SetActive (true);
						break;
					} else {
						camerasList [i + 1].gameObject.SetActive (true);
						break;
					}
				}
			}
		}
	}

	public ParticleSystem GetFirstPS (GameObject vfx){
		var ps = vfx.GetComponent<ParticleSystem> ();
		if (ps == null && vfx.transform.childCount > 0) {
			foreach (Transform t in vfx.transform) {
				ps = t.GetComponent<ParticleSystem> ();
				if(ps != null)
					return ps;
			}
		}
		return ps;
	}

	IEnumerator ShakeDelay (List<float> delay){
		for (int i = 0; i < delay.Count; i++) {
			yield return new WaitForSeconds (delay[i]);
			cameras.GetComponent<SimpleCameraShake> ().ShakeCamera ();
		}
	}
}
