using TMPro;
using UnityEngine;

namespace UsefulCodes.MyCodeLibrary.Utils
{
    public static class Buchi 
    {

        public static Mesh CreateEmptyMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            return mesh;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs,
            out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }
        
        //Get Mouse Position int World with Z=0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMousePositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        public static Vector3 GetMousePositionWithZ()
        {
            return GetMousePositionWithZ(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMousePositionWithZ(Camera worldCamera)
        {
            return GetMousePositionWithZ(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetMousePositionWithZ(Vector3 screenPosition,Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        
        
        //Create Text in the World
        public static TextMesh CreateWorldText(string text, Transform parent = null,  Vector3 localPosition = new Vector3(), int fontSize = 40,
            Color color = new Color(),TextAnchor textAnchor = TextAnchor.MiddleCenter,
            int sortingOrder=0,
            TextAlignment textAlignment = TextAlignment.Center)
        {
            if(color==null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
                sortingOrder);
        }
        
        //Create Text int the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor textAnchor, TextAlignment textAlignment , int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent,false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        
    }
}
