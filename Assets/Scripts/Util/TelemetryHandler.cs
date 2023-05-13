using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TelemetryHandler : MonoBehaviour
{
    // our form response url
    // string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdx1-azuuuLawyFMhfQcGa9-mWcO8y8nAhY_nEZDrbOJ5uQIg/formResponse";
    string URL = "https://docs.google.com/forms/d/e/1FAIpQLSezl6gq4QwLiDzyx5d_Z3WAR-xG_d9lNSrEawr4x93ONaw2Bg/viewform";

    public static TelemetryHandler instance;

    private void Awake() => instance = this;

    // send feedback
    public void Send()
    {
        Debug.Log("sending...");
        // send info
        StartCoroutine(Post());
    }

    // coroutine to post
    IEnumerator Post()
    {
        // create a new form
        WWWForm form = new WWWForm();
        // add the proper fields to that form
        form.AddField("entry.934937961", PlayerRunStatTracker.instance.startTime.ToString());
        form.AddField("entry.1818930104", PlayerRunStatTracker.instance.weaponUsage.ToString());
        form.AddField("entry.1132086311", PlayerRunStatTracker.instance.runsCompleted.ToString());
        form.AddField("entry.1221599062", Application.version.ToString());
        // perform the post request
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        // return the send request
        yield return www.SendWebRequest();

    }

}
