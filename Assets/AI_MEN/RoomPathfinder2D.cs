using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPathfinder2D : MonoBehaviour {

    private void Awake() {
        Debug.LogError("RoomPathfinder2D should not be added to a gameobject as a component. Call its static methods isntead.");
    }

    public static List<Vector3> Pathfind(Vector3 startPos, Vector3 targetPos) {

        List<Vector3> path = new List<Vector3>();

        float closestRoomDistance = 1000;
        Room closestRoom = null;

        Collider2D[] overlappingColliders = Physics2D.OverlapPointAll(startPos, LayerMask.GetMask("Room"));
        foreach(Collider2D overlappingCollider in overlappingColliders) {
            if(!overlappingCollider.CompareTag("Room")) continue;

            float distance = Vector3.Distance(startPos, overlappingCollider.transform.position);
            Room room = overlappingCollider.GetComponent<Room>();
            if (distance <= closestRoomDistance && room) {
                closestRoom = room;
                closestRoomDistance = distance;
            }
        }

        if(closestRoom == null) {
            Debug.LogError("An AI agent is not in a room or even near a room.");
            return null;
        }

        path.Add(closestRoom.transform.position);
        // go to the door of the room that leads where we want to go
        // go to the center of that next room
        // go to the next door of that room that lead where we want to go
        // etc.

        return path;

    }

}
