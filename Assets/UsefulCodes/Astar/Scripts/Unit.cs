using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulCodes.Astar.Scripts;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float Speed = 20;
    public float TurnDst = 5;

    private LinePath _path;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
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

    IEnumerator FollowPath()
    {
        while (true) {
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
