using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FearManager : MonoBehaviour
{
    // this script is used to manage the fear levels and changes how the player works when fear is applied
    // it houses all the stats and displays them

    // build and assign instance
    public static FearManager instance;
    private void Awake() => instance = this;

    // useful stats
    public float basePlayerSpeed; // the base player movement speed
    public float basePlayerMaxHealth; // the base amount of maximum health the player has

    private void Start()
    {
        // set player movement speed
        basePlayerSpeed = PlayerController.instance.moveSpeed;
        basePlayerMaxHealth = PlayerStatManager.instance.maxHealth;

        foreach (Effect effect in effects)
        {
            effect.ResetEffect();
            effect.StartEffect();
        }
    }

    // we'll have a list of our effects. each detriment has different stages. we have effects for everything, 
    // even if they are currently inactive. each possible detriment in the game has its own effect class
    public List<Effect> effects = new List<Effect>();

    // runs each effects application function
    public void ApplyEffects()
    {
        foreach (Effect effect in effects)
        {
            effect.ApplyEffect();
        }    
    }
}

// our base effect class
public abstract class Effect : MonoBehaviour
{
    // our public information
    // what stage is this effect in?
    public int effectStage;
    [HideInInspector] public int maxStage; // what is the highest stage this effect can be in

    public string effectType; // our effect type as a string
    public string effectInfo; // public info about what our effect does
    public List<string> effectInfos; // the list of the different infos we can swap between

    private void Start()
    {
        maxStage = effectInfos.Count - 1; // set total stages
    }

    public void ResetEffect()
    {
        effectStage = 0;
        effectInfo = effectInfos[0];
    }

    public abstract void StartEffect(); // a manually run start function

    // run this to apply the effect of this card to the player
    public abstract void ApplyEffect();

    // increase and decrease effect
    public virtual void IncreaseEffect()
    {
        if (effectStage < maxStage)
        {
            effectStage++;
        }
    }
    public virtual void DecreaseEffect()
    {
        if (effectStage > 0)
        {
            effectStage--;
        }
    }
}
