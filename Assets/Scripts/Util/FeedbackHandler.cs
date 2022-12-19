using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FeedbackHandler : MonoBehaviour
{
    [SerializeField] InputField feedback1; // our feedback
    [SerializeField] GameObject active, confirmation; // our active parent

    // our form response url
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdx1-azuuuLawyFMhfQcGa9-mWcO8y8nAhY_nEZDrbOJ5uQIg/formResponse";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        active.SetActive(!active.activeInHierarchy);
        feedback1.ActivateInputField();

        if (Input.GetKeyDown(KeyCode.Return) && active.activeInHierarchy)
        {
            Send();
        }
    }

    public void Send()
    {
        // send info
        StartCoroutine(Post(feedback1.text));
        // clear ui element
        feedback1.text = "";
        active.SetActive(false);
        // show confirmation
        StartCoroutine(ShowConfirmation());
    }

    IEnumerator Post(string s1)
    {
        // create a new form
        WWWForm form = new WWWForm();
        // add the proper fields to that form
        form.AddField("entry.1640074090", s1);
        form.AddField("entry.1274378120", System.DateTime.Now.ToString());
        form.AddField("entry.355090063", Application.version);
        // perform the post request
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        // return the send request
        yield return www.SendWebRequest(); 
        
    }

    IEnumerator ShowConfirmation()
    {
        confirmation.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        confirmation.SetActive(false);
        yield return null;
    }
}
