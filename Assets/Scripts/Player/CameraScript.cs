using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class CameraScript : MonoBehaviour
{
    // rewired input
    Player player;

    // controling variables
    [SerializeField] float aimSensitivity; // how sensitive is our camera
    float sensitivityChange;
    float currentSensitivity;
    public Transform headTransform; // the transform of our player's head
    public Transform bodyTransform; // the transform of our player's body
    [SerializeField] float yRotate; // Y rotation float
    [SerializeField] float xRotate; // X rotation float
    [SerializeField] float minYAngle; // min our Y can be (usually negative)
    [SerializeField] float maxYAngle; // max our Y can be (usually positive)
    [SerializeField] public bool canLook = true; // can we look around?

    // player variables
    [SerializeField] Transform aimTarget; // where is the camera looking?
    [SerializeField] Transform digeticAimTarget; // where is the camera looking?
    [SerializeField] Transform moveTargetRight; // where is the camera moving to?    
    [SerializeField] Transform moveTargetLeft; // where is the camera moving to?
    [SerializeField] Transform cameraContainer; // container
    [SerializeField] bool moveTargetIsRight = true; // true = right, false = left, toggle with Z
    Transform camTransform;
    RaycastHit hit; // our aiming raycast hit
    Ray ray; // our aiming ray

    // ui fade canvas
    [SerializeField] CanvasGroup fadeCanvas;

    // ui for enemy info
    [SerializeField] CanvasGroup enemyCanvas; float enemyCanvasTargetAlpha; 
    [SerializeField] Text enemyNameField, enemyHPField;
    [SerializeField] Slider enemyHPSlider;

    // How long the object should shake for.
    public float shakeDuration;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount, shakeDelta; // can be set in editor
    public float decreaseFactor; 

    Vector3 originalPos;

    // rifle aiming
    public RaycastHit rifleTargetHit;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // get our camera transform
    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // set our local pos for screenshake
    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void LateUpdate()
    {

        // perform a raycast from the center of the camera to the screen to world point of it's center
        Physics.Raycast(transform.position, transform.forward, out rifleTargetHit, Mathf.Infinity);

        if (canLook)
        {
            currentSensitivity = aimSensitivity + sensitivityChange;

            // run math to rotate the head of the player as we move the mouse
            yRotate += (player.GetAxis("JoyLookVertical") * 6f + player.GetAxis("MouseVertical")) * -currentSensitivity * Time.fixedDeltaTime;
            // clamp the rotation so we don't go around ourselves
            yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
            // calculate our X rotation
            xRotate += (player.GetAxis("JoyLookHorizontal") * 12f + player.GetAxis("MouseHorizontal")) * currentSensitivity * Time.fixedDeltaTime;
            // aim the camera at the child object of the head. head is moved by the above code
            transform.LookAt(aimTarget.position);
            // apply it
            headTransform.eulerAngles = new Vector3(yRotate, xRotate, 0f);
            bodyTransform.eulerAngles = new Vector3(0f, xRotate, 0f);
            // access our line renderers

            // leftArmLine.SetPosition(0, leftArm.position);
            // leftArmLine.SetPosition(1, digeticAimTarget.position); 
        }
        
        // toggle which side our camera is on
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (canLook)
            moveTargetIsRight = !moveTargetIsRight;
        }

        // move the container
        if (canLook)
        {
            if (moveTargetIsRight)
            {
                cameraContainer.position = moveTargetRight.position;
            }
            else
            {
                cameraContainer.position = moveTargetLeft.position;
            }
        }
    }

    private void FixedUpdate()
    {
        // check if we are in the hub
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            // turn down that alpha
            if (fadeCanvas.alpha > 0)
            fadeCanvas.alpha -= 0.1f;
        }

        // make sure our enemy canvas is displaying correctly
        if (enemyCanvas.alpha < enemyCanvasTargetAlpha)
        { enemyCanvas.alpha += 0.1f; } else if (enemyCanvas.alpha > enemyCanvasTargetAlpha) { enemyCanvas.alpha -= 0.025f; }

        if (canLook)
        {
            // screenshake
            if (shakeDuration > 0 && canLook)
            {
                camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * shakeDelta);
                shakeDuration -= decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                camTransform.localPosition = originalPos;
            }

            ray.origin = Camera.main.transform.position; // adjust this distance if the camera is adjusted
            ray.direction = Camera.main.transform.forward;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore) && (Vector3.Distance(hit.point, transform.position) > 10f))
            {
                // do a raycast and then position the target
                digeticAimTarget.position = hit.point;

                // check to see if this hits an enemy
                if (hit.transform.tag == "Enemy")
                {   // if it is an enemy, get its information and set & activate our UI
                    EnemyClass enemyClass = hit.transform.gameObject.GetComponent<EnemyClass>();
                    // set our UI fields
                    enemyNameField.text = enemyClass.NameText; enemyHPField.text = enemyClass.HP + " / " + enemyClass.maxHP;
                    // set our UI slider
                    enemyHPSlider.value = enemyClass.HP; enemyHPSlider.maxValue = enemyClass.maxHP;
                    // make sure we set our canvas to show 
                    enemyCanvasTargetAlpha = 1;
                }
                else
                {
                    // make sure we set our canvas to hide
                    enemyCanvasTargetAlpha = 0;
                }
            }
            else
            {
                // do a raycast and then position the target
                digeticAimTarget.position = transform.position+transform.forward*1000f;
                // make sure we set our canvas to hide
                enemyCanvasTargetAlpha = 0;
            }


            // zooming in with the camera
            if (player.GetButton("Aim"))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45, 0.25f);
                sensitivityChange = -aimSensitivity / 2;

            }

            if (!player.GetButton("Aim"))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90, 0.25f);
                sensitivityChange = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(hit.point, 0.5f);
    }
}
