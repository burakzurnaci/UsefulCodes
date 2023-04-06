using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Node
    {
        public bool Walkable;
        public Vector3 WorldPosition;
        public int GridX;
        public int GridZ;
        
        public int GCost;
        public int HCost;
        public Node Parent;
        

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridZ)
        {
            Walkable = walkable;
            WorldPosition = worldPos;
            GridX = gridX;
            GridZ = gridZ;
        }

        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }
    }
}

