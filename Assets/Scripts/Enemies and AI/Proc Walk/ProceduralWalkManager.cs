using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalkManager : MonoBehaviour
{
    [SerializeField] ProceduralWalkFoot frontRightFoot;
    [SerializeField] ProceduralWalkFoot frontLeftFoot;
    [SerializeField] ProceduralWalkFoot backRightFoot;
    [SerializeField] ProceduralWalkFoot backLeftFoot;
    [SerializeField] float slowSpeed;
    [SerializeField] float highSpeed;
    [SerializeField] TransformTranslate transformTranslate;

    // Start is called at the start of the game
    void Start()
    {
        StartCoroutine("LegUpdateCoroutine");
        StartCoroutine("SpeedSwapping");
    }

    // speed swapping
    IEnumerator SpeedSwapping()
    {
        yield return new WaitForSeconds(Random.Range(1, 5));
        transformTranslate.speed = slowSpeed;
        yield return new WaitForSeconds(Random.Range(1, 5));
        transformTranslate.speed = highSpeed;
        StartCoroutine("SpeedSwapping");
    }

}
