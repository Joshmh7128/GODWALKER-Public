using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTest : MonoBehaviour
{

    public Sprite ourSprite; // our spriste
    public CombatAbility_Item item; // our item

    // Start is called before the first frame update
    void Start()
    {
        ourSprite = item.abilityIcon;
    }
}
