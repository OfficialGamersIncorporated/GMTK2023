using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPathfinder2D {

    //private void Awake() {
    //    Debug.LogError("RoomPathfinder2D should not be added to a gameobject as a component. Call its static methods isntead.");
    //}

    static void DisplayPathRecursive(RoomPathfindingNode node) {
        node.Room.FCost.color = Color.red;
        if (node.Parent != null)
            DisplayPathRecursive(node.Parent);
    }
    static List<Vector3> CalculateVector3PathFromFinalNode(RoomPathfindingNode finalNode) {
        List<Vector3> path = new List<Vector3>();

        void crawlNodePathRecursive(RoomPathfindingNode node) {
            path.Add(node.Room.transform.position);
            if(node.Parent != null)
                crawlNodePathRecursive(node.Parent);
        }

        crawlNodePathRecursive(finalNode);

        return path;
    }

    static RoomPathfindingNode GetRoomWithLowestFCost(List<RoomPathfindingNode> nodes, Vector3 startPos, Vector3 endPos) {
        RoomPathfindingNode cheapestRoom = nodes[0];
        float lowestFCost = Mathf.Infinity;
        float lowestHCost = Mathf.Infinity;

        foreach(RoomPathfindingNode node in nodes) {
            float gCost = node.GetGCost();
            float hCost = node.GetHCost(endPos);
            float fCost = gCost + hCost; //node.GetFCost(endPos);
            if(fCost < lowestFCost || (fCost == lowestFCost && hCost <= lowestHCost)) {
                lowestFCost = fCost;
                lowestHCost = hCost;
                cheapestRoom = node;
            }
        }

        return cheapestRoom;
    }
    static RoomPathfindingNode FindRoomInNodes(List<RoomPathfindingNode> nodes, Room room) {
        foreach(RoomPathfindingNode node in nodes) {
            if(node.Room == room)
                return node;
        }
        return null;
    }
    static Room GetClosestRoom(Vector3 position) {
        float closestRoomDistance = Mathf.Infinity;
        Room closestRoom = null;

        Collider2D[] overlappingColliders = Physics2D.OverlapPointAll(position, LayerMask.GetMask("Room"));
        foreach(Collider2D overlappingCollider in overlappingColliders) {
            if(!overlappingCollider.CompareTag("Room")) continue;

            float distance = Vector3.Distance(position, overlappingCollider.transform.position);
            Room room = overlappingCollider.GetComponent<Room>();
            if(distance <= closestRoomDistance && room) {
                closestRoom = room;
                closestRoomDistance = distance;
            }
        }

        return closestRoom;
    }

    public static List<Vector3> Pathfind(Vector3 startPos, Vector3 targetPos) { // List<Vector3>
        //yield return new WaitUntil(() => Input.GetButtonDown("Fire2"));
        //List<Vector3> path = new List<Vector3>();

        Room startRoom = GetClosestRoom(startPos);
        Room targetRoom = GetClosestRoom(targetPos);

        if(startRoom == null) {
            Debug.LogError("An AI agent is not in a room or even near a room.");
            return null;
        }

        //path.Add(startRoom.transform.position);
        // go to the door of the room that leads where we want to go
        // go to the center of that next room
        // go to the next door of that room that lead where we want to go
        // etc.

        // A*
        List<RoomPathfindingNode> toBeEvaluated = new List<RoomPathfindingNode>();
        List<RoomPathfindingNode> alreadyEvaluated = new List<RoomPathfindingNode>();
        toBeEvaluated.Add(new RoomPathfindingNode(startRoom));

        RoomPathfindingNode finalNode = null;

        while(true) {
            //yield return new WaitUntil(() => Input.GetButtonDown("Fire2"));

            RoomPathfindingNode current = GetRoomWithLowestFCost(toBeEvaluated, startPos, targetPos);
            toBeEvaluated.Remove(current);
            current.GetFCost(targetPos);
            current.AlreadyEvaluated = true;
            alreadyEvaluated.Add(current);

            if(current.Room == targetRoom) {
                finalNode = current;
                break;
            }

            foreach(RoomConnection neighborRoomCon in current.Room.ConnectedRooms) {
                if(FindRoomInNodes(alreadyEvaluated, neighborRoomCon.RoomConnected) != null) continue;

                RoomPathfindingNode neighborNode = FindRoomInNodes(toBeEvaluated, neighborRoomCon.RoomConnected);
                if (neighborNode == null || neighborNode.CalculateGCostWithHypotheticalParentRecursive(current) < neighborNode.GetGCost()) {
                    if(neighborNode == null) {
                        neighborNode = new RoomPathfindingNode(neighborRoomCon.RoomConnected);
                        toBeEvaluated.Add(neighborNode);
                    }
                    neighborNode.Parent = current;
                    neighborNode.GetFCost(targetPos);
                }
            }
        }

        DisplayPathRecursive(finalNode);
        return CalculateVector3PathFromFinalNode(finalNode);
        
        //return path;

    }

}

class RoomPathfindingNode {
    public Room Room;
    float GCost = -1;
    public RoomPathfindingNode Parent = null;
    public bool AlreadyEvaluated = false;

    public float CalculateGCostWithHypotheticalParentRecursive(RoomPathfindingNode hypotheticalParent) {
        float distance = Vector3.Distance(Room.transform.position, hypotheticalParent.Room.transform.position);
        return distance + Parent.GetGCost();
    }
    public float GetFCost(Vector3 endPos) {
        float hCost = GetHCost(endPos);
        float gCost = GetGCost();
        Debug.Log(Room.FCost);
        Room.FCost.text = (hCost + gCost).ToString();
        Room.GCost.text = GCost.ToString();
        Room.HCost.text = hCost.ToString();
        return hCost + gCost;
    }
    public float GetHCost(Vector3 endPos) {
        return Vector3.Distance(Room.transform.position, endPos);
    }
    public float GetGCost() {
        if(AlreadyEvaluated)
            return GCost;
        else
            return CalculateGCostRecursive();
    }
    float CalculateGCostRecursive() {
        if(Parent == null) return 0;

        float distance = Vector3.Distance(Room.transform.position, Parent.Room.transform.position);
        GCost = distance + Parent.CalculateGCostRecursive();
        return GCost;
    }

    public RoomPathfindingNode(Room room) {
        Room = room;
    }
}