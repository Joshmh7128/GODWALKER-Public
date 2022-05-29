using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLookAt : MonoBehaviour
{
    public Transform targetPos;
    [SerializeField] bool targetPlayer;
    [SerializeField] bool onStart;
    [SerializeField] bool slowMove = false;
    [SerializeField] float slerpSpeed;

    private void Start()
    {
        if (onStart)
        {
            if (targetPlayer && targetPos == null)
            {
                targetPos = GameObject.Find("Player").GetComponent<Transform>();
            }

            // if we are directly looking at the target
            if (!slowMove)
            {
                transform.LookAt(targetPos);
            }
            else if (slowMove) // if we are slowly looking at the target
            {
                // get the direction from us to the target
                Vector3 dir = (targetPos.position - transform.position).normalized;
                // rotate towards the direction at the allotted speed
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                // slerp it
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * slerpSpeed);
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!onStart)
        {
            if (targetPlayer && targetPos == null)
            {
                targetPos = GameObject.Find("Player").GetComponent<Transform>();
            }

            // if we are directly looking at the target
            if (!slowMove)
            {
                transform.LookAt(targetPos);
            }
            else if (slowMove) // if we are slowly looking at the target
            {
                // get the direction from us to the target
                Vector3 dir = (targetPos.position - transform.position).normalized;
                // rotate towards the direction at the allotted speed
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                // slerp it
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * slerpSpeed);
            }
        }
    }
}
