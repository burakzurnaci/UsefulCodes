using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace UsefulCodes.Astar.Scripts
{
    public class PathRequestManager : MonoBehaviour
    {
        private Queue<PathResult> _results = new Queue<PathResult>();
        private static PathRequestManager instance;
        private PathFinding _pathFinding;


        private void Awake()
        {
            instance = this;
            _pathFinding = GetComponent<PathFinding>();
        }

        private void Update()
        {
            if (_results.Count > 0)
            {
                int itemInQueue = _results.Count;
                lock (_results)
                {
                    for (int i = 0; i < itemInQueue; i++)
                    {
                        PathResult result = _results.Dequeue();
                        result.Callback(result.Path, result.Success);
                    }
                }
            }
        }


        public static void RequestPath(PathRequest request)
        {
            ThreadStart threadStart = delegate
            {
                instance._pathFinding.FindPath(request, instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }

      

        public void FinishedProcessingPath(PathResult result)
        {
            lock (_results)
            {
                _results.Enqueue(result);
            }
            
        }
        
    }
    public struct PathResult
    {
        public Vector3[] Path;
        public bool Success;
        public Action<Vector3[], bool> Callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            Path = path;
            Success = success;
            Callback = callback;
        }

    }
    
    public struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> Callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            PathStart = _start;
            PathEnd = _end;
            Callback = _callback;

        }
    }
    
    
    
}



