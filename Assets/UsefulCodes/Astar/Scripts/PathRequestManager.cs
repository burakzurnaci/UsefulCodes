using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class PathRequestManager : MonoBehaviour
    {
        private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;

        private static PathRequestManager instance;
        private PathFinding _pathFinding;

        private bool _isProcessingPath;

        private void Awake()
        {
            instance = this;
            _pathFinding = GetComponent<PathFinding>();
        }


        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance._pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        void TryProcessNext()
        {
            if (!_isProcessingPath && _pathRequestQueue.Count > 0)
            {
                _currentPathRequest = _pathRequestQueue.Dequeue();
                _isProcessingPath = true;
                _pathFinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            _currentPathRequest.Callback(path, success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        struct PathRequest
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
}
