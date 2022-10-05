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

    // projectiles NOT SETUP 
    public List<PlayerProjectileScript> playerProjectileScripts = new List<PlayerProjectileScript>();

}
