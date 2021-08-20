using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // How long the object should shake for.
    public float shakeDuration = 0f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.25f; // can be set in editor
    public float decreaseFactor = 2.0f;

    Vector3 originalPos;

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
        // run math to rotate the head of the player as we move the mouse
        yRotate += player.GetAxis("MouseVertical") * -aimSensitivity * Time.fixedDeltaTime;
        // clamp the rotation so we don't go around ourselves
        yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
        // calculate our X rotation
        xRotate += player.GetAxis("MouseHorizontal") * aimSensitivity * Time.fixedDeltaTime;
        // aim the camera
        transform.LookAt(aimTarget.position);
        // apply it
        headTransform.eulerAngles = new Vector3(yRotate, xRotate, 0f);
        bodyTransform.eulerAngles = new Vector3(0f, xRotate, 0f);

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
        // zooming in with the camera
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45, 0.25f);
        }

        if (!Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 90, 0.25f);
        }
    }
}
