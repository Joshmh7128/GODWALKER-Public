using System.Collections;
using System.Collections.Generic;

public class SaveData
{
    // this class is used to hold all of our save data information


    // default weapons are weapons the player always has discovered, even when their save data is reset
    public List<string> DefaultWeapons = new List<string> // all of the names of default starter weapons
    {
        "Auto Pistol",
        "Blast Pistol",
        "Carbine",
        "DMR",
        "Fleet",
        "Marksman Pistol",
        "Revolver",
        "Shotgun",

    };

    public List<string> DiscoveredWeapons = new List<string> // all of the names of weapons that the player has discovered
    {


    };  

    public List<string> UndiscoveredWeapons = new List<string> // all of the names of weapons the player has not discovered
    {
        "Plasma Rifle",
        "Minigun",
        "Super Shotgun",
        "Sniper",
        "Rocket Launcher",
        "Missile Launcher",
        "Heavy Shotgun",
        "Ballista",
    };

    public List<string> ExcludedWeapons = new List<string> // any weapons we don't want to include in spawning pools, but are used in some way.
    {
        "Popgun",
        "Side Arm",
        "Double Barrel Shotgun"
    };
}
