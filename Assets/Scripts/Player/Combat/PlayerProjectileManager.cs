using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileManager : MonoBehaviour
{
    /// class holds all active projectiles and active explosions of the player
    
    // setup instance
    public static PlayerProjectileManager instance;
    private void Awake() => instance = this;

    public float activeProjectileMax = 100f;

    // explosions
    public List<PlayerExplosionScript> explosionScripts = new List<PlayerExplosionScript>();

    // shock explosion
    public List<PlayerShockExplosionScript> shockExplosionScripts = new List<PlayerShockExplosionScript>();
    public List<PlayerShockExplosionScript> loopingShockExplosionScripts = new List<PlayerShockExplosionScript>();
    public void RefreshShockList() // clean the list
    {
        for (int i = 0; i < shockExplosionScripts.Count; i++)
        {
            if (shockExplosionScripts[i] == null) shockExplosionScripts.Remove(shockExplosionScripts[i]);
        }
        for (int i = 0; i < loopingShockExplosionScripts.Count; i++)
        {
            if (loopingShockExplosionScripts[i] == null) loopingShockExplosionScripts.Remove(loopingShockExplosionScripts[i]);
        }
    }


    // projectiles
    public List<PlayerProjectileScript> activeProjectileScripts = new List<PlayerProjectileScript>();



}
