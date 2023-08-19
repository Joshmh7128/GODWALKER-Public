using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionAnimationHandler : MonoBehaviour
{
    // pulses corruption veins
    [SerializeField] float pulseRate; // how often does this pulse
    [SerializeField] float pulseSize, pulseReturn; // how big and close does this return?
    [SerializeField] AudioSource heartBeatSource; // the source of our heartbeat

    private void Start()
    {
        StartCoroutine(PulseRate());
    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, pulseReturn * Time.fixedDeltaTime);
    }

    IEnumerator PulseRate()
    {
        yield return new WaitForSeconds(pulseRate);

        StartCoroutine(PulseRate());

        transform.localScale = new Vector3(pulseSize, pulseSize, pulseSize);

        if (heartBeatSource) heartBeatSource.Play();
    }
}
