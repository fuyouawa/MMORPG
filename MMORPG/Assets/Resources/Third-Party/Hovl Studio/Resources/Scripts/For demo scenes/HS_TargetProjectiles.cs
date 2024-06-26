using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
//Also you need to shoose Firepoint, targets > 1, Aim image from canvas and 2 target markers and camera.
[RequireComponent(typeof(CharacterController))]
public class HS_TargetProjectiles : MonoBehaviour
{
    public float velocity = 9;
    [Space]

    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;
    private float secondLayerWeight = 0;

    [Space]
    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    private float verticalVel;
    private Vector3 moveVector;

    [Space]
    [Header("Effects")]
    public GameObject[] Prefabs;
    public GameObject[] PrefabsCast;
    private ParticleSystem Effect;
    private int prefabNumber;
    private Transform parentObject;
    public LayerMask collidingLayer = ~0; //Target marker can only collide with scene layer


    [Space]
    [Header("Canvas")]
    public Image aim;
    public Vector2 uiOffset;
    public List<Transform> screenTargets = new List<Transform>();
    private Transform target;
    private bool activeTarger = false;
    public Transform FirePoint;
    public float fireRate = 0.1f;
    private float fireCountdown = 0f;
    private bool rotateState = false;

    [Space]
    [Header("Sound effects")]
    private AudioSource soundComponent; //Play audio from Prefabs
    private AudioClip clip;

    [Space]
    [Header("Camera Shaker script")]
    public HS_CameraShaker cameraShaker;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        
        //Get clip from Audiosource from projectile if exist for playing when shooting
        if (PrefabsCast[0].GetComponent<AudioSource>())
        {
            soundComponent = PrefabsCast[0].GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        target = screenTargets[TargetIndex()];

        if (Input.GetMouseButtonDown(2))
        {
            Counter(-1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Counter(+1);
        }
        

        UserInterface();

        if (Input.GetMouseButton(0) && activeTarger)
        {
            if (rotateState == false)
            {
                StartCoroutine(RotateToTarget(fireRate, target.position));
            }
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 1, Time.deltaTime * 10);
            if (fireCountdown <= 0f)
            {
                GameObject projectile = Instantiate(Prefabs[prefabNumber], FirePoint.position, FirePoint.rotation);
                projectile.GetComponent<HS_TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset);
                Effect = PrefabsCast[prefabNumber].GetComponent<ParticleSystem>();
                Effect.Play();
                //Get Audiosource from Prefabs if exist
                if (PrefabsCast[prefabNumber].GetComponent<AudioSource>())
                {
                    soundComponent = PrefabsCast[prefabNumber].GetComponent<AudioSource>();
                    clip = soundComponent.clip;
                    soundComponent.PlayOneShot(clip);
                }
                StartCoroutine(cameraShaker.Shake(0.1f, 2, 0.2f, 0));
                fireCountdown = 0;
                fireCountdown += fireRate;
            }
        }
        else
        {
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 0, Time.deltaTime * 10);
        }
        fireCountdown -= Time.deltaTime;

        //Need second layer in the Animator
        if (anim.layerCount > 1) { anim.SetLayerWeight(1, secondLayerWeight); }

        InputMagnitude();

        //If you don't need the character grounded then get rid of this part.
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel = 0;
        }
        else
        {
            verticalVel -= 1f * Time.deltaTime;
        }
        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector);
    }

    void Counter(int count)
    {
        prefabNumber += count;
        if (prefabNumber > Prefabs.Length - 1)
        {
            prefabNumber = 0;
        }
        else if (prefabNumber < 0)
        {
            prefabNumber = Prefabs.Length - 1;
        }
    }

    private void UserInterface()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 1.4f, Screen.height / 2, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + (Vector3)uiOffset);
        Vector3 CornerDistance = screenPos - screenCenter;
        Vector3 absCornerDistance = new Vector3(Mathf.Abs(CornerDistance.x), Mathf.Abs(CornerDistance.y), Mathf.Abs(CornerDistance.z));

        //This way you can find target on the full screen
        //if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        // {screenPos.x > 0 && screenPos.y > 0 && screenPos.z > 0} - disable target if enemy backside
        //Find target near center of the screen     
        if (absCornerDistance.x < screenCenter.x / 3 && absCornerDistance.y < screenCenter.y / 3 && screenPos.x > 0 && screenPos.y > 0 && screenPos.z > 0 //If target is in the middle-right of the screen
            && !Physics.Linecast(transform.position + (Vector3)uiOffset, target.position + (Vector3)uiOffset * 2, collidingLayer)) //If player can see the target
        {
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenPos, Time.deltaTime * 3000);
            if (!activeTarger)
                activeTarger = true;
        }
        else
        {
            //Another way
            //aim.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenCenter, Time.deltaTime * 3000);
            if (activeTarger)
                activeTarger = false;
        }
    }

    //Rotate player to target when attack
    public IEnumerator RotateToTarget(float rotatingTime, Vector3 targetPoint)
    {
        rotateState = true;
        float delay = rotatingTime;
        var lookPos = targetPoint - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        while (true)
        {
            if (Speed == 0) { transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 20); }
            delay -= Time.deltaTime;
            if (delay <= 0 || transform.rotation == rotation)
            {
                rotateState = false;
                yield break;
            }
            yield return null;
        }
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //Movement vector
        desiredMoveDirection = forward * InputZ + right * InputX;

        //Character diagonal movement faster fix
        desiredMoveDirection.Normalize();

        if (blockRotationPlayer == false)
        {
            //You can use desiredMoveDirection if using InputMagnitude instead of Horizontal&Vertical axis
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), desiredRotationSpeed);
            //Limit back speed
            if (InputZ < -0.5)
                controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 1.5f));
            //else if (InputX < -0.1 || InputX > 0.1)
            //    controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 1.2f));
            else
                controller.Move(desiredMoveDirection * Time.deltaTime * velocity);
        }
    }

    void InputMagnitude()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
        anim.SetFloat("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player
        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, StopAnimTime, Time.deltaTime);
        }
    }

    public int TargetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 1.4f, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }
        return index;
    }
}
