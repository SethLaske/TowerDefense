using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Pathing : MonoBehaviour
{
    public SplineContainer spline;

    public int numberOfLines;
    public float distanceBetweenLines;
    
    public float lineLength;

    public List<Line> generatedLines = new List<Line>();

    private void Awake()
    {
        generatedLines.Clear();
        
        GenerateWithDistance();
    }

    private void GenerateWithCount()
    {
        if (spline == null || numberOfLines < 2 || lineLength < 0)
        {
            return;
        }
        
        for (int i = 0; i < numberOfLines; i++)
        {
            float t = (float)i / (numberOfLines - 1);

            Vector3 position = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);
            
            generatedLines.Add(new Line(position, tangent, lineLength));
        }
    }
    
    private void GenerateWithDistance()
    {
        if (spline == null || distanceBetweenLines <= 0 || lineLength < 0)
        {
            return;
        }

        float spineLength = spline.CalculateLength();

        if (spineLength < distanceBetweenLines)
        {
            return;
        }

        int lineCount = (int)(spineLength / distanceBetweenLines);

        for (int i = 0; i < lineCount; i++)
        {
            float t = (float)i / (lineCount - 1);

            Vector3 position = spline.EvaluatePosition(t);
            Vector3 tangent = spline.EvaluateTangent(t);
            
            generatedLines.Add(new Line(position, tangent, lineLength));
        }
    }

    public Vector2 GetNextTargetPoint(int currentPoint, float t)
    {
        int nextPoint = currentPoint + 1;

        if (nextPoint >= generatedLines.Count)
        {
            //BAD
            return Vector2.zero;
        }

        return generatedLines[nextPoint].GetPositionOnLine(t);
    }

    private void OnDrawGizmos()
    {
        if (generatedLines == null || generatedLines.Count < 1)
        {
            return;
        }

        Gizmos.color = Color.gray;
        
        foreach (Line line in generatedLines)
        {
            Gizmos.DrawLine(line.startingPoint, line.finishPoint);
        }
    }
}

[Serializable]
public class Line
{
    public Vector2 startingPoint;
    public Vector2 finishPoint;

    public Line(Vector2 midpoint, Vector2 splineDirection, float length)
    {
        Vector2 normal = new Vector2(-splineDirection.y, splineDirection.x).normalized;
        
        startingPoint = midpoint - normal * (length / 2);
        finishPoint = midpoint + normal * (length / 2);
    }

    public Vector2 GetPositionOnLine(float t)
    {
        return Vector2.Lerp(startingPoint, finishPoint, t);
    }
}