using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAP_ParticleSystemController{

	[System.Serializable]
	public class ParticleSystemOriginalSettings
	{
		public SerializableMinMaxGradient _startColor;
		public SerializableMinMaxGradient _colorOverLifetimeC;
		public SerializableMinMaxCurve _startSize;
		public SerializableMinMaxCurve _startSizeX;
		public SerializableMinMaxCurve _startSizeY;
		public SerializableMinMaxCurve _startSizeZ;
		public SerializableMinMaxCurve _startSpeed;
		public SerializableMinMaxCurve _startDelay;
		public SerializableMinMaxCurve _startLifetime;
		public SerializableMinMaxCurve _velocityOverLifetimeX;
		public SerializableMinMaxCurve _velocityOverLifetimeY;
		public SerializableMinMaxCurve _velocityOverLifetimeZ;
		public SerializableVector3 _localPosition;
		public SerializableGradient _trailGradient;
		public float _duration;
		public float _shapeRadius;
		public float _trailWidthMultiplier;
		public float _trailTime;
		public bool _active;
		public bool _loop;
		public bool _prewarm;
	}

	[ExecuteInEditMode]
	public class ParticleSystemController : MonoBehaviour {

		public float size = 1;
		public float speed = 1;
		public float duration = 1;
		public bool loop;
		public bool prewarm;
		public bool lights;
		public bool trails;
		public bool changeColor;
		public Color newMaxColor = new Color (0,0,0,1);
		public Color newMinColor = new Color (0,0,0,1);
		public List<GameObject> ParticleSystems = new List<GameObject>();
		public List<bool> ActiveParticleSystems = new List<bool>();

		private List<ParticleSystemOriginalSettings> psOriginalSettingsList = new List<ParticleSystemOriginalSettings> ();

		public void UpdateParticleSystem(){
			//Enables or Disbales Particle Systems you choose in inspector
			for(int i = 0; i< ParticleSystems.Count; i++){
				if (ActiveParticleSystems.Count == ParticleSystems.Count) {
					if (ActiveParticleSystems [i] == true)
						ParticleSystems [i].SetActive (true);
					else
						ParticleSystems [i].SetActive (false);
				} else {
					Debug.Log ("Make sure the ActiveParticleSystems list has the same amount as the ParticleSystems list.");
					return;
				}
			}

			if (ParticleSystems.Count > 0) {
				for (int i = 0; i < ParticleSystems.Count; i ++) {
					var ps = ParticleSystems [i].GetComponent<ParticleSystem> ();
					if (ps != null) {
						var main = ps.main;
						var shape = ps.shape;
						var psLights = ps.lights;
						var psTrails = ps.trails;
						var colorOverLifetime = ps.colorOverLifetime;
						var colorOverLifetimeC = colorOverLifetime.color;
						var startColor = main.startColor;
						var startSize = main.startSize;
						var startSizeX = main.startSizeX;
						var startSizeY = main.startSizeY;
						var startSizeZ = main.startSizeZ;
						var startSpeed = main.startSpeed;
						var startDelay = main.startDelay;
						var startLifetime = main.startLifetime;
						var velocityOverLifetime = ps.velocityOverLifetime;
						var velocityOverLifetimeX = velocityOverLifetime.x;
						var velocityOverLifetimeY = velocityOverLifetime.y;
						var velocityOverLifetimeZ = velocityOverLifetime.z;
						var localPos = ParticleSystems [i].transform.localPosition;

						//KEEP ORIGINAL VALUES
						if (!SaveParticleSystemScript.CheckExistingFile (gameObject)) {								
							ParticleSystemOriginalSettings psOriginalSettings =	new ParticleSystemOriginalSettings () {
								_startColor = new SerializableMinMaxGradient (startColor),
								_colorOverLifetimeC = new SerializableMinMaxGradient (colorOverLifetimeC),
								_startSize = new SerializableMinMaxCurve (startSize),
								_startSizeX = new SerializableMinMaxCurve (startSizeX),
								_startSizeY = new SerializableMinMaxCurve (startSizeY),
								_startSizeZ = new SerializableMinMaxCurve (startSizeZ),
								_startSpeed = new SerializableMinMaxCurve (startSpeed),
								_startDelay = new SerializableMinMaxCurve (startDelay),
								_startLifetime = new SerializableMinMaxCurve (startLifetime),
								_velocityOverLifetimeX = new SerializableMinMaxCurve (velocityOverLifetimeX),
								_velocityOverLifetimeY = new SerializableMinMaxCurve (velocityOverLifetimeY),
								_velocityOverLifetimeZ = new SerializableMinMaxCurve (velocityOverLifetimeZ),
								_localPosition = new SerializableVector3 (ParticleSystems [i].transform.localPosition),
								_duration = main.duration,
								_shapeRadius = shape.radius,
								_active = ps.gameObject.activeSelf,
								_loop = main.loop,
								_prewarm = main.prewarm
							};
							psOriginalSettingsList.Add (psOriginalSettings);
						} else {
							List<ParticleSystemOriginalSettings> listOriginalSettings = new List<ParticleSystemOriginalSettings> ();
							listOriginalSettings = SaveParticleSystemScript.LoadVFX (gameObject);

							startColor = listOriginalSettings [i]._startColor.GetMinMaxGradient();
							colorOverLifetimeC = listOriginalSettings [i]._colorOverLifetimeC.GetMinMaxGradient();
							startSize = listOriginalSettings [i]._startSize.GetMinMaxCurve();
							startSizeX = listOriginalSettings [i]._startSizeX.GetMinMaxCurve();
							startSizeY = listOriginalSettings [i]._startSizeY.GetMinMaxCurve();
							startSizeZ = listOriginalSettings [i]._startSizeZ.GetMinMaxCurve();
							startSpeed = listOriginalSettings [i]._startSpeed.GetMinMaxCurve();
							startDelay = listOriginalSettings [i]._startDelay.GetMinMaxCurve();
							startLifetime = listOriginalSettings [i]._startLifetime.GetMinMaxCurve();
							velocityOverLifetimeX = listOriginalSettings [i]._velocityOverLifetimeX.GetMinMaxCurve ();
							velocityOverLifetimeY = listOriginalSettings [i]._velocityOverLifetimeY.GetMinMaxCurve ();
							velocityOverLifetimeZ = listOriginalSettings [i]._velocityOverLifetimeZ.GetMinMaxCurve ();
							localPos = listOriginalSettings [i]._localPosition.GetVector3();
							main.duration = listOriginalSettings [i]._duration;
							shape.radius = listOriginalSettings [i]._shapeRadius;
							ps.gameObject.SetActive (listOriginalSettings [i]._active);
							loop = listOriginalSettings [i]._loop;
							prewarm = listOriginalSettings [i]._prewarm;
						}

						//LOOP
						if(!main.loop)
							main.loop = loop;

						//PREWARM
						main.prewarm = prewarm;

						//LIGHTS
						if (!lights && psLights.enabled)
							psLights.enabled = false;				

						//TRAILS
						if (!trails && psTrails.enabled)
							psTrails.enabled = false;	

						//POSITION
						if (i > 0) {
							if (localPos.x != 0 || localPos.y != 0 || localPos.z != 0) {						
								localPos.x *= size;
								localPos.y *= size;
								localPos.z *= size;
								ParticleSystems [i].transform.localPosition = localPos;
							}
						}	

						//DURATION
						if(duration != 1){
							main.duration *= duration;
						}
						
						//SIZE
						if (main.startSize3D) {
							if (startSize.mode == ParticleSystemCurveMode.TwoConstants) {
								startSizeX.constantMax *= size;
								startSizeX.constantMin *= size;

								startSizeY.constantMax *= size;
								startSizeY.constantMin *= size;

								startSizeZ.constantMax *= size;
								startSizeZ.constantMin *= size;
							} else {
								startSizeX.constant *= size;
								startSizeY.constant *= size;
								startSizeZ.constant *= size;
							}
							main.startSizeX = startSizeX;
							main.startSizeY = startSizeY;
							main.startSizeZ = startSizeZ;
						} else {
							if (startSize.mode == ParticleSystemCurveMode.TwoConstants) {
								startSize.constantMax *= size;
								startSize.constantMin *= size;
							} else {
								startSize.constant *= size;
							}
							main.startSize = startSize;
						}

						//START_SPEED (affected by size)
						if (startSpeed.mode == ParticleSystemCurveMode.TwoConstants) {
							startSpeed.constantMax *= size;
							startSpeed.constantMin *= size;
							main.startSpeed = startSpeed;
						} else {
							startSpeed.constant *= size;
							main.startSpeed = startSpeed;
						}

						//START_SPEED (affected by speed)
						if (startSpeed.mode == ParticleSystemCurveMode.TwoConstants) {
							startSpeed.constantMax *= speed;
							startSpeed.constantMin *= speed;
							main.startSpeed = startSpeed;
						} else {
							startSpeed.constant *= speed;
							main.startSpeed = startSpeed;
						}

						//LIFETIME
						if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants) {
							startLifetime.constantMax *= 1 / speed;
							startLifetime.constantMin *= 1 / speed;
							main.startLifetime = startLifetime;
						} else {
							startLifetime.constant *= 1 / speed;
							main.startLifetime = startLifetime;
						}

						//START_DELAY
						if (startDelay.mode == ParticleSystemCurveMode.TwoConstants) {
							startDelay.constantMax *= 1 / speed;
							startDelay.constantMin *= 1 / speed;
							main.startDelay = startDelay;
						} else {
							startDelay.constant *= 1 / speed;
							main.startDelay = startDelay;
						}

						//VELOCITY OVERLIFETIME
						if(velocityOverLifetime.enabled){
							float amount = 1;
							if(size != 1)
								amount = size;
							if(speed != 1)
								amount = speed;
							if(size != 1 && speed != 1) 
								amount = (size + speed)/2;
							
							if (velocityOverLifetime.x.mode == ParticleSystemCurveMode.TwoConstants) {								
								velocityOverLifetimeX.constantMax *= amount;
								velocityOverLifetimeX.constantMin *= amount;

								velocityOverLifetimeY.constantMax *= amount;
								velocityOverLifetimeY.constantMin *= amount;

								velocityOverLifetimeZ.constantMax *= amount;
								velocityOverLifetimeZ.constantMin *= amount;
							} else {
								velocityOverLifetimeX.constant *= amount;
								velocityOverLifetimeY.constant *= amount;
								velocityOverLifetimeZ.constant *= amount;
							}
							velocityOverLifetime.x = velocityOverLifetimeX;
							velocityOverLifetime.y = velocityOverLifetimeY;
							velocityOverLifetime.z = velocityOverLifetimeZ;
						}

						//RADIUS
						if (shape.enabled) {
							shape.radius *= size;
						}

						//COLOR
						if (changeColor) {
							if (main.startColor.mode == ParticleSystemGradientMode.Color) {
								startColor.color = ChangeHUE (startColor.color, newMaxColor);
								main.startColor = startColor;
							}
							if (main.startColor.mode == ParticleSystemGradientMode.TwoColors) {
								startColor.colorMax = ChangeHUE (startColor.colorMax, newMaxColor);
								startColor.colorMin = ChangeHUE (startColor.colorMin, newMinColor);
								main.startColor = startColor;
							} 
							if (main.startColor.mode == ParticleSystemGradientMode.Gradient) {
								startColor.gradient = ChangeGradientColor (startColor.gradient, newMaxColor, newMinColor);
								main.startColor = startColor;
							} 
							if (main.startColor.mode == ParticleSystemGradientMode.TwoGradients) {
								startColor.gradientMax = ChangeGradientColor (startColor.gradientMax, newMaxColor, newMinColor);
								startColor.gradientMin = ChangeGradientColor (startColor.gradientMin, newMinColor, newMaxColor);
								main.startColor = startColor;
							} 

							//COLOR OVERLIFETIME
							if (colorOverLifetime.enabled) {
								if (colorOverLifetime.color.mode == ParticleSystemGradientMode.Gradient) {							
									colorOverLifetimeC.gradient = ChangeGradientColor (colorOverLifetimeC.gradient, newMaxColor, newMinColor);
								}
								if (colorOverLifetime.color.mode == ParticleSystemGradientMode.TwoGradients) {
									colorOverLifetimeC.gradientMax = ChangeGradientColor (colorOverLifetimeC.gradientMax, newMaxColor, newMinColor);
									colorOverLifetimeC.gradientMin = ChangeGradientColor (colorOverLifetimeC.gradientMin, newMinColor, newMaxColor);
								}
								colorOverLifetime.color = colorOverLifetimeC;
							}
						}
					} else {
						//TRAIL RENDERER
						var trail = ParticleSystems [i].GetComponent<TrailRenderer> ();
						if (trail != null) {
							if (!SaveParticleSystemScript.CheckExistingFile (gameObject)) {		
								ParticleSystemOriginalSettings psOriginalSettings =	new ParticleSystemOriginalSettings {
									_trailGradient = new SerializableGradient (trail.colorGradient),
									_localPosition = new SerializableVector3 (trail.transform.localPosition),
									_trailWidthMultiplier = trail.widthMultiplier,
									_trailTime = trail.time
								};
								psOriginalSettingsList.Add (psOriginalSettings);
							} else {
								List<ParticleSystemOriginalSettings> listOriginalSettings = new List<ParticleSystemOriginalSettings> ();
								listOriginalSettings = SaveParticleSystemScript.LoadVFX (gameObject);

								trail.colorGradient = listOriginalSettings [i]._trailGradient.GetGradient();
								trail.transform.localPosition = listOriginalSettings [i]._localPosition.GetVector3 ();
								trail.widthMultiplier = listOriginalSettings [i]._trailWidthMultiplier;
								trail.time = listOriginalSettings [i]._trailTime;
							}
							trail.colorGradient = ChangeGradientColor (trail.colorGradient, newMaxColor, newMinColor);
							trail.widthMultiplier *= size;

							float amount = 1;
							if(size != 1)
								amount = size;
							if(speed != 1)
								amount = speed;
							if(size != 1 && speed != 1) 
								amount = (size + speed)/2;
							if(amount > 1)
								trail.time *= 1 / amount;
							else
								trail.time *= amount;
						}
					}
				}
				if (!SaveParticleSystemScript.CheckExistingFile (gameObject)) {		
					SaveParticleSystemScript.SaveVFX (gameObject, psOriginalSettingsList);
                }
#if UNITY_2018_3_OR_NEWER
                else
                {
                    SaveParticleSystemScript.SaveNestedPrefab(gameObject);
                }
#endif
            }
			else
				Debug.Log("No Particle Systems added to the Particle Systems list");
		}

		public void ChangeColorOnly () {
			if (ParticleSystems.Count == 0) {
				FillLists ();
			}

			if (ParticleSystems.Count > 0) {
				for (int i = 0; i < ParticleSystems.Count; i++) {
					var ps = ParticleSystems [i].GetComponent<ParticleSystem> ();
					if (ps != null) {
						var main = ps.main;
						var colorOverLifetime = ps.colorOverLifetime;
						var colorOverLifetimeC = colorOverLifetime.color;
						var startColor = main.startColor;
						//COLOR
						if (changeColor) {
							if (main.startColor.mode == ParticleSystemGradientMode.Color) {
								startColor.color = ChangeHUE (startColor.color, newMaxColor);
								main.startColor = startColor;
							}
							if (main.startColor.mode == ParticleSystemGradientMode.TwoColors) {
								startColor.colorMax = ChangeHUE (startColor.colorMax, newMaxColor);
								startColor.colorMin = ChangeHUE (startColor.colorMin, newMinColor);
								main.startColor = startColor;
							} 
							if (main.startColor.mode == ParticleSystemGradientMode.Gradient) {
								startColor.gradient = ChangeGradientColor (startColor.gradient, newMaxColor, newMinColor);
								main.startColor = startColor;
							} 
							if (main.startColor.mode == ParticleSystemGradientMode.TwoGradients) {
								startColor.gradientMax = ChangeGradientColor (startColor.gradientMax, newMaxColor, newMinColor);
								startColor.gradientMin = ChangeGradientColor (startColor.gradientMin, newMinColor, newMaxColor);
								main.startColor = startColor;
							} 

							//COLOR OVERLIFETIME
							if (colorOverLifetime.enabled) {
								if (colorOverLifetime.color.mode == ParticleSystemGradientMode.Gradient) {							
									colorOverLifetimeC.gradient = ChangeGradientColor (colorOverLifetimeC.gradient, newMaxColor, newMinColor);
								}
								if (colorOverLifetime.color.mode == ParticleSystemGradientMode.TwoGradients) {
									colorOverLifetimeC.gradientMax = ChangeGradientColor (colorOverLifetimeC.gradientMax, newMaxColor, newMinColor);
									colorOverLifetimeC.gradientMin = ChangeGradientColor (colorOverLifetimeC.gradientMin, newMinColor, newMaxColor);
								}
								colorOverLifetime.color = colorOverLifetimeC;
							}
						}
					} else {
						//TRAIL RENDERER
						var trail = ParticleSystems [i].GetComponent<TrailRenderer> ();
						if (trail != null) {
							if (!SaveParticleSystemScript.CheckExistingFile (gameObject)) {		
								ParticleSystemOriginalSettings psOriginalSettings =	new ParticleSystemOriginalSettings {
									_trailGradient = new SerializableGradient (trail.colorGradient),
									_localPosition = new SerializableVector3 (trail.transform.localPosition),
									_trailWidthMultiplier = trail.widthMultiplier,
									_trailTime = trail.time
								};
								psOriginalSettingsList.Add (psOriginalSettings);
							} else {
								List<ParticleSystemOriginalSettings> listOriginalSettings = new List<ParticleSystemOriginalSettings> ();
								listOriginalSettings = SaveParticleSystemScript.LoadVFX (gameObject);

								trail.colorGradient = listOriginalSettings [i]._trailGradient.GetGradient ();
								trail.transform.localPosition = listOriginalSettings [i]._localPosition.GetVector3 ();
								trail.widthMultiplier = listOriginalSettings [i]._trailWidthMultiplier;
								trail.time = listOriginalSettings [i]._trailTime;
							}
							trail.colorGradient = ChangeGradientColor (trail.colorGradient, newMaxColor, newMinColor);
						}
					}
				}
			}
		}

		public void ResizeOnly () {
			if (ParticleSystems.Count == 0) {
				FillLists ();
			}
			
			if (ParticleSystems.Count > 0) {
				for (int i = 0; i < ParticleSystems.Count; i ++) {
					var ps = ParticleSystems [i].GetComponent<ParticleSystem> ();
					if (ps != null) {
						var main = ps.main;						
						var shape = ps.shape;
						var startSize = main.startSize;
						var startSizeX = main.startSizeX;
						var startSizeY = main.startSizeY;
						var startSizeZ = main.startSizeZ;
						var startSpeed = main.startSpeed;
						var velocityOverLifetime = ps.velocityOverLifetime;
						var velocityOverLifetimeX = velocityOverLifetime.x;
						var velocityOverLifetimeY = velocityOverLifetime.y;
						var velocityOverLifetimeZ = velocityOverLifetime.z;
						var localPos = ParticleSystems [i].transform.localPosition;

						//POSITION
						if (i > 0) {
							if (localPos.x != 0 || localPos.y != 0 || localPos.z != 0) {						
								localPos.x *= size;
								localPos.y *= size;
								localPos.z *= size;
								ParticleSystems [i].transform.localPosition = localPos;
							}
						}
						
						//SIZE
						if (main.startSize3D) {
							if (startSize.mode == ParticleSystemCurveMode.TwoConstants) {
								startSizeX.constantMax *= size;
								startSizeX.constantMin *= size;

								startSizeY.constantMax *= size;
								startSizeY.constantMin *= size;

								startSizeZ.constantMax *= size;
								startSizeZ.constantMin *= size;
							} else {
								startSizeX.constant *= size;
								startSizeY.constant *= size;
								startSizeZ.constant *= size;
							}
							main.startSizeX = startSizeX;
							main.startSizeY = startSizeY;
							main.startSizeZ = startSizeZ;
						} else {
							if (startSize.mode == ParticleSystemCurveMode.TwoConstants) {
								startSize.constantMax *= size;
								startSize.constantMin *= size;
							} else {
								startSize.constant *= size;
							}
							main.startSize = startSize;
						}

						//START_SPEED (affected by size)
						if (startSpeed.mode == ParticleSystemCurveMode.TwoConstants) {
							startSpeed.constantMax *= size;
							startSpeed.constantMin *= size;
							main.startSpeed = startSpeed;
						} else {
							startSpeed.constant *= size;
							main.startSpeed = startSpeed;
						}

						//VELOCITY OVERLIFETIME
						if(velocityOverLifetime.enabled){
							float amount = 1;
							if(size != 1)
								amount = size;
							if(speed != 1)
								amount = speed;
							if(size != 1 && speed != 1) 
								amount = (size + speed)/2;
							
							if (velocityOverLifetime.x.mode == ParticleSystemCurveMode.TwoConstants) {								
								velocityOverLifetimeX.constantMax *= amount;
								velocityOverLifetimeX.constantMin *= amount;

								velocityOverLifetimeY.constantMax *= amount;
								velocityOverLifetimeY.constantMin *= amount;

								velocityOverLifetimeZ.constantMax *= amount;
								velocityOverLifetimeZ.constantMin *= amount;
							} else {
								velocityOverLifetimeX.constant *= amount;
								velocityOverLifetimeY.constant *= amount;
								velocityOverLifetimeZ.constant *= amount;
							}
							velocityOverLifetime.x = velocityOverLifetimeX;
							velocityOverLifetime.y = velocityOverLifetimeY;
							velocityOverLifetime.z = velocityOverLifetimeZ;
						}

						//RADIUS
						if (shape.enabled) {
							shape.radius *= size;
						}
					}
					else{
						//TRAIL RENDERER
						var trail = ParticleSystems [i].GetComponent<TrailRenderer> ();
						if (trail != null) {							
							trail.widthMultiplier *= size;

							float amount = 1;
							if(size != 1)
								amount = size;
							if(speed != 1)
								amount = speed;
							if(size != 1 && speed != 1) 
								amount = (size + speed)/2;
							if(amount > 1)
								trail.time *= 1 / amount;
							else
							trail.time *= amount;
						}
					}
				}
			}
		}

		public void ResetParticleSystem (){

			List<ParticleSystemOriginalSettings> listOriginalSettings = new List<ParticleSystemOriginalSettings> ();
			listOriginalSettings = SaveParticleSystemScript.LoadVFX (gameObject);

			if (listOriginalSettings != null) {
				for (int i = 0; i < ParticleSystems.Count; i++) {
					var ps = ParticleSystems [i].GetComponent<ParticleSystem> ();
					if (ps != null) {
						var main = ps.main;
						var shape = ps.shape;
						var colorOverLifetime = ps.colorOverLifetime;
						var velocityOverLifetime = ps.velocityOverLifetime;
						main.startColor = listOriginalSettings [i]._startColor.GetMinMaxGradient ();
						colorOverLifetime.color = listOriginalSettings [i]._colorOverLifetimeC.GetMinMaxGradient ();
						main.startSize = listOriginalSettings [i]._startSize.GetMinMaxCurve ();
						main.startSizeX = listOriginalSettings [i]._startSizeX.GetMinMaxCurve ();
						main.startSizeY = listOriginalSettings [i]._startSizeY.GetMinMaxCurve ();
						main.startSizeZ = listOriginalSettings [i]._startSizeZ.GetMinMaxCurve ();
						main.startSpeed = listOriginalSettings [i]._startSpeed.GetMinMaxCurve ();
						main.startDelay = listOriginalSettings [i]._startDelay.GetMinMaxCurve ();
						main.startLifetime = listOriginalSettings [i]._startLifetime.GetMinMaxCurve ();
						velocityOverLifetime.x = listOriginalSettings [i]._velocityOverLifetimeX.GetMinMaxCurve ();
						velocityOverLifetime.y = listOriginalSettings [i]._velocityOverLifetimeY.GetMinMaxCurve ();
						velocityOverLifetime.z = listOriginalSettings [i]._velocityOverLifetimeZ.GetMinMaxCurve ();
						ParticleSystems [i].transform.localPosition = listOriginalSettings [i]._localPosition.GetVector3 ();
						main.duration = listOriginalSettings [i]._duration;
						shape.radius = listOriginalSettings [i]._shapeRadius;
						ps.gameObject.SetActive (listOriginalSettings [i]._active);
						main.loop = listOriginalSettings [i]._loop;
						main.prewarm = listOriginalSettings [i]._prewarm;
					} else {
						var trail = ParticleSystems [i].GetComponent<TrailRenderer> ();
						if (trail != null) {						
							trail.colorGradient = listOriginalSettings [i]._trailGradient.GetGradient ();
							trail.widthMultiplier = listOriginalSettings [i]._trailWidthMultiplier;
							trail.time = listOriginalSettings [i]._trailTime;
							ParticleSystems [i].transform.localPosition = listOriginalSettings [i]._localPosition.GetVector3 ();
						}
					}
				}
#if UNITY_2018_3_OR_NEWER
                SaveParticleSystemScript.SaveNestedPrefab(gameObject);
#endif
                Debug.Log ( gameObject.name + " reseted to default.");
			}
		}
		
		public Color ChangeHUE (Color oldColor, Color newColor){
			float newHue;
			float newSaturation;
			float newValue;
			float oldHue;
			float oldSaturation;
			float oldValue;
			float originalAlpha = oldColor.a;
			Color.RGBToHSV (newColor, out newHue, out newSaturation, out newValue); 
			Color.RGBToHSV (oldColor, out oldHue, out oldSaturation, out oldValue); 
			var updatedColor = Color.HSVToRGB (newHue, oldSaturation, oldValue);
			updatedColor.a = originalAlpha;
			return updatedColor;
		}

		public Gradient ChangeGradientColor (Gradient oldGradient, Color newMaxColor, Color newMinColor){
			GradientColorKey[] colorKeys = new GradientColorKey[oldGradient.colorKeys.Length];
			for(int j = 0; j < oldGradient.colorKeys.Length; j++){
				colorKeys [j].time = oldGradient.colorKeys [j].time;
				if(j%2 == 0)
					colorKeys [j].color = ChangeHUE (oldGradient.colorKeys[j].color, newMaxColor);
				if(j%2 == 1)
					colorKeys [j].color = ChangeHUE (oldGradient.colorKeys[j].color, newMinColor);
			}
			oldGradient.SetKeys (colorKeys, oldGradient.alphaKeys);
			return oldGradient;
		}		

		public void FillLists (){
			if (ParticleSystems.Count == 0) {
				var ps = GetComponent<ParticleSystem> ();
				var trail = GetComponent<TrailRenderer> ();
				if (ps != null || trail != null)
					ParticleSystems.Add (gameObject);		

				AddChildRecurvsively (transform);

				for (int i = 0; i < ParticleSystems.Count; i++) {
					ActiveParticleSystems.Add (true);
				}
			} else {
				Debug.Log ("Lists already have GameObjects. For automatic filling consider emptying the lists and try again.");
			}
		}

		public void EmptyLists (){
			ParticleSystems.Clear();
			ActiveParticleSystems.Clear();
		}

		void AddChildRecurvsively (Transform transf){
			foreach (Transform t in transf) {
				var child = t.gameObject;
				var psChild = child.GetComponent<ParticleSystem> ();
				var trailChild = child.GetComponent<TrailRenderer> ();
				if (psChild != null || trailChild != null) 
					ParticleSystems.Add (child);				
				if (child.transform.childCount > 0) 
					AddChildRecurvsively (child.transform);				
			}
		}
	}
}
