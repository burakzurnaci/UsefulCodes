using System;
using System.Collections;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Unit : MonoBehaviour
    {
        private const float _minPathUpdateTime = .2f;
        private const float _pathUpdateMoveThreshold = .5f;
    
        public Transform target;
        public float Speed = 20;
        public float TurnDst = 5;
        public float TurnSpeed = 3;
        public float StoppingDistance =10;

        private LinePath _path;

        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        public void OnPathFound(Vector3[] wayPoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _path = new LinePath(wayPoints,transform.position,TurnDst,StoppingDistance);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f) {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(new PathRequest(transform.position,target.position,OnPathFound));
            
            float sqrMoveThreshold = _pathUpdateMoveThreshold * _pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;
            
            while (true)
            {
                yield return new WaitForSeconds(_minPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position,target.position,OnPathFound));
                    targetPosOld = target.position;
                }
                
            }
        }
        

        IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(_path.LookPoints[0]);

            float speedPercent = 1;
            
            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (_path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == _path.FinishLineIndex) {
                        followingPath = false;
                        break;
                    }else {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {

                    if (pathIndex >= _path.SlowDownIndex && StoppingDistance > 0) {
                        speedPercent = Mathf.Clamp01(_path.TurnBoundaries[_path.FinishLineIndex].DistanceFromPoint(pos2D) / StoppingDistance);
                        if (speedPercent < 0.01f) {
                            followingPath = false;
                        }
                    }
                    
                    Quaternion targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,Time.deltaTime*TurnSpeed);
                    transform.Translate(Vector3.forward*Time.deltaTime*Speed*speedPercent,Space.Self);
                }
            
                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if (_path != null)
            {
                _path.DrawWithGizmos();
            }
        }
    }
}
