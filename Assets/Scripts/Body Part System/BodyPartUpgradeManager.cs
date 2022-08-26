using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartUpgradeManager : MonoBehaviour
{
    /// this script manages all of our body parts
    /// 

    // setup our instance
    public static BodyPartUpgradeManager instance;
    private void Awake()
    {
        instance = this;
    }

    public bool EyeOfAthena; // are our shots 10% more accurate while ADS?
    public bool SpeedOfAres; // is our firerate 10% faster while sprinting?
    public bool TonalHaste; // do we get a 20% sprint boost for 5 seconds after reloading?
    public bool SpiritOfTheAir; // do guns reload 15% faster during a jump
    public bool EarthBreaker; // do we launch projectiles in all four cardinal directions from the player when we land?

}
