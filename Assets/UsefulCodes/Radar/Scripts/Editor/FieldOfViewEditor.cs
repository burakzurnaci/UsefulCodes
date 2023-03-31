using UnityEditor;
using UnityEngine;

namespace UsefulShaders.Radar.Scripts.Editor
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fow = (FieldOfView)target;
            Handles.color=Color.red;
            Handles.DrawWireArc(fow.transform.position,Vector3.up,Vector3.forward, 360,fow.vievRadius);
            Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
            Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
            
            Handles.DrawLine(fow.transform.position,fow.transform.position+viewAngleA*fow.vievRadius);
            Handles.DrawLine(fow.transform.position,fow.transform.position+viewAngleB*fow.vievRadius);
            Handles.color = Color.green;
            foreach (Transform visibleTarget in fow.visibleTargets)
            {
                Handles.DrawLine(fow.transform.position,visibleTarget.position);
            }
        }
    }
}


