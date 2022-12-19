using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FeedbackHandler : MonoBehaviour
{
    [SerializeField] InputField feedback1; // our feedback
    [SerializeField] GameObject active; // our active parent

    // our form response url
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdx1-azuuuLawyFMhfQcGa9-mWcO8y8nAhY_nEZDrbOJ5uQIg/formResponse";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        active.SetActive(!active.activeInHierarchy);
    }

    public void Send()
    {
        StartCoroutine(Post(feedback1.text));
    }

    IEnumerator Post(string s1)
    {
        // create a new form
        WWWForm form = new WWWForm();
        // add the proper fields to that form
        form.AddField("entry.1640074090", s1);
        // perform the post request
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        // return the send request
        yield return www.SendWebRequest(); 
        
    }
}
