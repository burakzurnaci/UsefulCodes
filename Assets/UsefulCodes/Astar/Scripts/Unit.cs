using System;
using System.Collections;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Unit : MonoBehaviour
    {
        private const float minPathUpdateTime = .2f;
        private const float pathUpdateMoveThreshold = .5f;
    
        public Transform target;
        public float Speed = 20;
        public float TurnDst = 5;
        public float TurnSpeed = 3;

        private LinePath _path;

        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        public void OnPathFound(Vector3[] wayPoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _path = new LinePath(wayPoints,transform.position,TurnDst);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f) {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
            
            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;
            
            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
                    targetPosOld = target.position;
                }
                
            }
        }
        

        IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(_path.LookPoints[0]);
        
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

                if (followingPath) {
                    Quaternion targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,Time.deltaTime*TurnSpeed);
                    transform.Translate(Vector3.forward*Time.deltaTime*Speed,Space.Self);
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
