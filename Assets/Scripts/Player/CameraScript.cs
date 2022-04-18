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
    float sensitivityChange, currentSensitivity; // changes we want to apply to it for the sake of making the X and Y easier to control // the result after we combine our aimSensitivity and the change
    public float xRotate, yRotate, xRotateMod, yRotateMod; // x, y rotation float
    [SerializeField] float minYAngle, maxYAngle; // min our Y can be is usually negative, max Y is usually positive
    public bool canLook = true; // can we look around?
    // player variables
    [SerializeField] Transform cameraContainer, cameraContainerGoal; // parent container and it's movement goal
    RaycastHit enemyInfoHit; // our aiming raycast hit
    Ray enemyInfoRay; // our aiming ray
    [SerializeField] PlayerController playerController;

    // ui fade canvas
    [SerializeField] CanvasGroup fadeCanvas;

    // ui for enemy info
    [SerializeField] CanvasGroup enemyCanvas; float enemyCanvasTargetAlpha; 
    [SerializeField] Text enemyNameField, enemyHPField;
    [SerializeField] Slider enemyHPSlider;

    // screenshake related
    Vector3 originalPos; [SerializeField] float snapShakeReturnLerpSpeed; // our original position (use when testing other pos than vector3.zero), how quickly we lerp back

    // camera track controller
    [SerializeField] Transform trackContainer;
    [SerializeField] float maximumClose, trackRate;


    // aiming
    public RaycastHit cameraCenterHit;

    // cosmetics to make our character look like they are moving
    public Transform headTransform, bodyTransform;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // set our local pos for screenshake
    void OnEnable()
    {
        originalPos = transform.position;
    }

    void LateUpdate()
    {

        // perform a raycast from the center of the camera to the screen to world point of it's center
        Physics.Raycast(transform.position, transform.forward, out cameraCenterHit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        if (canLook)
        {
            currentSensitivity = aimSensitivity + sensitivityChange;
            // run math to rotate the head of the player as we move the mouse
            yRotate += (player.GetAxis("JoyLookVertical") * 6f + player.GetAxis("MouseVertical")) * -currentSensitivity * Time.fixedDeltaTime;
            // clamp the rotation so we don't go around ourselves
            yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
            // calculate our X rotation
            xRotate += (player.GetAxis("JoyLookHorizontal") * 12f + player.GetAxis("MouseHorizontal")) * currentSensitivity * Time.fixedDeltaTime;
            // add in our rotate mods
            float finalxRotate = xRotate + xRotateMod;
            float finalyRotate = yRotate + yRotateMod;

            Mathf.SmoothStep(xRotate, finalxRotate, 5 * Time.deltaTime);
            Mathf.SmoothStep(yRotate, finalyRotate, 5 * Time.deltaTime);

            // apply it to our torso
            headTransform.eulerAngles = new Vector3(finalyRotate, finalxRotate, 0f);
            bodyTransform.eulerAngles = new Vector3(0f, finalxRotate, 0f);
            // apply it to our camera
            cameraContainer.eulerAngles = new Vector3(finalyRotate, finalxRotate, 0f);
        }

        // make sure there is nothing directly in front of our camera so that we do not clip in walls
        // if there is something in front of our camera, move it forward
        // cast a ray forward from the camera that is the distance of the camera to the player
        Ray trackControlRay = new Ray(); // our dolly control ray
        trackControlRay.direction = transform.forward; 
        // perform the raycast
        if (Physics.Raycast(trackControlRay, Vector3.Distance(transform.position, playerController.transform.position)))
        {
            // move our track container forward
            trackContainer.transform.position += new Vector3(0, 0, 1);
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
            enemyInfoRay.origin = Camera.main.transform.position; // adjust this distance if the camera is adjusted
            enemyInfoRay.direction = Camera.main.transform.forward;

            if (Physics.SphereCast(enemyInfoRay.origin, playerController.pistolShotSize, enemyInfoRay.direction, out enemyInfoHit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore) && (Vector3.Distance(enemyInfoHit.point, transform.position) > 10f))
            {
                // check to see if this hits an enemy
                if (enemyInfoHit.transform.tag == "Enemy")
                {   // if it is an enemy, get its information and set & activate our UI
                    EnemyClass enemyClass = enemyInfoHit.transform.gameObject.GetComponent<EnemyClass>();
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
                // digeticAimTarget.position = transform.position+transform.forward*1000f;
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

        // if our camera has been shaken from a shot, move it back to the original position
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, snapShakeReturnLerpSpeed * Time.deltaTime);

        float returnRate = 0.25f;

        // ensure that our rotate mods are reducing properly
        if (xRotateMod > 0)
        { xRotateMod -= returnRate; }

        if (xRotateMod < 0)
        { xRotateMod += returnRate; }

        if (yRotateMod > 0)
        { yRotateMod -= returnRate; }       
        
        if (yRotateMod < 0)
        { yRotateMod += returnRate; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyInfoHit.point, 0.5f);
    }

    // call this whenever we want our camera to snap shake
    public void SnapScreenShake(float snapShakeDelta)
    {
        transform.localPosition = transform.localPosition + new Vector3(0f, 0f, -snapShakeDelta);
    }
}
