using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class PathGraphDrawer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Color pathColor = Color.red;
    [SerializeField] private bool drawInGameView = true;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float animationDuration = 4f;
    [SerializeField] private float dotSize = 5f;
    [SerializeField] private float dotSpacing = 18f;
    [SerializeField] private float trailLength = 90f;

    private float animationStartTime;

    private void Reset()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void OnEnable()
    {
        animationStartTime = Time.time;
    }

    private void OnGUI()
    {
        // if (!drawInGameView || Event.current.type != EventType.Repaint)
        // {
        //     return;
        // }

        // float elapsedTime = Time.time - animationStartTime;
        // if (elapsedTime >= animationDuration)
        // {
        //     return;
        // }

        // Camera cameraToUse = targetCamera != null ? targetCamera : Camera.main;
        // if (cameraToUse == null || gridManager == null || gridManager. == null)
        // {
        //     return;
        // }

        // Dictionary<int, PathNode> nodesById = new Dictionary<int, PathNode>();
        // foreach (PathNode node in gridManager.pathNodeDefinitions)
        // {
        //     if (node != null && !nodesById.ContainsKey(node.Id))
        //     {
        //         nodesById.Add(node.Id, node);
        //     }
        // }

        // List<PathNode> pathNodes = BuildPath(nodesById);
        // DrawScreenPath(pathNodes, cameraToUse, elapsedTime);
    }

    private List<PathNode> BuildPath(Dictionary<int, PathNode> nodesById)
    {
        HashSet<int> nodeIdsWithIncomingEdges = new HashSet<int>();
        foreach (PathNode node in nodesById.Values)
        {
            if (node.NeighborIds == null)
            {
                continue;
            }

            foreach (int neighbourId in node.NeighborIds)
            {
                if (nodesById.ContainsKey(neighbourId))
                {
                    nodeIdsWithIncomingEdges.Add(neighbourId);
                }
            }
        }

        PathNode startNode = null;
        foreach (PathNode node in nodesById.Values)
        {
            if (!nodeIdsWithIncomingEdges.Contains(node.Id))
            {
                startNode = node;
                break;
            }
        }

        if (startNode == null)
        {
            foreach (PathNode node in nodesById.Values)
            {
                startNode = node;
                break;
            }
        }

        List<PathNode> pathNodes = new List<PathNode>();
        HashSet<int> visitedNodeIds = new HashSet<int>();
        PathNode currentNode = startNode;

        while (currentNode != null && visitedNodeIds.Add(currentNode.Id))
        {
            pathNodes.Add(currentNode);

            PathNode nextNode = null;
            if (currentNode.NeighborIds != null)
            {
                foreach (int neighbourId in currentNode.NeighborIds)
                {
                    if (nodesById.TryGetValue(neighbourId, out nextNode) &&
                        !visitedNodeIds.Contains(nextNode.Id))
                    {
                        break;
                    }

                    nextNode = null;
                }
            }

            currentNode = nextNode;
        }

        return pathNodes;
    }

    private void DrawScreenPath(
        List<PathNode> pathNodes,
        Camera cameraToUse,
        float elapsedTime)
    {
        if (pathNodes.Count < 2)
        {
            return;
        }

        List<Vector2> screenPoints = new List<Vector2>();
        foreach (PathNode node in pathNodes)
        {
            Vector3 screenPoint = cameraToUse.WorldToScreenPoint(node.CellPosition);
            if (screenPoint.z <= 0f)
            {
                return;
            }

            screenPoint.y = Screen.height - screenPoint.y;
            screenPoints.Add(screenPoint);
        }

        float totalLength = 0f;
        for (int index = 1; index < screenPoints.Count; index++)
        {
            totalLength += Vector2.Distance(screenPoints[index - 1], screenPoints[index]);
        }

        if (totalLength <= Mathf.Epsilon)
        {
            return;
        }

        float progress = Mathf.Clamp01(elapsedTime / animationDuration);
        float headDistance = totalLength * progress;
        float visibleTrailLength = Mathf.Min(trailLength, headDistance);

        GUI.color = pathColor;
        for (float distanceFromHead = 0f;
             distanceFromHead <= visibleTrailLength;
             distanceFromHead += dotSpacing)
        {
            Vector2 dotPosition = GetPointAtDistance(
                screenPoints,
                headDistance - distanceFromHead);
            GUI.DrawTexture(
                new Rect(
                    dotPosition.x - dotSize * 0.5f,
                    dotPosition.y - dotSize * 0.5f,
                    dotSize,
                    dotSize),
                Texture2D.whiteTexture);
        }

        GUI.color = Color.white;
    }

    private static Vector2 GetPointAtDistance(List<Vector2> points, float distance)
    {
        distance = Mathf.Max(0f, distance);
        for (int index = 1; index < points.Count; index++)
        {
            Vector2 start = points[index - 1];
            Vector2 end = points[index];
            float segmentLength = Vector2.Distance(start, end);

            if (distance <= segmentLength)
            {
                float segmentProgress = segmentLength <= Mathf.Epsilon
                    ? 0f
                    : distance / segmentLength;
                return Vector2.Lerp(start, end, segmentProgress);
            }

            distance -= segmentLength;
        }

        return points[points.Count - 1];
    }

}
