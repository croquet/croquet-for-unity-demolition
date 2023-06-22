using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRInfoTest : MonoBehaviour
{

    void Start()
    {

        //XRInfo.GetOVRHand(OVRHand.Hand.HandRight);
    }

    private void OnRenderObject()
    {
        // if (XRInfo.IsAvailable(OVRHand.Hand.HandRight))
        // {
        //     Transform indexTip = XRInfo.GetBoneTransform(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_IndexTip);
        //     Transform thumbTip = XRInfo.GetBoneTransform(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_ThumbTip);
        //     XRDeadBug.DrawLine(
        //         indexTip.position,
        //         thumbTip.position,
        //         new Color(XRInfo.GetBoneDistance(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_IndexTip, OVRSkeleton.BoneId.Hand_ThumbTip) * 15.0f, .5f, .5f),
        //         true);
        // }
        

    }
}