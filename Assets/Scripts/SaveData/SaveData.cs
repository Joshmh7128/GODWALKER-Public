using System.Collections;
using System.Collections.Generic;

public class SaveData
{
    // this class is used to hold all of our save data information


    // default weapons are weapons the player always has discovered, even when their save data is reset
    public List<string> DefaultWeapons = new List<string> // all of the names of default starter weapons
    {
        "Blast Pistol",
        "Carbine",
        "DMR",
        "Marksman Pistol",
        "Electric Revolver",
    };

    public List<string> DiscoveredWeapons = new List<string> // all of the names of weapons that the player has discovered
    {
        "Auto Pistol",
        "Ballista",
        "Blast Rifle",
        "Fleet",
        "Grenade Launcher",
        "Heavy Carbine",
        "Minigun",
        "Missile Launcher",
        "Shotgun",
        "Plasma Pistol",
        "Plasma Rifle",
        "Plasma Sniper",
        "Super Shotgun",
        "Sniper",
        "Seeking Launcher",
        "Rocket Pistol",
        "Sniper",
        "Slagbolt",
        "Super Blast Rifle",
        "Heavy Shotgun"

    };  

    public List<string> Tier2Weapons = new List<string> // all of the names of weapons the player has not discovered
    {

    };

    public List<string> Tier3Weapons = new List<string>
    {

    };

    public List<string> Tier4Weapons = new List<string>
    {
  
    };

    public List<string> ExcludedWeapons = new List<string> // any weapons we don't want to include in spawning pools, but are used in some way.
    {
        "Popgun",
        "Side Arm",
        "Double Barrel Shotgun",
        "Heavy Shotgun",
    };
}
