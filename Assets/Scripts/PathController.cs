using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/*
 * In order to give enemies width and to keep levels relatively simple to manage this script will tell the enemies where to walk
 * The spline will be split by some number of points along its total length. Each point will be the midpoint to a perpendicular line
 * Enemies will have some offset that will be followed along that path, thus not all being in a line
 * As the enemies pass each line they will request the path controller for the next line to move towards
 * This should also allow branching when I want it, the data structure will simply need to be paused
 */
public class PathController : MonoBehaviour
{
    public SplineContainer spline;

    //public int segmentCount = 2;

    public float pathWidth = .5f;

    private List<PathSegment> pathSegments = new List<PathSegment>();

    public float distanceBetweenSegments = .5f;

    private float trueDistanceBetweenSegments = 0;
    
    // If there's less than 10 segments I think there's a serious issue
    private const int MINIMUM_SEGMENTS = 10;

    private float splineLength = 0;
    
    public void Initialize()
    {
        if (spline == null || spline.Spline.Count < 2 || pathWidth <= 0)
        {
            Debug.LogError("[PathController] - unable to initialize");
            return;
        }

        if (pathSegments == null)
        {
            pathSegments = new List<PathSegment>();
        }

        splineLength = spline.CalculateLength();

        GeneratePathSegments();
    }

    [ContextMenu("Force Generate Segments")]
    private void GeneratePathSegments()
    {
        pathSegments.Clear();

        if (splineLength < distanceBetweenSegments)
        {
            Debug.LogError("[PathController] - No space for segments");
            return;
        }

        int segmentCount = (int)(splineLength / distanceBetweenSegments);

        if (segmentCount < MINIMUM_SEGMENTS)
        {
            Debug.LogError("[PathController] - Too few segments for valid path");
            return;
        }

        trueDistanceBetweenSegments = splineLength / (segmentCount - 1);
        
        PathSegment previousSegment = null;
        PathSegment currentSegment = null;
        
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);

            Vector3 position = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);

            currentSegment = new PathSegment(position, tangent, pathWidth, segmentCount - i);
            pathSegments.Add(currentSegment);

            if (i > 0 && previousSegment != null)
            {
                currentSegment.SetPreviousSegment(previousSegment);
                previousSegment.SetNextSegment(currentSegment);
            }

            previousSegment = currentSegment;
        }
    }

    public PathSegment GetSegmentAtIndex(int argSegmentIndex)
    {
        if (pathSegments == null || pathSegments.Count <= argSegmentIndex)
        {
            return null;
        }

        return pathSegments[argSegmentIndex];
    }

    public Vector2 GetNextTargetPoint(int currentPoint, float argSegmentOffset)
    {
        if (pathSegments == null)
        {
            Debug.LogError("[PathController] - Path not available");
            return Vector2.zero;
        }

        int nextPoint = currentPoint + 1;

        if (nextPoint >= pathSegments.Count)
        {
            //Reached end of path
            return Vector2.zero;
        }

        return pathSegments[nextPoint].GetPositionOnLine(argSegmentOffset);
    }

    public bool IsPointOnPath(Vector2 argCheckedPoint)
    {
        int checkingIndex = 0;

        //Adding some tolerance due to the circle checks happening
        float targetDistance = 1.0f * (pathWidth / 2);
        
        while (checkingIndex < pathSegments.Count)
        {
            float distanceToPoint = (argCheckedPoint - pathSegments[checkingIndex].centerPoint).magnitude;
            
            if (distanceToPoint < targetDistance)
            {
                return true;
            }

            float minimumDistanceNeeded = distanceToPoint - targetDistance;

            if (minimumDistanceNeeded > trueDistanceBetweenSegments)
            {
                checkingIndex += (int)(minimumDistanceNeeded / trueDistanceBetweenSegments);
            }
            else
            {
                checkingIndex++;
            }
        }

        return false;
    }
    
    public Vector2 FindClosestPathPointToPoint(Vector2 argCentralPoint, Vector2 argDefaultPoint)
    {
        int checkingIndex = 0;

        Vector2 closestPoint = argDefaultPoint;
        
        float closestDistance = (closestPoint - argCentralPoint).sqrMagnitude;
        
        while (checkingIndex < pathSegments.Count)
        {
            float distanceToCheckedPoint = (pathSegments[checkingIndex].centerPoint - argCentralPoint).sqrMagnitude;

            float distanceDifference = distanceToCheckedPoint - closestDistance;
            
            if (distanceDifference < 0)
            {
                //A new closest segment was found
                closestPoint = pathSegments[checkingIndex].centerPoint;
                closestDistance = distanceToCheckedPoint;
                
                //Only increment by 1 at a time once a closer one has been found
                checkingIndex ++;
            }
            else
            {
                checkingIndex += Mathf.Max((int)(Mathf.Sqrt(distanceDifference) / trueDistanceBetweenSegments), 1);
            }
            
        }

        return closestPoint;
    }

    private void OnDrawGizmos()
    {
        if (pathSegments == null || pathSegments.Count < 1)
        {
            return;
        }

        Gizmos.color = Color.gray;
        
        foreach (PathSegment line in pathSegments)
        {
            Gizmos.DrawLine(line.startingPoint, line.finishPoint);
        }
    }
}


[Serializable]
public class PathSegment
{
    public Vector2 centerPoint { get; private set; }
    public Vector2 startingPoint { get; private set; }
    public Vector2 finishPoint { get; private set; }

    //Used to calculate which enemy is furthest along
    public int sectionsFromEnd { get; private set; }

    // Segment closer to the end of path
    public PathSegment nextSegment { get; private set; }
    
    // Segment closer to the start of path
    public PathSegment previousSegment { get; private set; }

    public PathSegment(Vector2 argMidpoint, Vector2 argSplineDirection, float argLength, int argSectionsToEnd)
    {
        centerPoint = argMidpoint;
        
        Vector2 normal = new Vector2(-argSplineDirection.y, argSplineDirection.x).normalized;
        
        startingPoint = argMidpoint - normal * (argLength / 2);
        finishPoint = argMidpoint + normal * (argLength / 2);

        sectionsFromEnd = argSectionsToEnd;
    }

    public void SetNextSegment(PathSegment argNextSegment)
    {
        if (nextSegment != null)
        {
            Debug.LogError("Segment already assigned");
        }

        nextSegment = argNextSegment;
    }
    
    public void SetPreviousSegment(PathSegment argPreviousSegment)
    {
        if (previousSegment != null)
        {
            Debug.LogError("Segment already assigned");
        }

        previousSegment = argPreviousSegment;
    }

    public Vector2 GetPositionOnLine(float argSegmentOffset)
    {
        return Vector2.Lerp(startingPoint, finishPoint, argSegmentOffset);
    }
}