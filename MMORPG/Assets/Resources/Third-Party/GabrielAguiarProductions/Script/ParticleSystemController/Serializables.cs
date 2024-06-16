

#pragma warning disable 0162 // unreachable code detected.
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0472 // comparing color with null is always null.
#pragma warning disable 0618 // tangent mode obsolete warning.

using UnityEngine;
using System;

namespace GAP_ParticleSystemController{

	[Serializable]
	public class SerializableMinMaxGradient{
		public SerializableColor color;
		public SerializableColor colorMax;
		public SerializableColor colorMin;
		public SerializableAlphaKeys gradientAlphaKeys;
		public SerializableColorKeys gradientColorKeys;
		public SerializableAlphaKeys gradientMaxAlphaKeys;
		public SerializableColorKeys gradientMaxColorKeys;
		public SerializableAlphaKeys gradientMinAlphaKeys;
		public SerializableColorKeys gradientMinColorKeys;
		public SerializablePSGradientMode gradientMode;

		public SerializableMinMaxGradient (ParticleSystem.MinMaxGradient minMaxGradient){

			gradientMode = new SerializablePSGradientMode (minMaxGradient.mode);

			if (minMaxGradient.mode == ParticleSystemGradientMode.Color)
				color = new SerializableColor (minMaxGradient.color);

			if (minMaxGradient.mode == ParticleSystemGradientMode.TwoColors) {
				colorMax = new SerializableColor (minMaxGradient.colorMax);
				colorMin = new SerializableColor (minMaxGradient.colorMin);
			}

			if (minMaxGradient.mode == ParticleSystemGradientMode.Gradient) {
				gradientAlphaKeys = new SerializableAlphaKeys (minMaxGradient.gradient.alphaKeys);
				gradientColorKeys = new SerializableColorKeys (minMaxGradient.gradient.colorKeys);
			}

			if (minMaxGradient.mode == ParticleSystemGradientMode.TwoGradients) {
				gradientMaxAlphaKeys = new SerializableAlphaKeys (minMaxGradient.gradientMax.alphaKeys);
				gradientMaxColorKeys = new SerializableColorKeys (minMaxGradient.gradientMax.colorKeys);
				gradientMinAlphaKeys = new SerializableAlphaKeys (minMaxGradient.gradientMin.alphaKeys);
				gradientMinColorKeys = new SerializableColorKeys (minMaxGradient.gradientMin.colorKeys);
			}
		}

		public ParticleSystem.MinMaxGradient GetMinMaxGradient (){
			ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient ();
			if (gradientMode.GetGradientMode () == ParticleSystemGradientMode.Color) {
				if (minMaxGradient.color == null)
					minMaxGradient.color = new Color ();
				minMaxGradient.color = color.GetColor ();
			}
			
			if (gradientMode.GetGradientMode() == ParticleSystemGradientMode.TwoColors) {
				if (minMaxGradient.colorMax == null)
					minMaxGradient.colorMax = new Color ();
				minMaxGradient.colorMax = colorMax.GetColor ();

				if (minMaxGradient.colorMin == null)
					minMaxGradient.colorMin = new Color ();
				minMaxGradient.colorMin = colorMin.GetColor ();
			}

			if (gradientMode.GetGradientMode() == ParticleSystemGradientMode.Gradient) {
				if (minMaxGradient.gradient == null)
					minMaxGradient.gradient = new Gradient ();
				minMaxGradient.gradient.alphaKeys = gradientAlphaKeys.GetAlphaKeys ();
				minMaxGradient.gradient.colorKeys = gradientColorKeys.GetColorKeys ();
			}

			if (gradientMode.GetGradientMode() == ParticleSystemGradientMode.TwoGradients) {
				if (minMaxGradient.gradientMax == null)
					minMaxGradient.gradientMax = new Gradient ();
				minMaxGradient.gradientMax.alphaKeys = gradientMaxAlphaKeys.GetAlphaKeys ();
				minMaxGradient.gradientMax.colorKeys = gradientMaxColorKeys.GetColorKeys ();

				if (minMaxGradient.gradientMin == null)
					minMaxGradient.gradientMin = new Gradient ();
				minMaxGradient.gradientMin.alphaKeys = gradientMinAlphaKeys.GetAlphaKeys ();
				minMaxGradient.gradientMin.colorKeys = gradientMinColorKeys.GetColorKeys ();
			}

			minMaxGradient.mode = gradientMode.GetGradientMode ();

			return minMaxGradient;
		}
	}

	[Serializable]
	public class SerializableMinMaxCurve{
		public float constant;
		public float constantMax;
		public float constantMin;
		public SerializableAnimationCurve curve;
		public SerializableAnimationCurve curveMax;
		public SerializableAnimationCurve curveMin;
		public float curveMultiplier;
		public SerializablePSCurveMode curveMode;

		public SerializableMinMaxCurve (ParticleSystem.MinMaxCurve minMaxCurve){
			
			curveMode = new SerializablePSCurveMode (minMaxCurve.mode);

			if (minMaxCurve.mode == ParticleSystemCurveMode.Constant)
				constant = minMaxCurve.constant;

			if (minMaxCurve.mode == ParticleSystemCurveMode.Curve)
				curve = new SerializableAnimationCurve (minMaxCurve.curve);

			if (minMaxCurve.mode == ParticleSystemCurveMode.TwoConstants) {
				constantMax = minMaxCurve.constantMax;
				constantMin = minMaxCurve.constantMin;
			}

			if (minMaxCurve.mode == ParticleSystemCurveMode.TwoCurves) {
				curveMax = new SerializableAnimationCurve (minMaxCurve.curveMax);
				curveMin = new SerializableAnimationCurve (minMaxCurve.curve);
			}

			curveMultiplier = minMaxCurve.curveMultiplier;
		}

		public ParticleSystem.MinMaxCurve GetMinMaxCurve (){
			ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve ();

			if (curveMode.GetCurveMode() == ParticleSystemCurveMode.Constant)				
				minMaxCurve.constant = constant;

			if (curveMode.GetCurveMode() == ParticleSystemCurveMode.TwoConstants) {
				minMaxCurve.constantMax = constantMax;
				minMaxCurve.constantMin = constantMin;
			}

			if (curveMode.GetCurveMode () == ParticleSystemCurveMode.Curve) {
				if (minMaxCurve.curve == null)
					minMaxCurve.curve = new AnimationCurve ();
				minMaxCurve.curve = curve.GetAnimationCurve ();
			}

			if (curveMode.GetCurveMode() == ParticleSystemCurveMode.TwoCurves) {
				if (minMaxCurve.curveMax == null)
					minMaxCurve.curveMax = new AnimationCurve ();
				minMaxCurve.curveMax = curveMax.GetAnimationCurve();

				if (minMaxCurve.curveMin == null)
					minMaxCurve.curveMin = new AnimationCurve ();
				minMaxCurve.curveMin = curveMin.GetAnimationCurve();
			}

			minMaxCurve.curveMultiplier = curveMultiplier;
			minMaxCurve.mode = curveMode.GetCurveMode ();

			return minMaxCurve;
		}
	}

	[Serializable]
	public class SerializableAnimationCurve {
		public SerializableKeyFrames[] keys;
		public SerializableWrapMode postWrapMode;
		public SerializableWrapMode preWrapMode;

		public SerializableAnimationCurve (AnimationCurve animCurve){
			SerializableKeyFrames[] keys_ = new SerializableKeyFrames[animCurve.keys.Length];
			for (int i = 0; i < animCurve.length; i++) {
				keys_[i] = new SerializableKeyFrames (animCurve.keys[i]);
			}
			keys = keys_;
			postWrapMode = new SerializableWrapMode(animCurve.postWrapMode);
			preWrapMode = new SerializableWrapMode(animCurve.preWrapMode);
		}

		public AnimationCurve GetAnimationCurve (){
			AnimationCurve animCurv = new AnimationCurve ();
			animCurv.keys = new Keyframe[keys.Length];
			for(int i = 0; i < keys.Length; i++){
				animCurv.keys[i] = keys[i].GetKeyFrames();
			}
			animCurv.postWrapMode = postWrapMode.GetWrapMode();
			animCurv.preWrapMode = preWrapMode.GetWrapMode();

			return animCurv;
		}
	}

	[Serializable]
	public class SerializableKeyFrames{
		public float inTangent;
		public float outTangent;
		public int tangentMode;
		public float time;
		public float value;

		public SerializableKeyFrames (Keyframe keyFrame){

			inTangent = keyFrame.inTangent;
			outTangent = keyFrame.outTangent;
			tangentMode = keyFrame.tangentMode;
		
			time = keyFrame.time;
			value = keyFrame.value;
		}

		public Keyframe GetKeyFrames (){
			Keyframe kf = new Keyframe();

			kf.inTangent = inTangent;
			kf.outTangent = outTangent;
			kf.tangentMode = tangentMode;
			kf.time = time;
			kf.value = value;

			return kf;
		}
	}

	[Serializable]
	public class SerializableAlphaKeys{
		public float[] alpha;
		public float[] time;

		public SerializableAlphaKeys (GradientAlphaKey[] gradAlphaKeys){
			float[] alpha_ = new float[gradAlphaKeys.Length];
			float[] time_ = new float[gradAlphaKeys.Length];
			for (int i = 0; i < gradAlphaKeys.Length; i++) {
				alpha_[i] = gradAlphaKeys[i].alpha;
				time_[i] = gradAlphaKeys[i].time;
			}
			alpha = alpha_;
			time = time_;
		}

		public GradientAlphaKey[] GetAlphaKeys (){
			GradientAlphaKey[] gak = new GradientAlphaKey[alpha.Length];
			for (int i = 0; i < alpha.Length; i++) {
				gak [i].alpha = alpha [i];
				gak [i].time = time [i];
			}
			return gak;
		}
	}

	[Serializable]
	public class SerializableColorKeys{
		public SerializableColor[] color;
		public float[] time;

		public SerializableColorKeys (GradientColorKey[] gradColorKeys){
			SerializableColor[] color_ = new SerializableColor[gradColorKeys.Length];
			float[] time_ = new float[gradColorKeys.Length];
			for (int i = 0; i < gradColorKeys.Length; i++) {
				color_[i] = new SerializableColor (gradColorKeys[i].color);
				time_[i] = gradColorKeys[i].time;
			}
			color = color_;
			time = time_;
		}

		public GradientColorKey[] GetColorKeys (){
			GradientColorKey[] gck = new GradientColorKey[color.Length];
			for (int i = 0; i < color.Length; i++) {
				gck [i].color = color [i].GetColor();
				gck [i].time = time [i];
			}
			return gck;
		}
	}

	[Serializable]
	public class SerializableColor {
		public float R;
		public float G;
		public float B;
		public float A;

		public SerializableColor (Color color){
			R = color.r;
			G = color.g;
			B = color.b;
			A = color.a;
		}

		public Color GetColor (){
			return new Color (R,G,B,A);
		}
	}

	[Serializable]
	public class SerializableVector3 {
		public float x;
		public float y;
		public float z;

		public SerializableVector3 (Vector3 v3){
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}

		public Vector3 GetVector3 (){
			return new Vector3 (x,y,z);
		}
	}

	[Serializable]
	public class SerializableGradient{
		public SerializableAlphaKeys gradientAlphaKeys;
		public SerializableColorKeys gradientColorKeys;
		public SerializableGradientMode gradientMode;

		public SerializableGradient (Gradient gradient){
			gradientMode = new SerializableGradientMode (gradient.mode);
			gradientAlphaKeys = new SerializableAlphaKeys (gradient.alphaKeys);
			gradientColorKeys = new SerializableColorKeys (gradient.colorKeys);
		}

		public Gradient GetGradient (){
			Gradient gradient = new Gradient ();
			gradient.alphaKeys = gradientAlphaKeys.GetAlphaKeys ();
			gradient.colorKeys = gradientColorKeys.GetColorKeys ();
			gradient.mode = gradientMode.GetGradientMode ();

			return gradient;
		}
	}

	[Serializable]
	public class SerializablePSGradientMode{
		public string mode;

		public SerializablePSGradientMode (ParticleSystemGradientMode psGradientMode){
			mode = psGradientMode.ToString();
		}

		public ParticleSystemGradientMode GetGradientMode (){
			ParticleSystemGradientMode psGradientMode = new ParticleSystemGradientMode ();
			switch (mode) {
			case "Color":
				{
					psGradientMode = ParticleSystemGradientMode.Color;
					break;
				}
			case "Gradient":
				{
					psGradientMode = ParticleSystemGradientMode.Gradient;
					break;
				}
			case "RandomColor":
				{
					psGradientMode = ParticleSystemGradientMode.RandomColor;
					break;
				}
			case "TwoColors":
				{
					psGradientMode = ParticleSystemGradientMode.TwoColors;
					break;
				}
			case "TwoGradients":
				{
					psGradientMode = ParticleSystemGradientMode.TwoGradients;
					break;
				}
			}
			return psGradientMode;
		}
	}

	[Serializable]
	public class SerializableGradientMode{
		public string mode;

		public SerializableGradientMode (GradientMode gradientMode){
			mode = gradientMode.ToString();
		}

		public GradientMode GetGradientMode (){
			GradientMode gradientMode = new GradientMode ();
			switch (mode) {
			case "Blend":
				{
					gradientMode = GradientMode.Blend;
					break;
				}
			case "Fixed":
				{
					gradientMode = GradientMode.Fixed;
					break;
				}			
			}
			return gradientMode;
		}
	}

	[Serializable]
	public class SerializablePSCurveMode{
		public string mode;

		public SerializablePSCurveMode (ParticleSystemCurveMode psCurveMode){
			mode = psCurveMode.ToString();
		}

		public ParticleSystemCurveMode GetCurveMode (){
			ParticleSystemCurveMode psCurveMode = new ParticleSystemCurveMode ();
			switch (mode) {
			case "Constant":
				{
					psCurveMode = ParticleSystemCurveMode.Constant;
					break;
				}
			case "Curve":
				{
					psCurveMode = ParticleSystemCurveMode.Curve;
					break;
				}
			case "TwoConstants":
				{
					psCurveMode = ParticleSystemCurveMode.TwoConstants;
					break;
				}
			case "TwoCurves":
				{
					psCurveMode = ParticleSystemCurveMode.TwoCurves;
					break;
				}
			}

			return psCurveMode;
		}
	}

	[Serializable]
	public class SerializableWrapMode {
		public string mode;

		public SerializableWrapMode (WrapMode wrapMode){
			mode = wrapMode.ToString();
		}

		public WrapMode GetWrapMode (){
			WrapMode wrapMode = new WrapMode ();
			switch (mode) {
			case "Clamp":
				{
					wrapMode = WrapMode.Clamp;
					break;
				}
			case "ClampForever":
				{
					wrapMode = WrapMode.ClampForever;
					break;
				}
			case "Default":
				{
					wrapMode = WrapMode.Default;
					break;
				}
			case "Loop":
				{
					wrapMode = WrapMode.Loop;
					break;
				}
			case "Once":
				{
					wrapMode = WrapMode.Once;
					break;
				}
			case "PingPong":
				{
					wrapMode = WrapMode.PingPong;
					break;
				}
			}

			return wrapMode;
		}
	}
}
