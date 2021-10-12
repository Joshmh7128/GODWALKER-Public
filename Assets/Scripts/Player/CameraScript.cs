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
    [SerializeField] Transform headTransform; // the transform of our player's head
    [SerializeField] Transform bodyTransform; // the transform of our player's body
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

    // visuals
    [SerializeField] LineRenderer rightArmLine;
    [SerializeField] Transform rightArm;
    [SerializeField] LineRenderer leftArmLine;
    [SerializeField] Transform leftArm;

    // ui fade canvas
    [SerializeField] CanvasGroup fadeCanvas;

    // How long the object should shake for.
    public float shakeDuration = 0f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.25f; // can be set in editor
    public float decreaseFactor = 2.0f;

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

        // clamp our shake
        Mathf.Clamp(shakeAmount, 0, 0.08f);

        if (canLook)
        {
            // run math to rotate the head of the player as we move the mouse
            yRotate += player.GetAxis("MouseVertical") * -aimSensitivity * Time.fixedDeltaTime;
            yRotate += player.GetAxis("JoyLookVertical") * -aimSensitivity * 5 * Time.fixedDeltaTime;
            // clamp the rotation so we don't go around ourselves
            yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
            // calculate our X rotation
            xRotate += player.GetAxis("MouseHorizontal") * aimSensitivity * Time.fixedDeltaTime;
            xRotate += player.GetAxis("JoyLookHorizontal") * aimSensitivity * 10 * Time.fixedDeltaTime;
            // aim the camera
            transform.LookAt(aimTarget.position);
            // apply it
            headTransform.eulerAngles = new Vector3(yRotate, xRotate, 0f);
            bodyTransform.eulerAngles = new Vector3(0f, xRotate, 0f);
        }
            // access our line renderers
            rightArmLine.SetPosition(0, rightArm.position);
            rightArmLine.SetPosition(1, digeticAimTarget.position);

            leftArmLine.SetPosition(0, leftArm.position);
            leftArmLine.SetPosition(1, digeticAimTarget.position);
            // move our digetic aim target
            digeticAimTarget.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 100f));
        
        // screenshake
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            if (canLook)
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }

        // toggle which side our camera is on
        if (Input.GetKeyDown(KeyCode.Z))
        {
            moveTargetIsRight = !moveTargetIsRight;
        }

        // move the container
        if (moveTargetIsRight)
        {
            cameraContainer.position = moveTargetRight.position;
        } else
        {
            cameraContainer.position = moveTargetLeft.position;
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

        if (canLook)
        {
            // zooming in with the camera
            if (player.GetButton("Aim"))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45, 0.25f);
            }

            if (!player.GetButton("Aim"))
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90, 0.25f);
            }
        }
    }
}
