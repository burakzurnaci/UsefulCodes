using System;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Grid : MonoBehaviour
    {
        public Transform Player;
        public LayerMask UnwalkableMask;
        public Vector2 GridWorldSize;
        public float NodeRadius;
        private Node[,] _grid;

        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeZ;

        private void Start()
        {
            _nodeDiameter = NodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(GridWorldSize.x/_nodeDiameter);
            _gridSizeZ = Mathf.RoundToInt(GridWorldSize.y/_nodeDiameter);
            CreateGrid();
            
        }

        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeZ];
            Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.forward * GridWorldSize.y / 2;

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeZ; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) + 
                                         Vector3.forward * (y * _nodeDiameter + NodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, NodeRadius,UnwalkableMask));
                    _grid[x, y] = new Node(walkable, worldPoint);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
            float percentY = (worldPosition.z + GridWorldSize.y / 2) / GridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeZ - 1) * percentY);

            return _grid[x, y];


        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position,new Vector3(GridWorldSize.x,1,GridWorldSize.y));

            if (_grid != null)
            {
                Node playerNode = NodeFromWorldPoint(Player.position);
                foreach (Node n in _grid)
                {
                    Gizmos.color = (n.Walkable) ? Color.white : Color.red;
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawCube(n.WorldPosition,Vector3.one * (_nodeDiameter-.1f));
                }
            }
        }
    }
}
