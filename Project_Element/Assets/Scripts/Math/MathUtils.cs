// From Konstantine

using UnityEngine;
using System;

public class MathUtils
{
    public static float CompareEpsilon = 0.00001f;

    //Eases from the start to the end.  This is meant to be called over many frames.  The
    //values will change fast at first and gradually slow down.
    public static float LerpTo(float easeSpeed, float start, float end, float dt)
    {
        float diff = end - start;

        diff *= Mathf.Clamp(dt * easeSpeed, 0.0f, 1.0f);

        return diff + start;
    }

    //Eases angles from the start to the end.  This is meant to be called over many frames.  The
    //values will change fast at first and gradually slow down. 
    //This function was written assuming start and end are between 0 and 360
    public static float LerpToAngle(float easeSpeed, float start, float end, float dt)
    {
        float diff = end - start;

        if (diff > 180.0f)
        {
            diff = diff - 360.0f;
        }

        diff *= Mathf.Clamp(dt * easeSpeed, 0.0f, 1.0f);

        return diff + start;
    }

    //Eases from the start to the end.  This is meant to be called over many frames.  The
    //values will change fast at first and gradually slow down.
    public static Vector3 LerpTo(float easeSpeed, Vector3 start, Vector3 end, float dt)
    {
        Vector3 diff = end - start;

        diff *= Mathf.Clamp(dt * easeSpeed, 0.0f, 1.0f);

        return diff + start;
    }

    //Eases from the start to the end.  This is meant to be called over many frames.  The
    //values will change fast at first and gradually slow down.
    public static Vector3 SlerpTo(float easeSpeed, Vector3 start, Vector3 end, float dt)
    {
        float percent = Mathf.Clamp(dt * easeSpeed, 0.0f, 1.0f);

        return Vector3.Slerp(start, end, percent);
    }

    //Eases from the start to the end.  This is meant to be called over many frames.  The
    //values will change fast at first and gradually slow down.
    public static Vector3 SlerpToHoriz(float easeSpeed, Vector3 start, Vector3 end, Vector3 slerpCenter, float dt)
    {
        Vector3 startOffset = start - slerpCenter;
        Vector3 endOffset = end - slerpCenter;

        startOffset.y = 0.0f;
        endOffset.y = 0.0f;

        float percent = Mathf.Clamp(dt * easeSpeed, 0.0f, 1.0f);

        Vector3 result = Vector3.Slerp(startOffset, endOffset, percent) + slerpCenter;
        result.y = start.y;

        return result;
    }

    //Use this to compare floating point numbers, when you want to allow for a small degree of error
    public static bool AlmostEquals(float v1, float v2, float epsilon)
    {
        return Mathf.Abs(v2 - v1) <= epsilon;
    }

    //Use this to compare floating point numbers, when you want to allow for a small degree of error
    public static bool AlmostEquals(float v1, float v2)
    {
        return AlmostEquals(v1, v2, CompareEpsilon);
    }

    //Clamps a vector along the x-z plane
    public static Vector3 HorizontalClamp(Vector3 v, float maxLength)
    {
        float horizLengthSqrd = v.x * v.x + v.z * v.z;

        if (horizLengthSqrd <= maxLength * maxLength)
        {
            return v;
        }

        float horizLength = Mathf.Sqrt(horizLengthSqrd);

        v.x *= maxLength / horizLength;
        v.z *= maxLength / horizLength;

        return v;
    }

    //This function will project a point inside a capsule to the bottom of the capsule. 
    //The capsule is assumed to be oriented along the y-axis.
    public static Vector3 ProjectToBottomOfCapsule(
        Vector3 ptToProject,
        Vector3 capsuleCenter,
        float capsuleHeight,
        float capsuleRadius
        )
    {
        //Calculating the length of the line segment part of the capsule
        float lineSegmentLength = capsuleHeight - 2.0f * capsuleRadius;

        //Clamp line segment length
        lineSegmentLength = Math.Max(lineSegmentLength, 0.0f);
        
        //Calculate the line segment that goes along the capsules "Height"
        Vector3 bottomLineSegPt = capsuleCenter;
        bottomLineSegPt.y -= lineSegmentLength * 0.5f;

        //Get displacement from bottom of line segment
        Vector3 ptDisplacement = ptToProject - bottomLineSegPt;

        //Calculate needed distances
        float horizDistSqrd = ptDisplacement.x * ptDisplacement.x + ptDisplacement.z * ptDisplacement.z;
        
        float radiusSqrd = capsuleRadius * capsuleRadius;

        //The answer will be undefined if the pt is horizontally outside of the capsule
        if (horizDistSqrd > radiusSqrd)
        {
            return ptToProject;
        }

        //Calc projected pt
        float heightFromSegPt = -Mathf.Sqrt(radiusSqrd - horizDistSqrd);

        Vector3 projectedPt = ptToProject;
        projectedPt.y = bottomLineSegPt.y + heightFromSegPt;

        return projectedPt;
    }

    //Returns the angle from the horizontal plane of the direction in degrees.  
    //NOTE: This assumes the direction is normalized
    public static float CalcVerticalAngle(Vector3 dir)
    {
        //                 /|
        //                / |
        //               /  |
        //            h /   |
        //             /    | o
        //            /     |
        //           /ang   |
        //          /_______|
        //
        //sin(ang) = o/h, but since the dir is a unit vector h = 1
        //The angle will be: Asin(o)
        return Mathf.Rad2Deg * Mathf.Asin(dir.y);
    }

    public static Vector3 HorizontalClamp(Vector3 v, float minLength, float maxLength)
    {
        float horizLengthSqrd = v.x * v.x + v.z * v.z;

        if (horizLengthSqrd < minLength * minLength)
        {
            if (horizLengthSqrd > 0.0f)
            {
                float horizLength = Mathf.Sqrt(horizLengthSqrd);

                v.x *= minLength / horizLength;
                v.z *= minLength / horizLength;
            }
            else
            {
                //The direction of the vector is undefined in this case.  Choosing the z axis and using 
                //that.  
                v = Vector3.forward * minLength;
            }
        }
        else if (horizLengthSqrd > maxLength * maxLength)
        {
            float horizLength = Mathf.Sqrt(horizLengthSqrd);

            v.x *= maxLength / horizLength;
            v.z *= maxLength / horizLength;
        }

        return v;
    }
}
