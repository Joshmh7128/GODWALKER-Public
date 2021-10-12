using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSetShellScript : MonoBehaviour
{
    [SerializeField] int countDownTime;
    [SerializeField] Text displayText1;
    [SerializeField] Text displayText2;
    [SerializeField] GameObject shell;

    // OnEnable is called when the shell is enabled
    void OnEnable()
    {
        // initiate the cycle
        StartCoroutine(CountdownTimer(countDownTime));
    }

    IEnumerator CountdownTimer(int countDownTimeLocal)
    {
        if (countDownTimeLocal > 0)
        {
            // display our remaining time
            displayText1.text = "Closes in: " + countDownTimeLocal + " seconds";
            displayText2.text = "Closes in: " + countDownTimeLocal + " seconds";
            // make sure our shell is deactivated
            shell.SetActive(false);
            yield return new WaitForSeconds(1);
            countDownTimeLocal--;
        }

        if (countDownTimeLocal < 0)
        {
            // display our remaining time
            displayText1.text = "Closed: " + Mathf.Abs(countDownTimeLocal) + " seconds ago";
            displayText2.text = "Closed: " + Mathf.Abs(countDownTimeLocal) + " seconds ago";
            // make sure our shell is activated
            shell.SetActive(true);
            yield return new WaitForSeconds(1);
            countDownTimeLocal--;

        }

        // restart
        StartCoroutine(CountdownTimer(Mathf.Clamp(countDownTimeLocal, -999, 999)));
    }
}
