using System;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Grid : MonoBehaviour
    {
        public bool DisplayGridGizmos;
        public LayerMask UnwalkableMask;
        public Vector2 GridWorldSize;
        public float NodeRadius;
        public TerrainType[] WalkableRegions;
        public int ObstacleProximityPenalty = 10;
        private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
        private LayerMask _walkableMask;
        private Node[,] _grid;

        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeZ;

        private int _penaltyMin = int.MaxValue;
        private int _penaltyMax = int.MinValue;

        public List<Node> Path;

        private void Awake()
        {
            _nodeDiameter = NodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(GridWorldSize.x/_nodeDiameter);
            _gridSizeZ = Mathf.RoundToInt(GridWorldSize.y/_nodeDiameter);

            foreach (TerrainType region in WalkableRegions)
            {
                _walkableMask.value |= region.TerrainMask.value;
                _walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value,2),region.TerrainPenalty);
            }
            
            CreateGrid();
            
        }

        public int MaxSize
        {
            get
            {
                return _gridSizeX * _gridSizeZ;
            }
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
                    int movementPenalty = 0;

                   
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, _walkableMask))
                    {
                        _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += ObstacleProximityPenalty;
                    }
                    
                    _grid[x, y] = new Node(walkable, worldPoint,x,y,movementPenalty);
                }
            }
            
            BlurPenaltyMap(3);
        }

        void BlurPenaltyMap(int blurSize)
        {
            int kernalSize = blurSize * 2 + 1;
            int kernelExtents = (kernalSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[_gridSizeX, _gridSizeZ];
            int[,] penaltiesVerticalPass = new int[_gridSizeX, _gridSizeZ];

            for (int z = 0; z < _gridSizeZ; z++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, z] += _grid[sampleX, z].MovementPenalty;
                }

                for (int x = 1; x < _gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1,0,_gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents,0,_gridSizeX-1);

                    penaltiesHorizontalPass[x, z] = penaltiesHorizontalPass[x - 1, z] -
                        _grid[removeIndex, z].MovementPenalty + _grid[addIndex, z].MovementPenalty;
                }
            }
            
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int z = -kernelExtents; z <= kernelExtents; z++)
                {
                    int sampleY = Mathf.Clamp(z, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x,sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernalSize * kernalSize));
                _grid[x, 0].MovementPenalty = blurredPenalty;
                
                for (int z = 1; z < _gridSizeZ; z++)
                {
                    int removeIndex = Mathf.Clamp(z - kernelExtents - 1,0,_gridSizeZ);
                    int addIndex = Mathf.Clamp(z + kernelExtents,0,_gridSizeZ-1);

                    penaltiesVerticalPass[x, z] = penaltiesVerticalPass[x, z-1] -penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, z] / (kernalSize * kernalSize));
                    _grid[x, z].MovementPenalty = blurredPenalty;

                    if (blurredPenalty > _penaltyMax)
                    {
                        _penaltyMax = blurredPenalty;
                    }

                    if (blurredPenalty<_penaltyMin)
                    {
                        _penaltyMin = blurredPenalty;
                    }
                }
            }
            
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if(x == 0 && z==0)
                        continue;
                    
                    int checkX = node.GridX + x;
                    int checkZ = node.GridZ + z;
                    if (checkX >= 0 && checkX < _gridSizeX && checkZ >= 0 && checkZ < _gridSizeZ)
                    {
                        neighbours.Add(_grid[checkX,checkZ]);
                    }
                }
            }
            return neighbours;
        }

        //Find Grid Node from worldPosition
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
        
        // Debug Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position,new Vector3(GridWorldSize.x,1,GridWorldSize.y));
            
                if (_grid != null && DisplayGridGizmos)
                {
                    foreach (Node n in _grid)
                    {
                        Gizmos.color = Color.Lerp(Color.white,Color.black,Mathf.InverseLerp(_penaltyMin,_penaltyMax,n.MovementPenalty));
                        
                        Gizmos.color = (n.Walkable) ? Gizmos.color : Color.red;
                        Gizmos.DrawCube(n.WorldPosition,Vector3.one * (_nodeDiameter));
                    }
                }
        }

        [Serializable]
        public class TerrainType
        {
            public LayerMask TerrainMask;
            public int TerrainPenalty;
        }
    }
}
