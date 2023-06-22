using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handfire : MonoBehaviour
{
    public bool firing = false;
    private float lastFire = 0.0f;
    private float firePeriod = 0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (XRInfo.IsAvailable(OVRHand.Hand.HandRight))
        {
            if (XRInfo.GetOVRHand(OVRHand.Hand.HandRight).GetFingerIsPinching(OVRHand.HandFinger.Middle))
            {
                firing = !firing;
            }

            if (!firing)
            {
                return;
            }
            
            float handSpread = (XRInfo.GetBoneDistance(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_PinkyTip, OVRSkeleton.BoneId.Hand_ThumbTip) +
                               XRInfo.GetBoneDistance(OVRHand.Hand.HandRight,OVRSkeleton.BoneId.Hand_IndexTip, OVRSkeleton.BoneId.Hand_ThumbTip))/2.0f;
            if (handSpread > .60f)
            {
                firePeriod = Mathf.Clamp(1.0f/(2.0f*handSpread), 0.3f, 1.0f);
                Fire();
            }
            
            
        }
        
    }
    
    void Fire()
    {
        if (Time.time - lastFire > firePeriod)
        {
            Vector3 pos = XRInfo.GetBoneTransform(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_IndexTip).position;
            Vector3 dir = XRInfo.GetBoneTransform(OVRHand.Hand.HandRight, OVRSkeleton.BoneId.Hand_IndexTip).right;

            string dataStr = string.Join<float>(",", new[] { pos.x, pos.y, pos.z });
            dataStr += "|";
            dataStr += string.Join<float>(",", new[]  { dir.x, dir.y, dir.z});
            
            CroquetBridge.SendCroquetSync("event", "shoot", dataStr);
            lastFire = Time.time;
        }
    }
}
    
