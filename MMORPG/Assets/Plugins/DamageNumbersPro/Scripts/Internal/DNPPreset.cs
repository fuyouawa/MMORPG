using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DamageNumbersPro;

namespace DamageNumbersPro.Internal
{
    [CreateAssetMenu(fileName = "Preset", menuName = "TextMeshPro/Preset for DNP", order = -1)]
    public class DNPPreset : ScriptableObject
    {
        //Font:
        public bool changeFontAsset;
        public TMP_FontAsset fontAsset;

        //Color:
        public bool changeColor;
        public Color color = Color.white;
        public bool enableGradient;
        public VertexGradient gradient = new VertexGradient(Color.white, Color.white, Color.white, Color.white);

        //Number:
        public bool changeNumber;
        public bool enableNumber = true;
        public TextSettings numberSettings = new TextSettings(0);
        public DigitSettings digitSettings = new DigitSettings(0);

        //Left Text:
        public bool changeLeftText;
        public bool enableLeftText = true;
        public string leftText;
        public TextSettings leftTextSettings = new TextSettings(0f);

        //Right Text:
        public bool changeRightText;
        public bool enableRightText = true;
        public string rightText;
        public TextSettings rightTextSettings = new TextSettings(0f);

        //Vertical Text:
        public bool hideVerticalTexts = false;

        //Fade In:
        public bool changeFadeIn = false;
        public float durationFadeIn = 0.2f;
        public bool enableOffsetFadeIn = true;
        [Tooltip("TextA and TextB move together from this offset.")]
        public Vector2 offsetFadeIn = new Vector2(0.5f, 0);
        public bool enableScaleFadeIn = true;
        [Tooltip("Scales in from this scale.")]
        public Vector2 scaleFadeIn = new Vector2(2, 2);
        public bool enableCrossScaleFadeIn = false;
        [Tooltip("Scales TextA in from this scale and TextB from the inverse of this scale.")]
        public Vector2 crossScaleFadeIn = new Vector2(1, 1.5f);
        public bool enableShakeFadeIn = false;
        [Tooltip("Shakes in from this offset.")]
        public Vector2 shakeOffsetFadeIn = new Vector2(0, 1.5f);
        [Tooltip("Shakes in at this frequency.")]
        public float shakeFrequencyFadeIn = 4f;

        //Fade Out:
        public bool changeFadeOut = false;
        public float durationFadeOut = 0.2f;
        public bool enableOffsetFadeOut = true;
        [Tooltip("TextA and TextB move apart to this offset.")]
        public Vector2 offsetFadeOut = new Vector2(0.5f, 0);
        public bool enableScaleFadeOut = false;
        [Tooltip("Scales out to this scale.")]
        public Vector2 scaleFadeOut = new Vector2(2, 2);
        public bool enableCrossScaleFadeOut = false;
        [Tooltip("Scales TextA out to this scale and TextB to the inverse of this scale.")]
        public Vector2 crossScaleFadeOut = new Vector2(1, 1.5f);
        public bool enableShakeFadeOut = false;
        [Tooltip("Shakes out to this offset.")]
        public Vector2 shakeOffsetFadeOut = new Vector2(0, 1.5f);
        [Tooltip("Shakes out at this frequency.")]
        public float shakeFrequencyFadeOut = 4f;

        //Movement:
        public bool changeMovement = false;
        public bool enableLerp = true;
        public LerpSettings lerpSettings = new LerpSettings(0);
        public bool enableVelocity = false;
        public VelocitySettings velocitySettings = new VelocitySettings(0);
        public bool enableShaking = false;
        [Tooltip("Shake settings during idle.")]
        public ShakeSettings shakeSettings = new ShakeSettings(new Vector2(0.005f, 0.005f));
        public bool enableFollowing = false;
        public FollowSettings followSettings = new FollowSettings(0);

        //Rotation:
        public bool changeRotation = false;
        public bool enableStartRotation = false;
        [Tooltip("The minimum z-angle for the random spawn rotation.")]
        public float minRotation = -4f;
        [Tooltip("The maximum z-angle for the random spawn rotation.")]
        public float maxRotation = 4f;
        public bool enableRotateOverTime = false;
        [Tooltip("The minimum rotation speed for the z-angle.")]
        public float minRotationSpeed = -15f;
        [Tooltip("The maximum rotation speed for the z-angle.")]
        public float maxRotationSpeed = 15;
        [Tooltip("Defines rotation speed over lifetime.")]
        public AnimationCurve rotateOverTime = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(0.4f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0) });

        //Scaling:
        public bool changeScaling = false;
        public bool enableScaleByNumber = false;
        public ScaleByNumberSettings scaleByNumberSettings = new ScaleByNumberSettings(0);
        public bool enableScaleOverTime = false;
        [Tooltip("Will scale over it's lifetime using this curve.")]
        public AnimationCurve scaleOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.7f));

        //Spam Control:
        public bool changeSpamControl = false;
        public string spamGroup = "";
        public bool enableCombination = false;
        public CombinationSettings combinationSettings = new CombinationSettings(0);
        public bool enableDestruction = false;
        public DestructionSettings destructionSettings = new DestructionSettings(0);
        public bool enableCollision = false;
        public CollisionSettings collisionSettings = new CollisionSettings(0);
        public bool enablePush = false;
        public PushSettings pushSettings = new PushSettings(0);

        public bool IsApplied(DamageNumber dn)
        {
            TMP_Text[] textMeshs = dn.GetTextMeshs();

            if (textMeshs[0] == null)
            {
                dn.GetReferencesIfNecessary();
                textMeshs = dn.GetTextMeshs();
            }

            bool isApplied = true;

            //Font:
            if (changeFontAsset)
            {
                foreach(TMP_Text tmp in textMeshs)
                {
                    if (fontAsset != tmp.font)
                    {
                        isApplied = false;
                    }
                }
            }

            //Color:
            if (changeColor)
            {
                foreach (TMP_Text tmp in textMeshs)
                {
                    if (color != tmp.color || enableGradient != tmp.enableVertexGradient || !gradient.Equals(tmp.colorGradient))
                    {
                        isApplied = false;
                    }
                }
            }

            //Number:
            if(changeNumber)
            {
                if(enableNumber != dn.enableNumber || !numberSettings.Equals(dn.numberSettings) || !digitSettings.Equals(dn.digitSettings))
                {
                    isApplied = false;
                }
            }

            //Left Text:
            if(changeLeftText)
            {
                if(enableLeftText != dn.enableLeftText || !leftTextSettings.Equals(dn.leftTextSettings) || leftText != dn.leftText)
                {
                    isApplied = false;
                }
            }

            //Right Text:
            if (changeRightText)
            {
                if (enableRightText != dn.enableRightText || !rightTextSettings.Equals(dn.rightTextSettings) || rightText != dn.rightText)
                {
                    isApplied = false;
                }
            }

            //Vertical Texts:
            if (hideVerticalTexts)
            {
                if(dn.enableTopText || dn.enableBottomText)
                {
                    isApplied = false;
                }
            }

            //Fade In:
            if(changeFadeIn)
            {
                if(durationFadeIn != dn.durationFadeIn || enableOffsetFadeIn != dn.enableOffsetFadeIn || offsetFadeIn != dn.offsetFadeIn ||
                    enableScaleFadeIn != dn.enableScaleFadeIn || scaleFadeIn != dn.scaleFadeIn || enableCrossScaleFadeIn != dn.enableCrossScaleFadeIn ||
                    crossScaleFadeIn != dn.crossScaleFadeIn || enableShakeFadeIn != dn.enableShakeFadeIn || shakeOffsetFadeIn != dn.shakeOffsetFadeIn ||
                    shakeFrequencyFadeIn != dn.shakeFrequencyFadeIn)
                {
                    isApplied = false;
                }
            }

            //Fade Out:
            if (changeFadeOut)
            {
                if (durationFadeOut != dn.durationFadeOut || enableOffsetFadeOut != dn.enableOffsetFadeOut || offsetFadeOut != dn.offsetFadeOut ||
                    enableScaleFadeOut != dn.enableScaleFadeOut || scaleFadeOut != dn.scaleFadeOut || enableCrossScaleFadeOut != dn.enableCrossScaleFadeOut ||
                    crossScaleFadeOut != dn.crossScaleFadeOut || enableShakeFadeOut != dn.enableShakeFadeOut || shakeOffsetFadeOut != dn.shakeOffsetFadeOut ||
                    shakeFrequencyFadeOut != dn.shakeFrequencyFadeOut)
                {
                    isApplied = false;
                }
            }

            //Movement:
            if(changeMovement)
            {
                if(enableLerp != dn.enableLerp || !lerpSettings.Equals(dn.lerpSettings) ||
                    enableVelocity != dn.enableVelocity || !velocitySettings.Equals(dn.velocitySettings) ||
                    enableShaking != dn.enableShaking || !shakeSettings.Equals(dn.shakeSettings) ||
                    enableFollowing != dn.enableFollowing || !followSettings.Equals(dn.followSettings))
                {
                    isApplied = false;
                }
            }

            //Rotation:
            if(changeRotation)
            {
                if(enableStartRotation != dn.enableStartRotation || minRotation != dn.minRotation || maxRotation != dn.maxRotation ||
                    enableRotateOverTime != dn.enableRotateOverTime || minRotationSpeed != dn.minRotationSpeed || maxRotationSpeed != dn.maxRotationSpeed || !rotateOverTime.Equals(dn.rotateOverTime))
                {
                    isApplied = false;
                }
            }

            //Scale:
            if(changeScaling)
            {
                if(enableScaleByNumber != dn.enableScaleByNumber || !scaleByNumberSettings.Equals(dn.scaleByNumberSettings) ||
                    enableScaleOverTime != dn.enableScaleOverTime || !scaleOverTime.Equals(dn.scaleOverTime))
                {
                    isApplied = false;
                }
            }

            //Spam Group:
            if(changeSpamControl)
            {
                if(enableCombination != dn.enableCombination || !combinationSettings.Equals(dn.combinationSettings) ||
                    enableDestruction != dn.enableDestruction || !destructionSettings.Equals(dn.destructionSettings) ||
                    enableCollision != dn.enableCollision || !collisionSettings.Equals(dn.collisionSettings) ||
                    enablePush != dn.enablePush || !pushSettings.Equals(dn.pushSettings))
                {
                    isApplied = false;
                }
            }

            return isApplied;
        }

        public void Apply(DamageNumber dn)
        {
            TMP_Text[] textMeshs = dn.GetTextMeshs();

            //Font:
            if (changeFontAsset)
            {
                foreach(TMP_Text tmp in textMeshs)
                {
                    tmp.font = fontAsset;
                }
            }

            //Color:
            if (changeColor)
            {
                foreach (TMP_Text tmp in textMeshs)
                {
                    tmp.color = color;
                    tmp.enableVertexGradient = enableGradient;
                    tmp.colorGradient = gradient;
                }
            }

            //Number:
            if (changeNumber)
            {
                dn.enableNumber = enableNumber;
                dn.numberSettings = numberSettings;
                dn.digitSettings = digitSettings;
            }

            //Left Text:
            if (changeLeftText)
            {
                dn.enableLeftText = enableLeftText;
                dn.leftText = leftText;
                dn.leftTextSettings = leftTextSettings;
            }

            //Right Text:
            if (changeRightText)
            {
                dn.enableRightText = enableRightText;
                dn.rightText = rightText;
                dn.rightTextSettings = rightTextSettings;
            }

            //Hide Vertical Texts:
            if(hideVerticalTexts)
            {
                dn.enableTopText = dn.enableBottomText = false;
            }

            //Fade In:
            if(changeFadeIn)
            {
                dn.durationFadeIn = durationFadeIn;
                dn.enableOffsetFadeIn = enableOffsetFadeIn;
                dn.offsetFadeIn = offsetFadeIn;
                dn.enableScaleFadeIn = enableScaleFadeIn;
                dn.scaleFadeIn = scaleFadeIn;
                dn.enableCrossScaleFadeIn = enableCrossScaleFadeIn;
                dn.crossScaleFadeIn = crossScaleFadeIn;
                dn.enableShakeFadeIn = enableShakeFadeIn;
                dn.shakeOffsetFadeIn = shakeOffsetFadeIn;
                dn.shakeFrequencyFadeIn = shakeFrequencyFadeIn;
            }

            //Fade Out:
            if (changeFadeOut)
            {
                dn.durationFadeOut = durationFadeOut;
                dn.enableOffsetFadeOut = enableOffsetFadeOut;
                dn.offsetFadeOut = offsetFadeOut;
                dn.enableScaleFadeOut = enableScaleFadeOut;
                dn.scaleFadeOut = scaleFadeOut;
                dn.enableCrossScaleFadeOut = enableCrossScaleFadeOut;
                dn.crossScaleFadeOut = crossScaleFadeOut;
                dn.enableShakeFadeOut = enableShakeFadeOut;
                dn.shakeOffsetFadeOut = shakeOffsetFadeOut;
                dn.shakeFrequencyFadeOut = shakeFrequencyFadeOut;
            }

            //Movement:
            if(changeMovement)
            {
                dn.enableLerp = enableLerp;
                dn.lerpSettings = lerpSettings;
                dn.enableVelocity = enableVelocity;
                dn.velocitySettings = velocitySettings;
                dn.enableShaking = enableShaking;
                dn.shakeSettings = shakeSettings;
                dn.enableFollowing = enableFollowing;
                dn.followSettings = followSettings;
            }

            //Rotation:
            if(changeRotation)
            {
                dn.enableStartRotation = enableStartRotation;
                dn.minRotation = minRotation;
                dn.maxRotation = maxRotation;
                dn.enableRotateOverTime = enableRotateOverTime;
                dn.minRotationSpeed = minRotationSpeed;
                dn.maxRotationSpeed = maxRotationSpeed;
                dn.rotateOverTime = rotateOverTime;
            }

            //Scale:
            if(changeScaling)
            {
                dn.enableScaleByNumber = enableScaleByNumber;
                dn.scaleByNumberSettings = scaleByNumberSettings;
                dn.enableScaleOverTime = enableScaleOverTime;
                dn.scaleOverTime = scaleOverTime;
            }

            //Spam Control:
            if(changeSpamControl)
            {
                if(dn.spamGroup == null || dn.spamGroup == "")
                {
                    dn.spamGroup = spamGroup;
                }

                dn.enableCombination = enableCombination;
                dn.combinationSettings = combinationSettings;
                dn.enableDestruction = enableDestruction;
                dn.destructionSettings = destructionSettings;
                dn.enableCollision = enableCollision;
                dn.collisionSettings = collisionSettings;
                dn.enablePush = enablePush;
                dn.pushSettings = pushSettings;
            }
        }

        public void Get(DamageNumber dn)
        {
            TMP_Text[] textMeshs = dn.GetTextMeshs();

            //Font:
            changeFontAsset = true;
            foreach (TMP_Text tmp in textMeshs)
            {
                if(tmp != null)
                {
                    fontAsset = tmp.font;
                }
            }

            //Color:
            changeColor = true;
            foreach (TMP_Text tmp in textMeshs)
            {
                if(tmp != null)
                {
                    color = tmp.color;
                    enableGradient = tmp.enableVertexGradient;
                    gradient = tmp.colorGradient;
                }
            }

            //Fade In:
            changeFadeIn = true;
            durationFadeIn = dn.durationFadeIn;
            enableOffsetFadeIn = dn.enableOffsetFadeIn;
            offsetFadeIn = dn.offsetFadeIn;
            enableScaleFadeIn = dn.enableScaleFadeIn;
            scaleFadeIn = dn.scaleFadeIn;
            enableCrossScaleFadeIn = dn.enableCrossScaleFadeIn;
            crossScaleFadeIn = dn.crossScaleFadeIn;
            enableShakeFadeIn = dn.enableShakeFadeIn;
            shakeOffsetFadeIn = dn.shakeOffsetFadeIn;
            shakeFrequencyFadeIn = dn.shakeFrequencyFadeIn;

            //Fade Out:
            changeFadeOut = true;
            durationFadeOut = dn.durationFadeOut;
            enableOffsetFadeOut = dn.enableOffsetFadeOut;
            offsetFadeOut = dn.offsetFadeOut;
            enableScaleFadeOut = dn.enableScaleFadeOut;
            scaleFadeOut = dn.scaleFadeOut;
            enableCrossScaleFadeOut = dn.enableCrossScaleFadeOut;
            crossScaleFadeOut = dn.crossScaleFadeOut;
            enableShakeFadeOut = dn.enableShakeFadeOut;
            shakeOffsetFadeOut = dn.shakeOffsetFadeOut;
            shakeFrequencyFadeOut = dn.shakeFrequencyFadeOut;

            //Movement:
            changeMovement = true;
            enableLerp = dn.enableLerp;
            lerpSettings = dn.lerpSettings;
            enableVelocity = dn.enableVelocity;
            velocitySettings = dn.velocitySettings;
            enableShaking = dn.enableShaking;
            shakeSettings = dn.shakeSettings;
            enableFollowing = dn.enableFollowing;
            followSettings = dn.followSettings;

            //Rotation:
            changeRotation = true;
            enableStartRotation = dn.enableStartRotation;
            minRotation = dn.minRotation;
            maxRotation = dn.maxRotation;
            enableRotateOverTime = dn.enableRotateOverTime;
            minRotationSpeed = dn.minRotationSpeed;
            maxRotationSpeed = dn.maxRotationSpeed;
            rotateOverTime = dn.rotateOverTime;

            //Scale:
            changeScaling = true;
            enableScaleByNumber = dn.enableScaleByNumber;
            scaleByNumberSettings = dn.scaleByNumberSettings;
            enableScaleOverTime = dn.enableScaleOverTime;
            scaleOverTime = dn.scaleOverTime;

            //Spam Group:
            changeSpamControl = true;
            spamGroup = dn.spamGroup != "" ? "Default" : "";
            enableCombination = dn.enableCombination;
            combinationSettings = dn.combinationSettings;
            enableDestruction = dn.enableDestruction;
            destructionSettings = dn.destructionSettings;
            enableCollision = dn.enableCollision;
            collisionSettings = dn.collisionSettings;
            enablePush = dn.enablePush;
            pushSettings = dn.pushSettings;
        }
    }
}
