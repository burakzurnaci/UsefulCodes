using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Node : IHeapItem<Node>
    {
        public bool Walkable;
        public Vector3 WorldPosition;
        public int GridX;
        public int GridZ;
        public int MovementPenalty;
        
        public int GCost;
        public int HCost;
        public Node Parent;
        private int heapIndex;
        

        public Node(bool walkable, Vector3 worldPos, int gridX, int gridZ, int _penalty)
        {
            Walkable = walkable;
            WorldPosition = worldPos;
            GridX = gridX;
            GridZ = gridZ;
            MovementPenalty = _penalty;
        }

        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(nodeToCompare.HCost);
            }

            return -compare;
        }
    }
    
    
    
    
}

