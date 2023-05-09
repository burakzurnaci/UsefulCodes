using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class LinePath
    {
        public readonly Vector3[] LookPoints;
        public readonly Line[] TurnBoundaries;
        public readonly int FinishLineIndex;

        public LinePath(Vector3[] wayPoints, Vector3 startPos, float turnDst)
        {
            LookPoints = wayPoints;
            TurnBoundaries = new Line[LookPoints.Length];
            FinishLineIndex = TurnBoundaries.Length - 1;

            Vector2 previousPoint = V3ToV2(startPos);
            for (int i = 0; i < LookPoints.Length; i++)
            {
                Vector2 currentPoint = V3ToV2(LookPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint =(i==FinishLineIndex)?currentPoint : currentPoint - dirToCurrentPoint * turnDst;
                TurnBoundaries[i] =  new Line(turnBoundaryPoint, previousPoint-dirToCurrentPoint * turnDst);
                previousPoint = turnBoundaryPoint;
            }
        }

        Vector2 V3ToV2(Vector3 V3)
        {
            return new Vector2(V3.x, V3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;

            foreach (Vector3 p in LookPoints) {
                Gizmos.DrawCube(p+Vector3.up,Vector3.one);
            }
            
            Gizmos.color=Color.white;
            foreach (Line l in TurnBoundaries) {
                l.DrawWithGizmos(10);
            }
            
            
        }
    }
}
