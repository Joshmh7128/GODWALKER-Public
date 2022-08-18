using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart_Item : ItemClass
{
    // script exists to be an item containing a part that the player can pickup

    public GameObject bodyPartObject; // the bodypart that this item contains
    [HideInInspector] public BodyPartClass bodyPartClass;
    [SerializeField] Transform cosmeticTransform; // for modifying the scale/position of the part

    // on start, instantiate our bodypart at the center of our object
    private void Start()
    {
        InstantiateCosmeticPart();
    }

    // instantiating cosmetics
    void InstantiateCosmeticPart()
    {
        GameObject part = Instantiate(bodyPartObject, cosmeticTransform);
        bodyPartClass = Instantiate(part.GetComponent<BodyPartClass>());
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
        Destroy(gameObject);
    }

    // dynamic pickup
    public void Pickup(bool isRight)
    {
        // pickup this part using the playerbodypart manager
        PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType, isRight);
        // then destroy
        Destroy(gameObject);
    }
}
