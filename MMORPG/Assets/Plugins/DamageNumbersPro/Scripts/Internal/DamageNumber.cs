using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using TMPro;
using DamageNumbersPro.Internal;

#if ENABLE_INPUT_SYSTEM && DNP_NewInputSystem
using UnityEngine.InputSystem;
#endif

namespace DamageNumbersPro
{
    public abstract class DamageNumber : MonoBehaviour
    {
        #region Main Settings
        //Lifetime:
        [Tooltip("Damage number will not fade out on it's own.")]
        public bool permanent = false;
        [Tooltip("The lifetime after which this fades out.")]
        public float lifetime = 2f;
        [Tooltip("Ignores slow motion or game pause.")]
        public bool unscaledTime = false;

        //3D Settings:
        public bool enable3DGame = false;
        [Tooltip("Faces the camera at all times.")]
        public bool faceCameraView = true;
        [Tooltip("Uses LookAt(...) instead of the camera rotation.\nThis costs more performance but looks better in VR.\n\nNot recommended for spamming popups.")]
        public bool lookAtCamera = false;
        [Tooltip("Moves the number close to the camera and scales it down to make it look like it was visible through walls.")]
        public bool renderThroughWalls = true;
        [Tooltip("Keeps the screen-size consistent accross different distances.")]
        public bool consistentScreenSize = false;
        public DistanceScalingSettings distanceScalingSettings = new DistanceScalingSettings(0);
        [Tooltip("Override the camera looked at and scaled for.\nIf this set to None the Main Camera will be used.")]
        public Transform cameraOverride;
        #endregion

        #region Text Settings
        //Number:
        public bool enableNumber = true;
        [Tooltip("The number displayed in the text.\nCan be disabled if you only need text.")]
        public float number = 1;
        public TextSettings numberSettings = new TextSettings(0);
        public DigitSettings digitSettings = new DigitSettings(0);

        //Left Text:
        [FormerlySerializedAs("enablePrefix")]
        public bool enableLeftText = false;
        [Tooltip("Text displayed to the left of the number.")]
        [FormerlySerializedAs("prefix")]
        public string leftText = "";
        [FormerlySerializedAs("prefixSettings")]
        public TextSettings leftTextSettings = new TextSettings(0);

        //Right Text:
        [FormerlySerializedAs("enableSuffix")]
        public bool enableRightText = false;
        [Tooltip("Text displayed to the right of the number.")]
        [FormerlySerializedAs("suffix")]
        public string rightText = "";
        [FormerlySerializedAs("suffixSettings")]
        public TextSettings rightTextSettings = new TextSettings(0);

        //Top Text:
        public bool enableTopText = false;
        [Tooltip("Text displayed above the number.")]
        public string topText = "";
        public TextSettings topTextSettings = new TextSettings(0f);

        //Bottom Text:
        public bool enableBottomText = false;
        [Tooltip("Text displayed below the number.")]
        public string bottomText = "";
        public TextSettings bottomTextSettings = new TextSettings(0f);

        //Color by Number:
        public bool enableColorByNumber = false;
        public ColorByNumberSettings colorByNumberSettings = new ColorByNumberSettings(0f);
        #endregion

        #region Fade Settings
        //Fade In:
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
        #endregion

        #region Movement Settings
        //Lerping:
        public bool enableLerp = true;
        public LerpSettings lerpSettings = new LerpSettings(0);

        //Velocity:
        public bool enableVelocity = false;
        public VelocitySettings velocitySettings = new VelocitySettings(0);

        //Shaking:
        public bool enableShaking = false;
        [Tooltip("Shake settings during idle.")]
        public ShakeSettings shakeSettings = new ShakeSettings(new Vector2(0.005f, 0.005f));

        //Following:
        public bool enableFollowing = false;
        [Tooltip("Transform that will be followed.\nTries to maintain the position relative to the target.")]
        public Transform followedTarget;
        public FollowSettings followSettings = new FollowSettings(0);
        #endregion

        #region Rotation & Scale Settings
        //Start Rotation:
        public bool enableStartRotation = false;
        [Tooltip("The minimum z-angle for the random spawn rotation.")]
        public float minRotation = -4f;
        [Tooltip("The maximum z-angle for the random spawn rotation.")]
        public float maxRotation = 4f;
        public bool rotationRandomFlip = false;

        //Rotate By Time:
        public bool enableRotateOverTime = false;
        [Tooltip("The minimum rotation speed for the z-angle.")]
        public float minRotationSpeed = -15f;
        [Tooltip("The maximum rotation speed for the z-angle.")]
        public float maxRotationSpeed = 15;
        public bool rotationSpeedRandomFlip = false;
        [Tooltip("Defines rotation speed over lifetime.")]
        public AnimationCurve rotateOverTime = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(0.4f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0) });

        //Scale By Number:
        public bool enableScaleByNumber = false;
        public ScaleByNumberSettings scaleByNumberSettings = new ScaleByNumberSettings(0);

        //Scale By Time:
        public bool enableScaleOverTime = false;
        [Tooltip("Will scale over it's lifetime using this curve.")]
        public AnimationCurve scaleOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.7f));
        #endregion

        #region Spam Control Settings
        [Tooltip("The group of numbers which will affect each other using the features bellow.")]
        public string spamGroup = "";

        //Combination:
        public bool enableCombination = false;
        public CombinationSettings combinationSettings = new CombinationSettings(0);

        //Destruction:
        public bool enableDestruction = false;
        public DestructionSettings destructionSettings = new DestructionSettings(0);

        //Collision:
        public bool enableCollision = false;
        public CollisionSettings collisionSettings = new CollisionSettings(0);

        //Push:
        public bool enablePush = false;
        public PushSettings pushSettings = new PushSettings(0);
        #endregion

        #region Performance Settings
        //Update Delay:
        public float updateDelay = 0.0125f;

        //Pooling:
        public bool enablePooling = false;
        [Tooltip("Maximum of damage numbers stored in pool.")]
        public int poolSize = 50;
        [Tooltip("Pooled damage numbers are not destroyed on load.\nThis option will fade them out on load.\nSo you don't see popups from the previous scene.")]
        public bool disableOnSceneLoad = true;
        #endregion

        #region Editor Variables
        /// <summary>
        /// Please ignore this variable, it's used by the editor.
        /// </summary>
        public string editorLastFont;
        #endregion

        //References:
        TextMeshPro textMeshPro;
        MeshRenderer textMeshRenderer;
        MeshRenderer meshRendererA;
        MeshRenderer meshRendererB;
        MeshFilter meshFilterA;
        MeshFilter meshFilterB;
        protected Transform transformA;
        protected Transform transformB;
        List<System.Tuple<MeshRenderer, MeshRenderer>> subMeshRenderers;
        List<System.Tuple<MeshFilter, MeshFilter>> subMeshFilters;
        protected List<Mesh> meshs;
        protected List<Color[]> colors;
        protected List<float[]> alphas;

        //Fading:
        protected float currentFade;
        protected float startTime;
        float startLifeTime;
        protected float currentLifetime;
        float fadeInSpeed;
        float fadeOutSpeed;
        protected float baseAlpha;
        Vector2 currentScaleInOffset;
        Vector2 currentScaleOutOffset;

        //Position:
        public Vector3 position;
        Vector3 finalPosition;
        protected Vector3 remainingOffset;
        protected Vector2 currentVelocity;

        //Scaling:
        protected Vector3 originalScale;
        float numberScale;
        float combinationScale;
        float destructionScale;
        float renderThroughWallsScale = 0.1f;
        float lastScaleFactor = 1f;
        bool firstFrameScale;

        //Rotation:
        float currentRotationSpeed;
        float currentRotation;

        //Following:
        Vector3 lastTargetPosition;
        Vector3 targetOffset;
        float currentFollowSpeed;

        //Spam Control:
        static Dictionary<string, HashSet<DamageNumber>> spamGroupDictionary;
        bool removedFromDictionary;

        //Combination:
        DamageNumber myAbsorber;
        bool givenNumber;
        float absorbStartTime;
        Vector3 absorbStartPosition;

        //3D:
        Transform targetCamera;
        float simulatedScale;

        //Destruction:
        bool isDestroyed;
        float destructionStartTime;

        //Collision & Push:
        bool collided;
        bool pushed;

        //Pooling:
        DamageNumber originalPrefab;
        public static Transform poolParent;
        static Dictionary<int, HashSet<DamageNumber>> pools;
        int poolingID;
        bool performRestart;
        bool destroyAfterSpawning;

        //Fallback font fix.
        static Dictionary<TMP_FontAsset, GameObject> fallbackDictionary;

        //Custom Events:
        protected bool isFadingOut;

        void Start()
        {
            //Once:
            GetReferencesIfNecessary();

            if(enablePooling && disableOnSceneLoad)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            //Repeated for Pooling:
            Restart();
        }

        void Update()
        {
            //For Pooling:
            if (performRestart)
            {
                Restart();
                performRestart = false;
            }
        }

        void LateUpdate()
        {
            if (!performRestart)
            {
                UpdateScaleAnd3D();
            }

            OnLateUpdate();
        }

        /// <summary>
        /// This is called by DNPUpdater for improved performance.
        /// You can ignore this function.
        /// </summary>
        public void UpdateDamageNumber(float delta, float time)
        {
            //Check activity.
            if(gameObject.activeInHierarchy == false)
            {
                startTime += delta;
                startLifeTime += delta;
                absorbStartTime += delta;
                destructionStartTime += delta;
                return;
            }

            //Vectors:
            if(DNPUpdater.vectorsNeedUpdate)
            {
                DNPUpdater.UpdateVectors(transform);
            }

            //Fading:
            if (IsAlive(time))
            {
                HandleFadeIn(delta);
            }
            else
            {
                HandleFadeOut(delta);
            }

            //Custom Event:
            OnUpdate(delta);

            //Movement:
            if (enableLerp)
            {
                HandleLerp(delta);
            }
            if (enableVelocity)
            {
                HandleVelocity(delta);
            }
            if (enableFollowing)
            {
                HandleFollowing(delta);
            }

            //Rotation:
            if (enableRotateOverTime)
            {
                HandleRotateOverTime(delta, time);
                UpdateRotationZ();
            }

            //Offset:
            finalPosition = position;
            if (enableShaking)
            {
                finalPosition = ApplyShake(finalPosition, shakeSettings, time);
            }

            //Combination:
            if (enableCombination)
            {
                HandleCombination(delta, time);
            }

            //Destruction:
            if (enableDestruction)
            {
                HandleDestruction(time);
            }

            //Apply Transform:
            SetPosition(finalPosition);
        }

        #region Spawn Functions
        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// </summary>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn()
        {
            DamageNumber newDN = default;
            int instanceID = GetInstanceID();

            //Check Pool:
            if (enablePooling && PoolAvailable(instanceID))
            {
                //Get from Pool:
                foreach (DamageNumber dn in pools[instanceID])
                {
                    newDN = dn; //This is the only way I can get a unknown element from a hashset, using a single loop iteration.
                    break;
                }
                pools[instanceID].Remove(newDN);
            }
            else
            {
                //Create New:
                GameObject newGO = Instantiate<GameObject>(gameObject);
                newDN = newGO.GetComponent<DamageNumber>();

                if (enablePooling)
                {
                    newDN.originalPrefab = this;
                }
            }

            newDN.gameObject.SetActive(true); //Active Gameobject
            newDN.OnPreSpawn();

            if (enablePooling)
            {
                newDN.SetPoolingID(instanceID);
                newDN.destroyAfterSpawning = false;
            }

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetPosition(newPosition);

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// Also sets the popup's number.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <param name="newNumber">The displayed number of this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition, float newNumber)
        {
            DamageNumber newDN = Spawn(newPosition);

            //Number:
            newDN.enableNumber = true;
            newDN.number = newNumber;

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// Also sets the popup's number and makes it follow a transform.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <param name="newNumber">The displayed number of this popup.</param>
        /// <param name="followedTransform">The transform, which this popup should follow.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition, float newNumber, Transform followedTransform)
        {
            DamageNumber newDN = Spawn(newPosition, newNumber);

            //Following:
            newDN.SetFollowedTarget(followedTransform);

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// Also makes the popup follow a transform.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <param name="followedTransform">The transform, which this popup should follow.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition, Transform followedTransform)
        {
            DamageNumber newDN = Spawn(newPosition);

            //Following:
            newDN.SetFollowedTarget(followedTransform);

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// Also sets the popup's text and disables the number.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <param name="newText">The text displayed by this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition, string newText)
        {
            DamageNumber newDN = Spawn(newPosition);

            //Disable Number:
            newDN.enableNumber = false;

            //Text:
            newDN.enableLeftText = true;
            newDN.leftText = newText;

            return newDN;
        }

        /// <summary>
        /// Spawns a new popup and handles pooling.
        /// Also sets the popup's text and makes it follow a transform.
        /// Disables the number.
        /// </summary>
        /// <param name="newPosition">The worldspace position of this popup.</param>
        /// <param name="newText">The text displayed by this popup.</param>
        /// <param name="followedTransform">The transform, which this popup should follow.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber Spawn(Vector3 newPosition, string newText, Transform followedTransform)
        {
            DamageNumber newDN = Spawn(newPosition, newText);

            //Following:
            newDN.SetFollowedTarget(followedTransform);

            return newDN;
        }
        #endregion

        #region GUI Spawn Functions
        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="anchoredPosition">The anchored position relative to rectParent.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, Vector2 anchoredPosition)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetAnchoredPosition(rectParent, anchoredPosition);

            return newDN;
        }

        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="rectPosition">The RectTransform, which this popup's anchored position is relative to.</param>
        /// <param name="anchoredPosition">This popup's anchored position relative to rectPosition.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, RectTransform rectPosition, Vector2 anchoredPosition)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetAnchoredPosition(rectParent, rectPosition, anchoredPosition);

            return newDN;
        }

        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="anchoredPosition">The anchored position relative to rectParent.</param>
        /// <param name="newNumber">The displayed number of this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, Vector2 anchoredPosition, float newNumber)
        {
            DamageNumber newDN = SpawnGUI(rectParent, anchoredPosition);

            //Number:
            newDN.enableNumber = true;
            newDN.number = newNumber;

            return newDN;
        }

        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="rectPosition">The RectTransform, which this popup's anchored position is relative to.</param>
        /// <param name="anchoredPosition">This popup's anchored position relative to rectPosition.</param>
        /// <param name="newNumber">The displayed number of this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, RectTransform rectPosition, Vector2 anchoredPosition, float newNumber)
        {
            DamageNumber newDN = SpawnGUI(rectParent, rectPosition, anchoredPosition);

            //Number:
            newDN.enableNumber = true;
            newDN.number = newNumber;

            return newDN;
        }

        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="anchoredPosition">The anchored position relative to rectParent.</param>
        /// <param name="newText">The text displayed by this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, Vector2 anchoredPosition, string newText)
        {
            DamageNumber newDN = SpawnGUI(rectParent, anchoredPosition);

            //Disable Number:
            newDN.enableNumber = false;

            //Text:
            newDN.enableLeftText = true;
            newDN.leftText = newText;

            return newDN;
        }

        /// <summary>
        /// Spawns a new GUI popup and handles pooling.
        /// Use this function for DamageNumberGUI only.
        /// </summary>
        /// <param name="rectParent">The RectTransform, which this popup should be parented to. Spam Control features like Combination only work under the same parent.</param>
        /// <param name="rectPosition">The RectTransform, which this popup's anchored position is relative to.</param>
        /// <param name="anchoredPosition">This popup's anchored position relative to rectPosition.</param>
        /// <param name="newText">The text displayed by this popup.</param>
        /// <returns>The spawned popup, which can be modified at runtime.</returns>
        public DamageNumber SpawnGUI(RectTransform rectParent, RectTransform rectPosition, Vector2 anchoredPosition, string newText)
        {
            DamageNumber newDN = SpawnGUI(rectParent, rectPosition, anchoredPosition);

            //Disable Number:
            newDN.enableNumber = false;

            //Text:
            newDN.enableLeftText = true;
            newDN.leftText = newText;

            return newDN;
        }
        #endregion

        #region Removed Functions
        /*
        public DamageNumber Spawn(Vector3 newPosition, float newNumber, Color newColor)
        {
            DamageNumber newDN = Spawn(newPosition, newNumber);

            //Position:
            newDN.SetPosition(newPosition);

            //Number:
            newDN.number = newNumber;

            //Color:
            newDN.SetColor(newColor);

            return newDN;
        }

        public DamageNumber Spawn(Vector3 newPosition, string newLeftText, Color newColor)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetPosition(newPosition);

            //Number:
            newDN.enableLeftText = true;
            newDN.leftText = newLeftText;

            //Color:
            newDN.SetColor(newColor);

            return newDN;
        }

        public DamageNumber Spawn(Vector3 newPosition, float newNumber, Transform followedTransform, Color newColor)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetPosition(newPosition);

            //Text:
            newDN.number = newNumber;

            //Following:
            newDN.SetFollowedTarget(followedTransform);

            //Color:
            newDN.SetColor(newColor);

            return newDN;
        }

        public DamageNumber Spawn(Vector3 newPosition, string newLeftText, Transform followedTransform, Color newColor)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetPosition(newPosition);

            //Text:
            newDN.enableLeftText = true;
            newDN.leftText = newLeftText;

            //Following:
            newDN.SetFollowedTarget(followedTransform);

            //Color:
            newDN.SetColor(newColor);

            return newDN;
        }

        public DamageNumber Spawn(Transform rectParent, Vector2 anchoredPosition)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetAnchoredPosition(rectParent, anchoredPosition);

            return newDN;
        }

        public DamageNumber Spawn(Transform rectParent, Vector2 anchoredPosition, float number)
        {
            DamageNumber newDN = Spawn();

            //Number:
            newDN.number = number;

            //Position:
            newDN.SetAnchoredPosition(rectParent, anchoredPosition);

            return newDN;
        }

        public DamageNumber Spawn(Transform rectParent, Transform rectPosition, Vector2 anchoredPosition, float number)
        {
            DamageNumber newDN = Spawn();

            //Number:
            newDN.number = number;

            //Position:
            newDN.SetAnchoredPosition(rectParent, rectPosition, anchoredPosition);

            return newDN;
        }

        public DamageNumber Spawn(Transform rectParent, Transform rectPosition, Vector2 anchoredPosition)
        {
            DamageNumber newDN = Spawn();

            //Position:
            newDN.SetAnchoredPosition(rectParent, rectPosition, anchoredPosition);

            return newDN;
        }
        */
        #endregion

        #region Public Functions
        /// <summary>
        /// Makes the damage number follow a transform.
        /// Will also modify the spamGroup so that only damage numbers following this taget interact with each other.
        /// </summary>
        public void SetFollowedTarget(Transform followedTransform)
        {
            //Following:
            enableFollowing = true;
            followedTarget = followedTransform;

            //Spam Group:
            spamGroup += followedTransform.GetInstanceID();
        }
        public void SetColor(Color newColor)
        {
            //References:
            GetReferencesIfNecessary();

            //Set Color:
            foreach (TMP_Text tmp in GetTextMeshs())
            {
                tmp.color = newColor;
            }
        }
        public void SetGradientColor(VertexGradient newGradient)
        {
            //References:
            GetReferencesIfNecessary();

            //Set Gradient:
            foreach (TMP_Text tmp in GetTextMeshs())
            {
                tmp.enableVertexGradient = true;
                tmp.colorGradient = newGradient;
            }
        }
        public void SetRandomColor(Color from, Color to)
        {
            SetColor(Color.Lerp(from, to, Random.value));
        }
        public void SetRandomColor(Gradient gradient)
        {
            SetColor(gradient.Evaluate(Random.value));
        }
        public void SetGradientColor(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
        {
            VertexGradient newGradient = new VertexGradient();
            newGradient.topLeft = topLeft;
            newGradient.topRight = topRight;
            newGradient.bottomLeft = bottomLeft;
            newGradient.bottomRight = bottomRight;

            SetGradientColor(newGradient);
        }
        public void SetFontMaterial(TMP_FontAsset font)
        {
            //References:
            GetReferencesIfNecessary();

            //Set Font:
            foreach (TMP_Text tmp in GetTextMeshs())
            {
                tmp.font = font;
            }
        }
        public TMP_FontAsset GetFontMaterial()
        {
            //References:
            GetReferencesIfNecessary();

            //Get Font:
            foreach (TMP_Text tmp in GetTextMeshs())
            {
                if (tmp.font != null)
                {
                    return tmp.font;
                }
            }

            return null;
        }
        public void SetScale(float newScale)
        {
            originalScale = transform.localScale = new Vector3(newScale, newScale, newScale);
        }

        public virtual Vector3 GetUpVector()
        {
            return DNPUpdater.upVector;
        }
        public virtual Vector3 GetRightVector()
        {
            return DNPUpdater.rightVector;
        }
        public virtual Vector3 GetFreshUpVector()
        {
            return transform.up;
        }
        public virtual Vector3 GetFreshRightVector()
        {
            return transform.right;
        }

        #endregion

        #region Utility Functions
        public virtual void GetReferences()
        {
            baseAlpha = 1f;

            textMeshPro = transform.Find("TMP").GetComponent<TextMeshPro>();
            textMeshRenderer = textMeshPro.GetComponent<MeshRenderer>();
            transformA = transform.Find("MeshA");
            transformB = transform.Find("MeshB");
            meshRendererA = transformA.GetComponent<MeshRenderer>();
            meshRendererB = transformB.GetComponent<MeshRenderer>();
            meshFilterA = transformA.GetComponent<MeshFilter>();
            meshFilterB = transformB.GetComponent<MeshFilter>();

            subMeshRenderers = new List<System.Tuple<MeshRenderer, MeshRenderer>>();
            subMeshFilters = new List<System.Tuple<MeshFilter, MeshFilter>>();

            Transform parentA = meshRendererA.transform;
            Transform parentB = meshRendererB.transform;
            for (int n = 0; n < parentA.childCount; n++)
            {
                Transform childA = parentA.GetChild(n);
                Transform childB = parentB.GetChild(n);
                subMeshRenderers.Add(new System.Tuple<MeshRenderer, MeshRenderer>(childA.GetComponent<MeshRenderer>(), childB.GetComponent<MeshRenderer>()));
                subMeshFilters.Add(new System.Tuple<MeshFilter, MeshFilter>(childA.GetComponent<MeshFilter>(), childB.GetComponent<MeshFilter>()));
            }
        }

        public virtual void GetReferencesIfNecessary()
        {
            if (textMeshPro == null || subMeshRenderers == null)
            {
                GetReferences();
            }
        }

        /// <summary>
        /// Starts fading out this damage number.
        /// Use this to fade out damage numbers early.
        /// </summary>
        public void FadeOut()
        {
            permanent = false;
            startLifeTime = -1000;
        }

        /// <summary>
        /// Restarts the fade-in animation.
        /// Could be used for fixed gui texts with permanent lifetime.
        /// </summary>
        public void FadeIn()
        {
            currentFade = 0;
        }

        /// <summary>
        /// Returns 1 text mesh pro component on the mesh version.
        /// Returns 2 text mesh pro components on the gui version.
        /// </summary>
        /// <returns></returns>
        public virtual TMP_Text[] GetTextMeshs()
        {
            return new TMP_Text[] { textMeshPro };
        }

        /// <summary>
        /// Returns the text mesh pro component.
        /// Use this to change text mesh pro settings at runtime.
        /// </summary>
        /// <returns></returns>
        public virtual TMP_Text GetTextMesh()
        {
            return textMeshPro;
        }

        public virtual Material[] GetSharedMaterials()
        {
            return textMeshRenderer.sharedMaterials;
        }
        public virtual Material[] GetMaterials()
        {
            return textMeshRenderer.materials;
        }
        public virtual Material GetSharedMaterial()
        {
            return textMeshRenderer.sharedMaterial;
        }
        public virtual Material GetMaterial()
        {
            return textMeshRenderer.material;
        }

        /// <summary>
        /// Input Time.time or Time.unscaledTime (depending on the time setting in main settings).
        /// Returns if damage number is still alive.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual bool IsAlive(float time)
        {
            if (permanent)
            {
                return true;
            }

            return time - startLifeTime < currentLifetime;
        }

        /// <summary>
        /// Use this function to manually destroy a damage number.
        /// This will also handle pooling.
        /// </summary>
        public void DestroyDNP()
        {
            //Updater:
            DNPUpdater.UnregisterPopup(unscaledTime, updateDelay, this);

            //Pooling / Destroying:
            if (enablePooling && originalPrefab != null)
            {
                if (pools == null)
                {
                    pools = new Dictionary<int, HashSet<DamageNumber>>();
                }

                if (!pools.ContainsKey(poolingID))
                {
                    pools.Add(poolingID, new HashSet<DamageNumber>());
                }

                RemoveFromDictionary();

                if (pools[poolingID].Count < poolSize)
                {
                    PreparePooling();
                }
                else
                {
                    Destroy(gameObject); //Not enough pool space.
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public virtual void CheckAndEnable3D()
        {
            //Dimension Check:
            Camera camera = Camera.main;

            if (camera == null)
            {
                camera = Camera.current;
            }

            if (camera != null)
            {
                if (!camera.orthographic)
                {
                    enable3DGame = true;
                }
            }
        }

        /// <summary>
        /// Returns true if this is a Mesh or false if this is a GUI Component.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMesh()
        {
            return true;
        }
        public static GameObject NewMesh(string tmName, Transform parent)
        {
            //GameObject:
            GameObject newTM = new GameObject();
            newTM.name = tmName;
            newTM.layer = parent.gameObject.layer;

            //Mesh:
            MeshRenderer mr = newTM.AddComponent<MeshRenderer>();
            newTM.AddComponent<MeshFilter>();
            mr.receiveShadows = false;
            mr.allowOcclusionWhenDynamic = false;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.lightProbeUsage = LightProbeUsage.Off;
            mr.reflectionProbeUsage = ReflectionProbeUsage.Off;

            //Transform:
            Transform transform = newTM.transform;
            transform.SetParent(parent, true);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;

            return newTM;
        }
        #endregion

        #region Pooling
        /// <summary>
        /// Use this function ONCE PER PREFAB to prewarm it's pool at the start of your game.
        /// It will generate enough damage numbers to fill the pool size.
        /// </summary>
        public void PrewarmPool()
        {
            if (enablePooling)
            {
                int instanceId = GetInstanceID();

                //Initialize Dictionary:
                if (pools == null)
                {
                    pools = new Dictionary<int, HashSet<DamageNumber>>();
                }

                //Initialize Pool:
                if (!pools.ContainsKey(instanceId))
                {
                    pools.Add(instanceId, new HashSet<DamageNumber>());
                }

                //Fill Pool:
                int amount = poolSize - pools[instanceId].Count;
                if(amount > poolSize * 0.5f)
                {
                    for (int n = 0; n < amount; n++)
                    {
                        DamageNumber dn = Spawn(new Vector3(-9999, -9999, -9999));
                        dn.destroyAfterSpawning = true;
                    }
                }
            }
        }

        protected void Restart()
        {
            //Updater:
            DNPUpdater.RegisterPopup(unscaledTime, updateDelay, this);

            //Get Scale:
            if (originalScale.x < 0.02f)
            {
                originalScale = transform.localScale;
            }

            //Fix Fading Scale:
            transformA.localScale = transformB.localScale = Vector3.one;

            //Custom Event:
            OnStart();

            #region Fallback Fix
            if (IsMesh())
            {
                //Create fallback dictionary.
                if (fallbackDictionary == null)
                {
                    fallbackDictionary = new Dictionary<TMP_FontAsset, GameObject>();
                }

                //Get font material.
                TMP_FontAsset fontAsset = GetFontMaterial();

                //Check if in dictionary.
                if (!fallbackDictionary.ContainsKey(fontAsset) && fontAsset != null)
                {
                    bool usesFallbackFonts = fontAsset.fallbackFontAssetTable != null && fontAsset.fallbackFontAssetTable.Count > 0;
                    if (fontAsset.isMultiAtlasTexturesEnabled || usesFallbackFonts)
                    { 
                        //New tmp for fallback assets.
                        GameObject fallbackAsset = Instantiate<GameObject>(textMeshPro.gameObject);
                        fallbackAsset.transform.localScale = Vector3.zero;
                        fallbackAsset.SetActive(true);
                        fallbackAsset.hideFlags = HideFlags.HideAndDontSave;
                        DontDestroyOnLoad(fallbackAsset);

                        //Create base string containing a single character of the base font.
                        string textString = "" + (char)fontAsset.characterTable[0].unicode;

                        //Add all characters to support multi-atlas fonts.
                        if (fontAsset.isMultiAtlasTexturesEnabled)
                        {
                            foreach (TMP_Character character in fontAsset.characterTable)
                            {
                                textString += (char)character.unicode;
                            }
                        }

                        //Create a new string containing various unicode characters of the fallback fonts.
                        if (usesFallbackFonts)
                        {
                            for (int f = 0; f < fontAsset.fallbackFontAssetTable.Count; f++)
                            {
                                TMP_FontAsset fallbackFont = fontAsset.fallbackFontAssetTable[f];

                                if (fallbackFont != null && fallbackFont.characterTable != null)
                                {
                                    foreach (TMP_Character fallbackCharacter in fallbackFont.characterTable)
                                    {
                                        if (fallbackCharacter != null)
                                        {
                                            if (fontAsset.characterLookupTable.ContainsKey(fallbackCharacter.unicode))
                                            {
                                                //Character already in main font.
                                            }
                                            else
                                            {
                                                bool addCharacter = true;

                                                for (int pF = 0; pF < f; pF++)
                                                {
                                                    TMP_FontAsset previousFallbackFont = fontAsset.fallbackFontAssetTable[pF];
                                                    if (previousFallbackFont != null && previousFallbackFont.characterLookupTable.ContainsKey(fallbackCharacter.unicode))
                                                    {
                                                        //Character already in a higher priority fallback font.
                                                        addCharacter = false;
                                                        break;
                                                    }
                                                }

                                                if (addCharacter)
                                                {
                                                    textString += (char)fallbackCharacter.unicode;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Assign text and add to dictionary.
                        fallbackAsset.GetComponent<TextMeshPro>().text = textString;
                        fallbackDictionary.Add(fontAsset, fallbackAsset);
                    }
                    else
                    {
                        fallbackDictionary.Add(fontAsset, null);
                    }
                }
            }
            #endregion

            //Called right after spawn:
            float time = unscaledTime ? Time.unscaledTime : Time.time;
            Initialize(time);

            //Spam Control:
            if (spamGroup != "")
            {
                TryCombination(time);
                TryDestruction(time);
                TryCollision();
                TryPush();
            }

            //Scale to Zero:
            firstFrameScale = true;
            if(destroyAfterSpawning)
            {
                destroyAfterSpawning = false;
                startLifeTime = -100;
            }
        }
        void Initialize(float time)
        {
            numberScale = destructionScale = combinationScale = currentFollowSpeed = 1f;
            baseAlpha = 1f;
            finalPosition = position = GetPosition();
            startLifeTime = startTime = time;
            currentLifetime = lifetime;
            isFadingOut = false;

            //3D Game:
            if (enable3DGame)
            {
                if (cameraOverride != null)
                {
                    targetCamera = cameraOverride;
                }
                else if (Camera.main != null)
                {
                    targetCamera = Camera.main.transform;
                }
                else if (Camera.current != null)
                {
                    targetCamera = Camera.current.transform;
                }
            }

            //Scale:
            UpdateScaleAnd3D();

            //Rotation:
            if (enableRotateOverTime)
            {
                currentRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

                if(rotationSpeedRandomFlip && Random.value < 0.5f)
                {
                    currentRotationSpeed *= -1;
                }
            }

            //Spam Groups:
            AddToDictionary();

            //Lerp:
            if (enableLerp)
            {
                float xOffset = Random.Range(lerpSettings.minX, lerpSettings.maxX) * GetPositionFactor();
                if(lerpSettings.randomFlip && Random.value < 0.5f)
                {
                    xOffset = -xOffset;
                }

                remainingOffset = GetFreshRightVector() * xOffset + GetFreshUpVector() * (Random.Range(lerpSettings.minY, lerpSettings.maxY) * GetPositionFactor());
            }

            //Velocity:
            if (enableVelocity)
            {
                currentVelocity = new Vector2(Random.Range(velocitySettings.minX, velocitySettings.maxX), Random.Range(velocitySettings.minY, velocitySettings.maxY)) * GetPositionFactor();

                if(velocitySettings.randomFlip && Random.value < 0.5f)
                {
                    currentVelocity.x = -currentVelocity.x;
                }
            }

            //Start Rotation:
            if (enableStartRotation)
            {
                currentRotation = Random.Range(minRotation, maxRotation);

                if (rotationRandomFlip && Random.value < 0.5f)
                {
                    currentRotation *= -1;
                }
            }
            else
            {
                currentRotation = 0;
            }

            //Fading:
            currentFade = durationFadeIn > 0f ? 0f : 1f;

            //Fade In:
            fadeInSpeed = 1f / Mathf.Max(0.0001f, durationFadeIn);
            if (enableCrossScaleFadeIn)
            {
                currentScaleInOffset = crossScaleFadeIn;
                if (currentScaleInOffset.x == 0) currentScaleInOffset.x += 0.001f;
                if (currentScaleInOffset.y == 0) currentScaleInOffset.y += 0.001f;
            }

            //Fade Out:
            fadeOutSpeed = 1f / Mathf.Max(0.0001f, durationFadeOut);
            if (enableCrossScaleFadeOut)
            {
                currentScaleOutOffset = crossScaleFadeOut;
                if (currentScaleOutOffset.x == 0) currentScaleOutOffset.x += 0.001f;
                if (currentScaleOutOffset.y == 0) currentScaleOutOffset.y += 0.001f;
            }

            lastTargetPosition = Vector3.zero;

            //Update Text:
            UpdateText();

            //Update Rotation:
            UpdateRotationZ();
        }

        void PreparePooling()
        {
            //Add to Pool:
            pools[poolingID].Add(this);

            //Disable GameObject:
            gameObject.SetActive(false);

            //Queue Restart:
            performRestart = true;

            //Reset Runtime Variables:
            transform.localScale = originalScale;
            lastTargetPosition = targetOffset = Vector3.zero;

            //Clear Combination Targets:
            myAbsorber = null;

            //Reset some Setting Variables:
            spamGroup = originalPrefab.spamGroup;
            leftText = originalPrefab.leftText;
            rightText = originalPrefab.rightText;
            followedTarget = originalPrefab.followedTarget;
            enableCollision = originalPrefab.enableCollision;
            enablePush = originalPrefab.enablePush;
        }

        bool PoolAvailable(int id)
        {
            if (pools != null && pools.ContainsKey(id))
            {
                if (pools[id].Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        void SetPoolingID(int id)
        {
            poolingID = id;

            //Initiate Dictionaries:
            if (pools == null)
            {
                pools = new Dictionary<int, HashSet<DamageNumber>>();
            }

            //Initiate Pool Parent:
            if (poolParent == null)
            {
                GameObject poolGameobject = new GameObject("Damage Number Pool");
                DontDestroyOnLoad(poolGameobject);

                poolParent = poolGameobject.transform;
                poolParent.localScale = Vector3.one;
                poolParent.eulerAngles = poolParent.position = Vector3.zero;
            }

            //Parent:
            transform.SetParent(poolParent, true);
        }
        #endregion

        #region Text
        public void UpdateText()
        {
            //Number:
            string numberText = "";
            if (enableNumber)
            {
                string numberString;
                bool shortened;

                if (digitSettings.decimals <= 0)
                {
                    numberString = ProcessIntegers(Mathf.RoundToInt(number).ToString(), out shortened);
                }
                else
                {
                    //Digits:
                    string allDigits = Mathf.RoundToInt(Mathf.Abs(number) * Mathf.Pow(10, digitSettings.decimals)).ToString();

                    bool hasMinus = number < 0;

                    //Add zeros to the left to fix numbers less than 1.
                    int usedDecimals = digitSettings.decimals;
                    int currentLength = allDigits.Length;
                    if(currentLength < usedDecimals)
                    {
                        for(int i = 0; i < usedDecimals - currentLength; i++)
                        {
                            allDigits = "0" + allDigits;
                        }
                    }

                    while (digitSettings.hideZeros && allDigits.EndsWith("0") && usedDecimals > 0)
                    {
                        allDigits = allDigits.Substring(0, allDigits.Length - 1);
                        usedDecimals--;
                    }

                    string integers = allDigits.Substring(0, Mathf.Max(0, allDigits.Length - usedDecimals));

                    integers = ProcessIntegers(integers, out shortened);

                    if (integers == "")
                    {
                        integers = "0";
                    }

                    int digitLength = allDigits.Length;
                    while (digitLength < usedDecimals)
                    {
                        if(digitSettings.hideZeros)
                        {
                            usedDecimals--;
                        }
                        else
                        {
                            allDigits += "0";
                            digitLength = allDigits.Length;
                        }
                    }

                    string decimals = allDigits.Substring(allDigits.Length - usedDecimals);

                    if (usedDecimals > 0 && !shortened)
                    {
                        numberString = integers + digitSettings.decimalChar + decimals;
                    }
                    else
                    {
                        numberString = integers;
                    }

                    if (hasMinus)
                    {
                        numberString = "-" + numberString;
                    }
                }

                numberText = ApplyTextSettings(numberString, numberSettings);

                if (enableScaleByNumber)
                {
                    numberScale = scaleByNumberSettings.fromScale + (scaleByNumberSettings.toScale - scaleByNumberSettings.fromScale) * Mathf.Clamp01((number - scaleByNumberSettings.fromNumber) / (scaleByNumberSettings.toNumber - scaleByNumberSettings.fromNumber));
                }

                if(enableColorByNumber)
                {
                    SetColor(colorByNumberSettings.colorGradient.Evaluate(Mathf.Clamp01((number - colorByNumberSettings.fromNumber) / (colorByNumberSettings.toNumber - colorByNumberSettings.fromNumber))));
                }
            }

            //Prefix:
            string prefixText = "";
            if (enableTopText)
            {
                prefixText += ApplyTextSettings(topText, topTextSettings) + System.Environment.NewLine;
            }
            if (enableLeftText)
            {
                prefixText += ApplyTextSettings(leftText, leftTextSettings);
            }

            //Suffix:
            string suffixText = "";
            if (enableRightText)
            {
                suffixText += ApplyTextSettings(rightText, rightTextSettings);
            }
            if (enableBottomText)
            {
                suffixText += System.Environment.NewLine + ApplyTextSettings(bottomText, bottomTextSettings);
            }

            GetReferencesIfNecessary();

            //Scale Fix:
            Vector3 currentLocalScale = transform.localScale;
            if(!enable3DGame || !renderThroughWalls)
            {
                renderThroughWallsScale = 1f;
            }
            if (lastScaleFactor < 1)
            {
                lastScaleFactor = 1f;
            }
            float minScale = renderThroughWallsScale * lastScaleFactor;
            if (currentLocalScale.x < minScale)
            {
                transform.localScale = new Vector3(minScale, minScale, minScale);
            }

            //Update Text:
            SetTextString(prefixText + numberText + suffixText);
            transform.localScale = currentLocalScale;

            //Get Colors:
            colors = new List<Color[]>();
            alphas = new List<float[]>();

            for (int n = 0; n < meshs.Count; n++)
            {
                Mesh mesh = meshs[n];

                if(mesh != null)
                {
                    Color[] color = mesh.colors;
                    float[] alpha = new float[color.Length];

                    for (int c = 0; c < color.Length; c++)
                    {
                        alpha[c] = color[c].a;
                    }

                    alphas.Add(alpha);
                    colors.Add(color);
                }
                else
                {
                    colors.Add(new Color[0]);
                    alphas.Add(new float[0]);
                }
            }

            //Finish:
            UpdateAlpha(currentFade);
            OnTextUpdate();
        }

        protected virtual void SetTextString(string fullString)
        {
            //Generate Mesh:
            textMeshPro.gameObject.SetActive(true);
            textMeshPro.text = fullString;
            textMeshPro.ForceMeshUpdate();

            //Clear Meshs:
            ClearMeshs();

            meshs = new List<Mesh>();
            meshs.Add(Instantiate<Mesh>(textMeshPro.mesh));
            meshFilterA.mesh = meshFilterB.mesh = meshs[0];
            meshRendererA.sharedMaterials = meshRendererB.sharedMaterials = textMeshRenderer.sharedMaterials;

            //Submeshes:
            int usedSubMeshes = 0;
            for (int c = 0; c < textMeshPro.transform.childCount; c++)
            {
                MeshRenderer subMeshRenderer = textMeshPro.transform.GetChild(c).GetComponent<MeshRenderer>();

                if (subMeshRenderer != null)
                {
                    MeshFilter subMeshFilter = subMeshRenderer.GetComponent<MeshFilter>();

                    //Create new Submesh:
                    if (subMeshRenderers.Count <= c)
                    {
                        GameObject subMeshA = NewMesh("Sub", meshRendererA.transform);
                        GameObject subMeshB = NewMesh("Sub", meshRendererB.transform);
                        subMeshRenderers.Add(new System.Tuple<MeshRenderer, MeshRenderer>(subMeshA.GetComponent<MeshRenderer>(), subMeshB.GetComponent<MeshRenderer>()));
                        subMeshFilters.Add(new System.Tuple<MeshFilter, MeshFilter>(subMeshA.GetComponent<MeshFilter>(), subMeshB.GetComponent<MeshFilter>()));
                    }

                    //Apply:
                    System.Tuple<MeshRenderer, MeshRenderer> subRenderers = subMeshRenderers[c];
                    System.Tuple<MeshFilter, MeshFilter> subFilters = subMeshFilters[c];
                    subRenderers.Item1.sharedMaterials = subRenderers.Item2.sharedMaterials = subMeshRenderer.sharedMaterials;

                    Mesh newSubMesh = Instantiate<Mesh>(subMeshFilter.sharedMesh);
                    subFilters.Item1.mesh = subFilters.Item2.mesh = newSubMesh;
                    meshs.Add(newSubMesh);

                    //Enable:
                    subRenderers.Item1.transform.localScale = subRenderers.Item2.transform.localScale = Vector3.one;

                    usedSubMeshes++;
                }
            }

            //Hide Unused Meshs:
            for (int n = usedSubMeshes; n < subMeshRenderers.Count; n++)
            {
                subMeshRenderers[n].Item1.transform.localScale = subMeshRenderers[n].Item2.transform.localScale = Vector3.zero;
            }

            //Disable TMP:
            textMeshPro.gameObject.SetActive(false);
        }

        string ProcessIntegers(string integers, out bool shortened)
        {
            shortened = false;

            //Short Suffix:
            if (digitSettings.suffixShorten)
            {
                int currentSuffix = -1;

                while (integers.Length > digitSettings.maxDigits && currentSuffix < digitSettings.suffixes.Count - 1 && integers.Length - digitSettings.suffixDigits[currentSuffix + 1] > 0)
                {
                    currentSuffix++;
                    integers = integers.Substring(0, integers.Length - digitSettings.suffixDigits[currentSuffix]);
                }

                if (currentSuffix >= 0)
                {
                    integers += digitSettings.suffixes[currentSuffix];
                    shortened = true;
                    return integers;
                }
            }

            //Dots:
            if (digitSettings.dotSeparation && digitSettings.dotDistance > 0)
            {
                char[] chars = integers.ToCharArray();
                integers = "";
                for (int n = chars.Length - 1; n > -1; n--)
                {
                    integers = chars[n] + integers;

                    if ((chars.Length - n) % digitSettings.dotDistance == 0 && n > 0)
                    {
                        integers = digitSettings.dotChar + integers;
                    }
                }
            }

            return integers;
        }
        string ApplyTextSettings(string text, TextSettings settings)
        {
            string newString = text;

            if (text == "")
            {
                return "";
            }

            //Formatting:
            if (settings.bold)
            {
                newString = "<b>" + newString + "</b>";
            }
            if (settings.italic)
            {
                newString = "<i>" + newString + "</i>";
            }
            if (settings.underline)
            {
                newString = "<u>" + newString + "</u>";
            }
            if (settings.strike)
            {
                newString = "<s>" + newString + "</s>";
            }

            //Custom Color:
            if (settings.customColor)
            {
                newString = "<color=#" + ColorUtility.ToHtmlStringRGBA(settings.color) + ">" + newString + "</color>";
            }

            if (settings.mark)
            {
                newString = "<mark=#" + ColorUtility.ToHtmlStringRGBA(settings.markColor) + ">" + newString + "</mark>";
            }

            if (settings.alpha < 1)
            {
                newString = "<alpha=#" + ColorUtility.ToHtmlStringRGBA(new Color(1, 1, 1, settings.alpha)).Substring(6) + ">" + newString + "<alpha=#FF>";
            }

            //Change Size:
            if (settings.size > 0)
            {
                newString = "<size=+" + settings.size.ToString().Replace(',', '.') + ">" + newString + "</size>";
            }
            else if (settings.size < 0)
            {
                newString = "<size=-" + Mathf.Abs(settings.size).ToString().Replace(',', '.') + ">" + newString + "</size>";
            }

            //Character Spacing:
            if (settings.characterSpacing != 0)
            {
                newString = "<cspace=" + settings.characterSpacing.ToString().Replace(',', '.') + ">" + newString + "</cspace>";
            }

            //Spacing:
            if (settings.horizontal > 0)
            {
                string space = "<space=" + settings.horizontal.ToString().Replace(',', '.') + "em>";
                newString = space + newString + space;
            }

            if (settings.vertical != 0)
            {
                newString = "<voffset=" + settings.vertical.ToString().Replace(',', '.') + "em>" + newString + "</voffset>";
            }

            //Return:
            return newString;
        }

        private void ClearMeshs()
        {
            if (meshs != null)
            {
                if (Application.isPlaying)
                {
                    foreach (Mesh mesh in meshs)
                    {
                        Destroy(mesh);
                    }
                }
                else
                {
                    foreach (Mesh mesh in meshs)
                    {
                        DestroyImmediate(mesh);
                    }
                }
            }
        }
        #endregion

        #region Fading
        void HandleFadeIn(float delta)
        {
            if (currentFade < 1)
            {
                currentFade = Mathf.Min(1, currentFade + delta * fadeInSpeed);
                UpdateFade(enableOffsetFadeIn, offsetFadeIn, enableCrossScaleFadeIn, currentScaleInOffset, enableScaleFadeIn, scaleFadeIn, enableShakeFadeIn, shakeOffsetFadeIn, shakeFrequencyFadeIn);
            }
        }
        void HandleFadeOut(float delta)
        {
            if (isFadingOut == false)
            {
                isFadingOut = true;
                OnStop();
            }

            currentFade = Mathf.Max(0, currentFade - delta * fadeOutSpeed);

            UpdateFade(enableOffsetFadeOut, offsetFadeOut, enableCrossScaleFadeOut, currentScaleOutOffset, enableScaleFadeOut, scaleFadeOut, enableShakeFadeOut, shakeOffsetFadeOut, shakeFrequencyFadeOut);

            RemoveFromDictionary();

            if (currentFade <= 0)
            {
                DestroyDNP();
            }
        }
        void UpdateFade(bool enablePositionOffset, Vector2 positionOffset, bool enableScaleOffset, Vector2 scaleOffset, bool enableScale, Vector2 scale, bool enableShake, Vector2 shakeOffset, float shakeFrequency)
        {
            Vector2 basePosition = Vector2.zero;
            float inverseFade = currentFade - 1;

            if (enableShake)
            {
                basePosition = shakeOffset * Mathf.Sin(inverseFade * shakeFrequency) * inverseFade;
            }

            //Position Offset:
            if (enablePositionOffset)
            {
                Vector2 posOffset = positionOffset * inverseFade;
                SetLocalPositionA(basePosition + posOffset);
                SetLocalPositionB(basePosition - posOffset);
            }
            else
            {
                SetLocalPositionA(basePosition);
                SetLocalPositionB(basePosition);
            }

            //Scale & Scale Offset:
            if (enableScaleOffset)
            {
                if (enableScale)
                {
                    Vector3 scaleA = Vector2.Lerp(scaleOffset * scale, Vector2.one, currentFade);
                    scaleA.z = 1;
                    Vector3 scaleB = Vector2.Lerp(new Vector3(1f / scaleOffset.x, 1f / scaleOffset.y, 1) * scale, Vector2.one, currentFade);
                    scaleB.z = 1;

                    transformA.localScale = scaleA;
                    transformB.localScale = scaleB;
                }
                else
                {
                    Vector3 scaleA = Vector2.Lerp(scaleOffset, Vector2.one, currentFade);
                    scaleA.z = 1;
                    Vector3 scaleB = Vector2.Lerp(new Vector3(1f / scaleOffset.x, 1f / scaleOffset.y, 1), Vector2.one, currentFade);
                    scaleB.z = 1;

                    transformA.localScale = scaleA;
                    transformB.localScale = scaleB;
                }
            }
            else if (enableScale)
            {
                Vector3 newScale = Vector2.Lerp(scale, Vector2.one, currentFade);
                newScale.z = 1;
                transformA.localScale = transformB.localScale = newScale;
            }

            //Alpha:
            UpdateAlpha(currentFade);
        }
        public void UpdateAlpha(float progress)
        {
            float alphaFactor = progress * progress * baseAlpha * baseAlpha;

            if(meshs != null)
            {
                for (int n = 0; n < meshs.Count; n++)
                {
                    if (colors[n] != null && meshs[n] != null)
                    {
                        Color[] color = colors[n];
                        float[] alpha = alphas[n];

                        for (int c = 0; c < color.Length; c++)
                        {
                            color[c].a = alphaFactor * alpha[c];
                        }

                        meshs[n].colors = color;
                    }
                }
            }
            
            OnFade(progress);
        }
        #endregion

        #region Movement
        void HandleFollowing(float deltaTime)
        {
            //No Target:
            if (followedTarget == null)
            {
                lastTargetPosition = Vector3.zero;
                return;
            }

            //Get Offset:
            if (lastTargetPosition != Vector3.zero)
            {
                targetOffset += followedTarget.position - lastTargetPosition;
            }
            lastTargetPosition = followedTarget.position;

            //Apply Drag:
            if (followSettings.drag > 0 && currentFollowSpeed > 0)
            {
                currentFollowSpeed -= followSettings.drag * deltaTime;

                if (currentFollowSpeed < 0)
                {
                    currentFollowSpeed = 0;
                }
            }

            //Move to Target:
            Vector3 oldOffset = targetOffset;
            targetOffset = Vector3.Lerp(targetOffset, Vector3.zero, deltaTime * followSettings.speed * currentFollowSpeed);
            position += oldOffset - targetOffset;
        }

        void HandleLerp(float deltaTime)
        {
            float deltaFactor = Mathf.Min(1, deltaTime * lerpSettings.speed);
            Vector3 deltaOffset = remainingOffset * deltaFactor;
            remainingOffset -= deltaOffset;
            position += deltaOffset;
        }

        void HandleVelocity(float deltaTime)
        {
            if (velocitySettings.dragX > 0)
            {
                currentVelocity.x = Mathf.Lerp(currentVelocity.x, 0, deltaTime * velocitySettings.dragX);
            }
            if (velocitySettings.dragY > 0)
            {
                currentVelocity.y = Mathf.Lerp(currentVelocity.y, 0, deltaTime * velocitySettings.dragY);
            }

            currentVelocity.y -= velocitySettings.gravity * deltaTime * GetPositionFactor();
            position += (GetUpVector() * currentVelocity.y + GetRightVector() * currentVelocity.x) * deltaTime;
        }

        Vector3 ApplyShake(Vector3 vector, ShakeSettings shakeSettings, float time)
        {
            float currentTime = time - startTime;
            float currentFactor = Mathf.Sin(shakeSettings.frequency * currentTime) * GetPositionFactor();

            if (shakeSettings.offset.y != 0)
            {
                vector += GetUpVector() * currentFactor * shakeSettings.offset.y;
            }

            if (shakeSettings.offset.x != 0)
            {
                vector += GetRightVector() * currentFactor * shakeSettings.offset.x;
            }

            return vector;
        }

        public Vector3 GetTargetPosition()
        {
            return position + remainingOffset;
        }

        public virtual Vector3 GetPosition()
        {
            return transform.position;
        }

        protected virtual void SetLocalPositionA(Vector3 localPosition)
        {
            transformA.localPosition = localPosition;
        }

        protected virtual void SetLocalPositionB(Vector3 localPosition)
        {
            transformB.localPosition = localPosition;
        }

        public virtual void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        /// <summary>
        /// This function is for the GUI version of damage numbers pro.
        /// It will position the damage number at your mouse position.
        /// Leave 'canvasCamera' as 'null' if your canvas render mode is "Screen Space - Overlay"e
        /// </summary>
        public virtual void SetToMousePosition(RectTransform rectParent, Camera canvasCamera)
        {
            Vector2 mousePosition = Vector3.zero;

#if ENABLE_INPUT_SYSTEM && DNP_NewInputSystem
            if(Mouse.current != null) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, Mouse.current.position.ReadValue(), canvasCamera, out mousePosition);
            }
#else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, Input.mousePosition, canvasCamera, out mousePosition);
#endif


            SetAnchoredPosition(rectParent, mousePosition);
        }

        /// <summary>
        /// Use this function after you spawn a GUI version of damage numbers pro.
        /// </summary>
        public virtual void SetAnchoredPosition(Transform rectParent, Vector2 anchoredPosition)
        {
            //Old Transform:
            Vector3 oldScale = transform.localScale;
            Vector3 oldRotation = transform.eulerAngles;

            //Set Parent and Position:
            transform.SetParent(rectParent, false);
            transform.position = anchoredPosition;

            //New Transform:
            transform.localScale = oldScale;
            transform.eulerAngles = oldRotation;
        }

        /// <summary>
        /// Use this function after you spawn a GUI version of damage numbers pro.
        /// </summary>
        public virtual void SetAnchoredPosition(Transform rectParent, Transform rectPosition, Vector2 relativeAnchoredPosition)
        {
            //Old Transform:
            Vector3 oldScale = transform.localScale;
            Vector3 oldRotation = transform.eulerAngles;

            //Set Parent and Position:
            transform.SetParent(rectParent, false);
            transform.position = rectPosition.position + (Vector3)relativeAnchoredPosition;

            //New Transform:
            transform.localScale = oldScale;
            transform.eulerAngles = oldRotation;
        }

        protected virtual float GetPositionFactor()
        {
            return 1f;
        }
        #endregion

        #region Spam Control
        void AddToDictionary()
        {
            if (spamGroup != "")
            {
                //Create Dictionary:
                if (spamGroupDictionary == null)
                {
                    spamGroupDictionary = new Dictionary<string, HashSet<DamageNumber>>();
                }

                //Create HashSet:
                if (spamGroupDictionary.ContainsKey(spamGroup) == false)
                {
                    spamGroupDictionary.Add(spamGroup, new HashSet<DamageNumber>());
                }

                //Add to HashSet:
                if (spamGroupDictionary[spamGroup].Contains(this) == false)
                {
                    spamGroupDictionary[spamGroup].Add(this);
                }

                removedFromDictionary = false;
            }
            else
            {
                removedFromDictionary = true;
            }
        }
        void RemoveFromDictionary()
        {
            if (!removedFromDictionary && spamGroup != "")
            {
                if (spamGroupDictionary != null && spamGroupDictionary.ContainsKey(spamGroup))
                {
                    if (spamGroupDictionary[spamGroup].Contains(this))
                    {
                        spamGroupDictionary[spamGroup].Remove(this);
                        removedFromDictionary = true;
                    }
                }
            }
        }
        #endregion

        #region Combination
        void HandleCombination(float delta, float time)
        {
            if (myAbsorber != null)
            {
                if(myAbsorber.myAbsorber != null)
                {
                    myAbsorber = myAbsorber.myAbsorber;
                }

                if (time - startTime < combinationSettings.spawnDelay)
                {
                    absorbStartPosition = position;
                    absorbStartTime = time;
                    return;
                }

                //Reset Lifetime:
                startLifeTime = time;

                //Combination Progress:
                float combinationProgress = combinationSettings.absorbDuration > 0 ? (time - absorbStartTime) / combinationSettings.absorbDuration : 1f;

                //Move:
                if (combinationSettings.moveToAbsorber)
                {
                    position = Vector3.Lerp(absorbStartPosition, myAbsorber.position, combinationProgress);
                }

                //Scale:
                combinationScale = combinationSettings.scaleCurve.Evaluate(combinationProgress);

                //Alpha:
                baseAlpha = 1f * combinationSettings.alphaCurve.Evaluate(combinationProgress);
                UpdateAlpha(currentFade);

                if(combinationSettings.instantGain && combinationProgress > 0)
                {
                    GiveNumber(time);
                }

                if (combinationProgress >= 1)
                {
                    GiveNumber(time);
                    DestroyDNP();
                }
            }
        }

        void TryCombination(float time)
        {
            if (enableCombination == false) return; //No Combination

            myAbsorber = null;

            //Combination Methods:
            switch (combinationSettings.method)
            {
                case (CombinationMethod.ABSORB_NEW):
                    float oldestStartTime = time + 0.5f;
                    DamageNumber oldestNumber = null;

                    foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                    {
                        if (otherNumber != this && otherNumber.enableCombination && otherNumber.myAbsorber == null && otherNumber.startTime <= oldestStartTime)
                        {
                            if (Vector3.Distance(otherNumber.GetTargetPosition(), GetTargetPosition()) < combinationSettings.maxDistance * GetPositionFactor())
                            {
                                oldestStartTime = otherNumber.startTime;
                                oldestNumber = otherNumber;
                            }
                        }
                    }

                    if (oldestNumber != null)
                    {
                        GetAbsorbed(oldestNumber, time);
                    }
                    break;
                case (CombinationMethod.REPLACE_OLD):
                    foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                    {
                        if (otherNumber != this && otherNumber.enableCombination)
                        {
                            if (Vector3.Distance(otherNumber.position, position) < combinationSettings.maxDistance * GetPositionFactor())
                            {
                                if (otherNumber.myAbsorber == null)
                                {
                                    otherNumber.startTime = time - 0.01f;
                                }

                                otherNumber.GetAbsorbed(this, time);
                            }
                        }
                    }
                    break;
                case (CombinationMethod.IS_ALWAYS_ABSORBER):
                    foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                    {
                        if (otherNumber != this && otherNumber.enableCombination && otherNumber.combinationSettings.method == CombinationMethod.IS_ALWAYS_VICTIM)
                        {
                            if (Vector3.Distance(otherNumber.position, position) < combinationSettings.maxDistance * GetPositionFactor())
                            {
                                if (otherNumber.myAbsorber == null)
                                {
                                    otherNumber.startTime = time - 0.01f;
                                }

                                otherNumber.GetAbsorbed(this, time);
                            }
                        }
                    }
                    break;
                case (CombinationMethod.IS_ALWAYS_VICTIM):
                    foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                    {
                        if (otherNumber != this && otherNumber.enableCombination && otherNumber.myAbsorber == null && otherNumber.combinationSettings.method == CombinationMethod.IS_ALWAYS_ABSORBER)
                        {
                            if (Vector3.Distance(otherNumber.GetTargetPosition(), GetTargetPosition()) < combinationSettings.maxDistance * GetPositionFactor())
                            {
                                GetAbsorbed(otherNumber, time);
                                break;
                            }
                        }
                    }
                    break;

            }
        }

        void GetAbsorbed(DamageNumber otherNumber, float time)
        {
            if (myAbsorber != null)
            {
                return;
            }

            //Disable Other Features:
            otherNumber.enablePush = otherNumber.enableCollision = false;

            //Set Absorber:
            myAbsorber = otherNumber;

            //Initialize Variables:
            absorbStartPosition = position;
            absorbStartTime = time;
            givenNumber = false;

            //Reset Lifetime:
            myAbsorber.startLifeTime = time;

            //Spawn in Absorber:
            if (combinationSettings.teleportToAbsorber)
            {
                position = otherNumber.position;
            }
        }

        void GiveNumber(float time)
        {
            if (!givenNumber)
            {
                givenNumber = true;

                myAbsorber.number += number;

                if (myAbsorber.myAbsorber == null)
                {
                    myAbsorber.combinationScale = combinationSettings.absorberScaleFactor;
                    myAbsorber.startTime = time;
                    myAbsorber.currentLifetime = myAbsorber.lifetime + combinationSettings.bonusLifetime;
                }

                myAbsorber.UpdateText();
                myAbsorber.OnAbsorb(number, myAbsorber.number);
            }
        }

        /// <summary>
        /// Called when a damage number absorbs of another damage number.
        /// If instant gain is enabled this will be called right when the absorption starts.
        /// </summary>
        protected virtual void OnAbsorb(float number, float newSum)
        {

        }
        #endregion

        #region Destruction
        void HandleDestruction(float time)
        {
            if (enableDestruction && isDestroyed)
            {
                if (time - startTime < destructionSettings.spawnDelay)
                {
                    destructionStartTime = time;
                    return;
                }

                float destructionProgress = destructionSettings.duration > 0 ? (time - destructionStartTime) / destructionSettings.duration : 1f;

                if (destructionProgress >= 1)
                {
                    DestroyDNP();
                }
                else
                {
                    baseAlpha = 1f * destructionSettings.alphaCurve.Evaluate(destructionProgress);
                    UpdateAlpha(currentFade);

                    destructionScale = destructionSettings.scaleCurve.Evaluate(destructionProgress);
                }
            }
        }

        void TryDestruction(float time)
        {
            if (enableDestruction)
            {
                isDestroyed = false;

                foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                {
                    if (otherNumber.isDestroyed == false && otherNumber != this && otherNumber.enableDestruction)
                    {
                        if (Vector3.Distance(otherNumber.GetTargetPosition(), GetTargetPosition()) < destructionSettings.maxDistance * GetPositionFactor())
                        {
                            otherNumber.isDestroyed = true;
                            otherNumber.destructionStartTime = time;
                        }
                    }
                }
            }
        }
        #endregion

        #region Collision
        void TryCollision()
        {
            foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
            {
                otherNumber.collided = false;
            }

            TryCollision(GetTargetPosition());
        }
        void TryCollision(Vector3 sourcePosition)
        {
            if (enableCollision)
            {
                collided = true;

                Vector3 myTargetPosition = GetTargetPosition();
                float radius = collisionSettings.radius * simulatedScale * GetPositionFactor();

                foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                {
                    if (otherNumber.enableCollision && otherNumber != this)
                    {
                        Vector3 otherTargetPosition = otherNumber.GetTargetPosition();
                        Vector3 offset = otherTargetPosition - myTargetPosition;
                        float distance = offset.magnitude;

                        if (distance < radius)
                        {
                            Vector3 offsetOrigin = otherTargetPosition - sourcePosition + offset + collisionSettings.desiredDirection * GetPositionFactor();

                            if (offsetOrigin == Vector3.zero)
                            {
                                offsetOrigin = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
                            }

                            otherNumber.remainingOffset += offsetOrigin.normalized * (radius - distance) * collisionSettings.pushFactor;

                            if(!otherNumber.collided)
                            {
                                otherNumber.TryCollision(sourcePosition);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Push
        void TryPush()
        {
            foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
            {
                otherNumber.pushed = false;
            }

            TryPush(GetTargetPosition());
        }
        void TryPush(Vector3 sourcePosition)
        {
            if (enablePush)
            {
                pushed = true;

                Vector3 myTargetPosition = GetTargetPosition();
                float radius = pushSettings.radius * simulatedScale * GetPositionFactor();

                DamageNumber bestPushTarget = null;
                float pushDirection = pushSettings.pushOffset > 0 ? 1 : -1;
                float heightCap = myTargetPosition.y + 1000 * pushDirection;
                foreach (DamageNumber otherNumber in spamGroupDictionary[spamGroup])
                {
                    if (otherNumber.enablePush && !otherNumber.pushed)
                    {
                        Vector3 targetPosition = otherNumber.GetTargetPosition();
                        if (targetPosition.y * pushDirection < heightCap* pushDirection && Vector3.Distance(myTargetPosition, targetPosition) < radius)
                        {
                            heightCap = targetPosition.y;
                            bestPushTarget = otherNumber;
                        }
                    }
                }

                if (bestPushTarget != null)
                {
                    float heightDifference = (heightCap - myTargetPosition.y);
                    float pushDistance = (pushSettings.pushOffset * GetPositionFactor() - heightDifference);
                    bestPushTarget.remainingOffset.y += pushDirection > 0 ? Mathf.Max(pushDistance, 0) : Mathf.Min(pushDistance, 0);

                    bestPushTarget.TryPush(sourcePosition);
                }
            }
        }
        #endregion

        #region Scale and Rotation
        protected virtual void UpdateRotationZ()
        {
            SetRotationZ(meshRendererA.transform);
            SetRotationZ(meshRendererB.transform);
        }

        protected void SetRotationZ(Transform transformTarget)
        {
            Vector3 localRotation = transformTarget.localEulerAngles;
            localRotation.z = currentRotation;
            transformTarget.localEulerAngles = localRotation;
        }

        void HandleRotateOverTime(float delta, float time)
        {
            currentRotation += currentRotationSpeed * delta * rotateOverTime.Evaluate((time - startTime) / currentLifetime);
        }

        void UpdateScaleAnd3D()
        {
            Vector3 appliedScale = originalScale;
            lastScaleFactor = 1f;

            //Scale Down from Combination:
            if (enableCombination)
            {
                combinationScale = Mathf.Lerp(combinationScale, 1f, Time.deltaTime * combinationSettings.absorberScaleFade);
                lastScaleFactor *= combinationScale;
            }

            //Scale by Number Size:
            if (enableScaleByNumber)
            {
                lastScaleFactor *= numberScale;
            }

            //Scale over Lifetime:
            if (enableScaleOverTime)
            {
                float time = unscaledTime ? Time.unscaledTime : Time.time;
                appliedScale *= scaleOverTime.Evaluate(Mathf.Clamp01((time - startTime) / (currentLifetime + durationFadeOut)));
            }

            //Destruction Scale:
            if (enableDestruction)
            {
                appliedScale *= destructionScale;
            }

            //Perspective:
            #region Perspective
            if (enable3DGame && targetCamera != null)
            {
                //Face Camera:
                if(faceCameraView)
                {
                    if(lookAtCamera)
                    {
                        transform.LookAt(transform.position + (transform.position - targetCamera.position));
                    }
                    else
                    {
                        transform.rotation = targetCamera.rotation;
                    }
                }

                //Initialize Offset:
                Vector3 offset = default;
                float distance = default;

                //Consistent Screen Size:
                if (consistentScreenSize)
                {
                    //Calculate Offset:
                    offset = finalPosition - targetCamera.position;
                    distance = Mathf.Max(0.004f, offset.magnitude);

                    //Calculate Scale:
                    lastScaleFactor *= distance / distanceScalingSettings.baseDistance;

                    if (distance < distanceScalingSettings.closeDistance)
                    {
                        lastScaleFactor *= distanceScalingSettings.closeScale;
                    }
                    else if (distance > distanceScalingSettings.farDistance)
                    {
                        lastScaleFactor *= distanceScalingSettings.farScale;
                    }
                    else
                    {
                        lastScaleFactor *= distanceScalingSettings.farScale + (distanceScalingSettings.closeScale - distanceScalingSettings.farScale) * Mathf.Clamp01(1 - (distance - distanceScalingSettings.closeScale) / Mathf.Max(0.01f, distanceScalingSettings.farDistance - distanceScalingSettings.closeScale));
                    }
                }

                appliedScale *= lastScaleFactor;
                simulatedScale = appliedScale.x;

                //Render Through Walls:
                if (renderThroughWalls)
                {
                    float near = 0.3f;
                    if (Camera.main != null)
                    {
                        near = Camera.main.nearClipPlane;
                    }

                    //Move close to camera:
                    if (!consistentScreenSize)
                    {
                        offset = finalPosition - targetCamera.position;
                        distance = Mathf.Max(0.004f, offset.magnitude);
                    }
                    near += 0.0005f * distance + 0.02f + near * 0.02f * Vector3.Angle(offset, targetCamera.forward);

                    transform.position = offset.normalized * near + targetCamera.position;

                    //Adjust Scale:
                    renderThroughWallsScale = near / distance;
                    appliedScale *= renderThroughWallsScale;
                }
            }
            else
            {
                appliedScale *= lastScaleFactor;
                simulatedScale = appliedScale.x;
            }
            #endregion

            //Apply:
            transform.localScale = appliedScale;

            if (firstFrameScale)
            {
                if(durationFadeIn > 0)
                {
                    transform.localScale = Vector3.zero;
                }
                firstFrameScale = false;
            }
        }
        #endregion

        #region Custom Events

        /// <summary>
        /// This function is called while fading in or out.
        /// </summary>
        /// <param name="currentFade">The current fade and alpha factor meaning 0 = 0% and 1 = 100% faded in.</param>
        protected virtual void OnFade(float currentFade)
        {

        }

        /// <summary>
        /// This function is called whenever the text is updated.
        /// Including when the damage number spawns.
        /// </summary>
        protected virtual void OnTextUpdate()
        {

        }

        /// <summary>
        /// This function is called on every update interval.
        /// Frequency can be configured under performance settings.
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void OnUpdate(float deltaTime)
        {

        }

        /// <summary>
        /// This function is called whenever a damage number is spawned.
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// This function is called whenever a damage number starts fading out.
        /// So this is called a bit earlier than OnDestroy.
        /// </summary>
        protected virtual void OnStop()
        {

        }

        protected virtual void OnLateUpdate()
        {

        }


        /// <summary>
        /// This event was created to fix a GUI specific issue.
        /// I recommend using the OnStart() method for custom code.
        /// </summary>
        protected virtual void OnPreSpawn()
        {
            
        }

        #endregion

        #region Unity Events
        void OnDestroy()
        {
            RemoveFromDictionary();

            //Clear Mesh:
            ClearMeshs();

            //Remove from Pool:
            if (enablePooling && pools != null)
            {
                if (pools.ContainsKey(poolingID))
                {
                    if (pools[poolingID].Contains(this))
                    {
                        pools[poolingID].Remove(this);
                    }
                }
            }

            if (enablePooling && disableOnSceneLoad)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(currentFade > 0)
            {
                DestroyDNP();
            }
        }

        void Reset()
        {
            if (!Application.isPlaying)
            {
                CheckAndEnable3D();
            }
        }
        #endregion
    }
}
