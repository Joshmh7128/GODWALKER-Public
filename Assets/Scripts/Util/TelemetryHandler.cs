using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class TelemetryHandler : MonoBehaviour
{
    // our form response url
    //string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdx1-azuuuLawyFMhfQcGa9-mWcO8y8nAhY_nEZDrbOJ5uQIg/formResponse";
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSezl6gq4QwLiDzyx5d_Z3WAR-xG_d9lNSrEawr4x93ONaw2Bg/formResponse";

    // send feedback
    public void Send()
    {
        Debug.Log("sending...");
        // send info
        StartCoroutine("Post");
    }

    string MostUsedWeapons()
    {
        // sort our most used weapons
        Dictionary<string, int> local = PlayerRunStatTracker.instance.weaponUsage;

        // sort
        var sorted = from entry in local orderby entry.Value descending select entry.Key;

        string r = "";

        // build the string
        foreach (string key in sorted)
        {
            // add key
            r += key + ":";
            // add value
            r += local[key] + ";";
        }

        return r;
    }

    // coroutine to post
    IEnumerator Post()
    {
        // create a new form
        WWWForm form = new WWWForm();
        // add the proper fields to that form
        
        form.AddField("entry.934937961", (System.DateTime.Now - PlayerRunStatTracker.instance.startTime).ToString());

        if (PlayerPrefs.GetString("CollectData", "true") == "true")
        {
            form.AddField("entry.1818930104", MostUsedWeapons());
            form.AddField("entry.1132086311", PlayerRunStatTracker.instance.runsCompleted.ToString());
            form.AddField("entry.1221599062", Application.version.ToString());
            form.AddField("entry.1582724862", PlayerPrefs.GetString("UID"));
        }

        // perform the post request
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        // return the send request
        yield return www.SendWebRequest();
    }
}
