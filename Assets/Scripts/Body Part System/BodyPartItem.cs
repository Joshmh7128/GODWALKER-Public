using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartItem : ItemClass
{
    // script exists to be an item containing a part that the player can pickup

    public GameObject bodyPartObject; // the bodypart that this item contains
    [HideInInspector] public BodyPartClass bodyPartClass;
    [SerializeField] Transform cosmeticTransform; // for modifying the scale/position of the part
    [SerializeField] Transform ourCanvas; // so we can make sure it is destroyed
    [SerializeField] GameObject pickupFX, playerPickupFX; // the effect when we get picked up

    // on start, instantiate our bodypart at the center of our object
    private void Start()
    {
        InstantiateCosmeticPart();
    }

    // instantiating cosmetics
    void InstantiateCosmeticPart()
    {
        GameObject part = Instantiate(bodyPartObject, cosmeticTransform);
        bodyPartClass = Instantiate(part.GetComponent<BodyPartClass>(), new Vector3(9999,9999,9999), Quaternion.identity, null); // instantiating them in slipspace
        // make sure they dont zero out on start
        foreach (Transform parent in part.transform)
        { parent.GetComponent<ZeroOut>().cancel = true; }
        part.transform.localPosition = Vector3.zero;
    }

    // simple pickup
    public void Pickup()
    {
        // pickup this part using the playerbodypart manager
        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType);
        // then destroy
        OnDestroyGameObject();
    }

    // dynamic pickup
    public void Pickup(bool isRight)
    {
        // pickup this part using the playerbodypart manager
        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType, isRight);
        // then destroy
        OnDestroyGameObject();
    }

    private void Update()
    {
        ProcessCanGrab();

        // rotate our cosmetic parent
        cosmeticTransform.localEulerAngles = new Vector3(0, cosmeticTransform.localEulerAngles.y + Time.deltaTime * 5f, 0);
    }

    void ProcessCanGrab()
    {

        // grab check
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < pickupDistance)
        {
            canGrab = true;
        }

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > pickupDistance)
        {
            canGrab = false;
        }

        // actual grabbing
        if (canGrab)
        {
            // replace with Use button later
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PlayerBodyPartManager.instance.pickupCooldown <= 0 && PlayerBodyPartManager.instance.highlightedBodyPart == gameObject)
                {
                    // change our pickups based on what we are
                    if (bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Head || bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Torso)
                    {
                        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType); // pickup the weapon
                        OnDestroyGameObject();
                    }       
                    
                    // change our pickups based on what we are
                    if (bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Arm || bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Leg)
                    {
                        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType, true); // pickup the weapon
                        OnDestroyGameObject();
                    }
                }
            }

            // other button
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (PlayerBodyPartManager.instance.pickupCooldown <= 0 && PlayerBodyPartManager.instance.highlightedBodyPart == gameObject)
                {
                    // change our pickups based on what we are
                    if (bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Arm || bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Leg)
                    {
                        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType, false); // pickup the weapon
                        OnDestroyGameObject();
                    }
                }
            }
        }
    }

    void OnDestroyGameObject()
    {
        Instantiate(pickupFX, transform.position, pickupFX.transform.rotation, null);
        Instantiate(playerPickupFX, PlayerController.instance.transform.position + new Vector3(0, -1, 0), playerPickupFX.transform.rotation, PlayerController.instance.transform);
        PlayerCameraController.instance.FOVKickRequest(5f);
        Destroy(ourCanvas.gameObject);
        Destroy(gameObject); // remove the weapon from the world
    }
}
