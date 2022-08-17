using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart_Item : MonoBehaviour
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

    void InstantiateCosmeticPart()
    {
        GameObject part = Instantiate(bodyPartObject, cosmeticTransform);
        foreach (Transform parent in part.transform)
        { parent.GetComponent<ZeroOut>().cancel = true; }
        part.transform.localPosition = Vector3.zero;
        bodyPartClass = part.GetComponent<BodyPartClass>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // when we cone into contact with the player
        if (other.gameObject.transform.tag == "Player")
        {
            // pickup this part using the playerbodypart manager
            PlayerBodyPartManager.instance.PickupPart(bodyPartClass, bodyPartClass.bodyPartType);
            // then destroy
            Destroy(gameObject);
        }
    }
}
