using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalkManager : MonoBehaviour
{
    [SerializeField] ProceduralWalkFoot frontRightFoot;
    [SerializeField] ProceduralWalkFoot frontLeftFoot;
    [SerializeField] ProceduralWalkFoot backRightFoot;
    [SerializeField] ProceduralWalkFoot backLeftFoot;

    // Start is called at the start of the game
    void Start()
    {
        StartCoroutine("LegUpdateCoroutine");
    }

    // custom update function
    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
                frontLeftFoot.MoveFoot();
                backRightFoot.MoveFoot();
                // wait a frame
                yield return null;
            } while (backRightFoot.onSit == false || frontLeftFoot.onSit == false);

            do
            {
                frontRightFoot.MoveFoot();
                backLeftFoot.MoveFoot();
                // wait a frame
                yield return null;
            } while (frontRightFoot.onSit == false || backLeftFoot.onSit == false);
        }
    }
}
