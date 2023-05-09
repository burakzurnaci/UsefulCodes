using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace UsefulCodes.Astar.Scripts
{
    public class PathFinding : MonoBehaviour
    {
        private PathRequestManager _requestManager;
        private Grid _grid;

        private void Awake()
        {
            _requestManager = GetComponent<PathRequestManager>();
            _grid = GetComponent<Grid>();
        }
        
        public void StartFindPath(Vector3 startPos,Vector3 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }

        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] _waypoints = new Vector3[0];
            bool _pathSuccess = false;
            
            Node startNode = _grid.NodeFromWorldPoint(startPos);
            Node targetNode = _grid.NodeFromWorldPoint(targetPos);

            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        print("Path found: " + sw.ElapsedMilliseconds + " ms");
                        _pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
                        if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                        {
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetNode);
                            neighbour.Parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                            {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }

            yield return null;
            if (_pathSuccess)
            {
                _waypoints=RetracePath(startNode,targetNode);
            }
            _requestManager.FinishedProcessingPath(_waypoints,_pathSuccess);
        }

        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            Vector3[] wayPoints = SimplifyPath(path);
            Array.Reverse(wayPoints);
            return wayPoints;

        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> wayPoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew =
                    new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridZ - path[i].GridZ);
                if (directionNew != directionOld)
                {
                    wayPoints.Add(path[i].WorldPosition);
                }

                directionOld = directionNew;
            }

            return wayPoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstZ = Mathf.Abs(nodeA.GridZ - nodeB.GridZ);

            if (dstX > dstZ)
                return 14 * dstZ + 10 * (dstX - dstZ);
            return 14 * dstX + 10 * (dstZ - dstX);
        }
    }
}
