using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveCombatArenaTest : MonoBehaviour
{
    /// this script manages all of the progressive elements of our progressive dynamic combat arenas 
    /// 

    private void FixedUpdate()
    {
        // process our darkness leaving when we take the godheart
        if (darknessParent != null)
        ProcessDarkness();
    }

    #region
    // for taking the godheart
    [SerializeField] Transform darknessParent; // the parent that holds our dark panels
    [SerializeField] Transform heartGiver; // the parent that holds our heartGiver
    [SerializeField] float darknessActivationDistance; // the distance when the darkness moves when the player approaches
    // moves the darkness out of the way once we collect the godheart
    void ProcessDarkness()
    {
        if (darknessParent != null && Vector3.Distance(PlayerController.instance.transform.position, darknessParent.position) < darknessActivationDistance)
            darknessParent.localScale += Vector3.one * 4 * Time.fixedDeltaTime;

        if (darknessParent.localScale.x > 100)
            Destroy(darknessParent.gameObject);
    }
    #endregion
}
