using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeBodyPartClass : BodyPartClass
{
    /// this class combines two bodyparts
    [SerializeField] BodyPartClass firstBodyPart, secondBodyPart;

    public override void OnJump()
    {
        firstBodyPart.OnJump();
        secondBodyPart.OnJump();
    }

    

}
