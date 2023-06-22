using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// XRInfo
/// 
/// The goal of this is to allow for one liner access to almost any information that OVR can provide by efficiently caching references. 
/// I am attempting to keep this a pure class without the need for attaching to a GameObject or any kind of other instantiation or management.
/// Although this is specific to OVR for now, in the future this one line access goal should be made available for any interface.
/// 
/// </summary>

public class XRInfo : MonoBehaviour
{
    //private GameObject leftHandGO;
    //private GameObject rightHandGO;

    //private OVRHand leftHand;
    //private OVRHand rightHand;

    //private OVRSkeleton leftHandSkeleton;
    //private OVRSkeleton rightHandSkeleton;


    public struct HandInfo
    {
        public GameObject handGO;
        public OVRHand hand;
        public OVRSkeleton handSkeleton;
    }

    private static Dictionary<OVRHand.Hand, HandInfo> handsInfo;

    //private struct handfinger


    public bool isTrackedAvailableValid;
    private static bool handsCached = false;

    private static bool headInfoCached = false;
    private static Transform centerEyeTransform;

    private static void cacheHeadInfo()
    {
        try
        {
            OVRCameraRig ovrCameraRig = FindObjectOfType<OVRCameraRig>();
            centerEyeTransform = ovrCameraRig.centerEyeAnchor.transform;
        }
        catch
        {
            headInfoCached = false;
        }
    }

    private static void cacheHandInfo()
    {
        try
        {
            handsInfo = new Dictionary<OVRHand.Hand, HandInfo>();

            // search the scene for hands once and cache references
            var hands = FindObjectsOfType<OVRHand>(); // TODO: This is the only call that requires a monobehaviour inheritance so should work to remove

            HandInfo rightHandInfo = new HandInfo();
            rightHandInfo.hand = hands.Where(hand => hand.GetComponent<OVRSkeleton>().GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight).First();
            rightHandInfo.handGO = rightHandInfo.hand.gameObject;
            rightHandInfo.handSkeleton = rightHandInfo.handGO.GetComponent<OVRSkeleton>();
            handsInfo.Add(OVRHand.Hand.HandRight, rightHandInfo);

            HandInfo leftHandInfo = new HandInfo();
            leftHandInfo.hand = hands.Where(hand => hand.GetComponent<OVRSkeleton>().GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft).First();
            leftHandInfo.handGO = leftHandInfo.hand.gameObject;
            leftHandInfo.handSkeleton = leftHandInfo.handGO.GetComponent<OVRSkeleton>();
            handsInfo.Add(OVRHand.Hand.HandLeft, leftHandInfo);

            handsCached = true;
        }
        catch
        {
            handsCached = false;
        }

    }

    public static bool IsAvailable(OVRHand.Hand hand)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        // "Availability" is strictly defined as being:
        // - isActiveAndEnabled
        // - isTracked
        // - isDataValid
        // - isDataHighConfidence
        HandInfo currentHandInfo;
        if (handsInfo.TryGetValue(hand, out currentHandInfo))
        {
            OVRHand currentHand = currentHandInfo.hand;
            bool isAvailable = currentHand.isActiveAndEnabled && currentHand.IsTracked && currentHand.IsDataValid && currentHand.IsDataHighConfidence;
            return isAvailable;
        }

        return false;

    }

    public static Transform GetCenterEyeAnchorTransform()
    {
        if (!headInfoCached) cacheHeadInfo();

        return centerEyeTransform;
    }

    public static OVRHand GetOVRHand(OVRHand.Hand hand)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                return currentHandInfo.hand;
            }
            else
            {
                return null;
            }
        }

        return null;
    }
    
    public static OVRSkeleton GetOVRSkeleton(OVRHand.Hand hand)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                return currentHandInfo.handSkeleton;
            }
            else
            {
                return null;
            }
        }

        return null;
    } 

    public static Transform GetBoneTransform(OVRHand.Hand hand, OVRSkeleton.BoneId boneID)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                int indexFromBoneID = (int)boneID;
                return currentHandInfo.handSkeleton.Bones[indexFromBoneID].Transform;
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    public static Vector3 GetPalmPosition(OVRHand.Hand hand)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            // retrieve hand
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                return (currentHandInfo.handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position +
                       currentHandInfo.handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform.position +
                       currentHandInfo.handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb0].Transform.position) / 3.0f;
            }
        }

        // default sentinel value
        return Vector3.zero;
    }

    public static Vector3 GetPalmNormal(OVRHand.Hand hand)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            // retrieve hand
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                int indexFromBoneID = (int)OVRSkeleton.BoneId.Hand_Index1;
                if (hand == OVRHand.Hand.HandRight)
                {
                    return -currentHandInfo.handSkeleton.Bones[indexFromBoneID].Transform.up;
                }
                else
                {
                    return currentHandInfo.handSkeleton.Bones[indexFromBoneID].Transform.up;
                }
            }
        }

        // default sentinel value, consider optionals
        return Vector3.zero;
    }

    public static float GetBoneDistance(OVRHand.Hand hand, OVRSkeleton.BoneId boneIDa, OVRSkeleton.BoneId boneIDb)
    {
        // If handInfo is not cached, cache.
        if (!handsCached) cacheHandInfo();

        if (IsAvailable(hand))
        {
            HandInfo currentHandInfo;
            if (handsInfo.TryGetValue(hand, out currentHandInfo))
            {
                int indexFromBoneIDa = (int)boneIDa;
                int indexFromBoneIDb = (int)boneIDb;
                return (
                    currentHandInfo.handSkeleton.Bones[indexFromBoneIDa].Transform.position -
                    currentHandInfo.handSkeleton.Bones[indexFromBoneIDb].Transform.position
                    ).magnitude;
            }
            else
            {
                // TODO: returning a sentinel value is a bad idea
                return -1.0f;
            }
        }
        // TODO: returning a sentinel value is a bad idea
        return -1.0f;
    }


}