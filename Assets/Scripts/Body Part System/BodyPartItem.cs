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
    public bool overrideGeneration;
    public BodyPartClass.BodyPartTypes overrideBodyPartType; // use this and the bool to override the generation of a part

    // on start, instantiate our bodypart at the center of our object
    private void Start()
    {
        InstantiateCosmeticPart();
    }

    // instantiating cosmetics
    void InstantiateCosmeticPart()
    {
        // build the part out of our body part object
        GameObject cosmeticPart = Instantiate(bodyPartObject, cosmeticTransform);
        // if we want this object to be a specific body part, set and use override
        if (overrideGeneration)
        {
            cosmeticPart.GetComponent<BodyPartClass>().overrideGen = true;
            cosmeticPart.GetComponent<BodyPartClass>().bodyPartType = overrideBodyPartType;
        }
        bodyPartClass = Instantiate(cosmeticPart.GetComponent<BodyPartClass>(), new Vector3(9999,9999,9999), Quaternion.identity, null); // instantiating them in slipspace
        bodyPartClass.cancelConstruct = true; // cancel the construct because we want an exact copy of the item



        StartCoroutine(Buffer(cosmeticPart));
        // make sure they dont zero out on start
        for (int i = 0; i < cosmeticPart.GetComponent<BodyPartClass>().cosmeticParts.Count; i++)
        {
            if (cosmeticPart.GetComponent<BodyPartClass>().cosmeticParent)
            {
                foreach (Transform child in cosmeticPart.GetComponent<BodyPartClass>().cosmeticParent)
                {
                    child.GetComponent<ZeroOut>().cancel = true;
                }
            }
            else
            {
                // cancel the zero out
                cosmeticPart.GetComponent<BodyPartClass>().cosmeticParts[i].gameObject.GetComponent<ZeroOut>().cancel = true;
            }

        }
        cosmeticPart.transform.localPosition = Vector3.zero;
    }

    // buffer for spawning in the object we are picking up
    IEnumerator Buffer(GameObject cosmeticPart)
    {
        yield return new WaitForFixedUpdate();

        bodyPartClass.cancelConstruct = true; // cancel the construct because we want an exact copy of the item
        bodyPartClass.RefreshPart(cosmeticPart.GetComponent<BodyPartClass>().bodyPartType); // refresh the part now that it is in 
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
                        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType); // pickup the body part
                        OnDestroyGameObject();
                    }       
                    
                    // change our pickups based on what we are
                    if (bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Arm || bodyPartClass.bodyPartType == BodyPartClass.BodyPartTypes.Leg)
                    {
                        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType, true); // pickup the body part
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
        // call an invoke
        PlayerBodyPartManager.instance.CallParts("OnBodyPartPickup", 0.1f);
        Instantiate(pickupFX, transform.position, pickupFX.transform.rotation, null);
        Instantiate(playerPickupFX, PlayerController.instance.transform.position + new Vector3(0, -1, 0), playerPickupFX.transform.rotation, PlayerController.instance.transform);
        PlayerCameraController.instance.FOVKickRequest(5f);
        Destroy(ourCanvas.gameObject);
        Destroy(gameObject); // remove the weapon from the world
    }

}
