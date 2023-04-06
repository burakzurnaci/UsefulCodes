using UnityEngine;

namespace UsefulCodes.Astar.Scripts
{
    public class Node
    {
        public bool Walkable;
        public Vector3 WorldPosition;

        public Node(bool walkable, Vector3 worldPos)
        {
            Walkable = walkable;
            WorldPosition = worldPos;
        }
    }
}
