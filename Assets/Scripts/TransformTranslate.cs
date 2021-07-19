using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTranslate : MonoBehaviour
{
    [SerializeField] Transform targetPos;
    [SerializeField] float speed; 

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos.position, speed * Time.deltaTime);
    }
}
