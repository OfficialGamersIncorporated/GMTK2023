using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour {
    public enum EnteranceBearing { North, South, East, West };
    public enum EnteranceType { BlankWall, Doorway };

    [SerializeField]
    public List<Enterance> Enterances = new List<Enterance>() {
        new Enterance(EnteranceBearing.North),
        new Enterance(EnteranceBearing.East),
        new Enterance(EnteranceBearing.South),
        new Enterance(EnteranceBearing.West),
    };
    [HideInInspector] // enable debugging in the inspector to view this in the editor.
    public List<RoomConnection> ConnectedRooms = new List<RoomConnection>();


    public RoomConnection FindConnectedRoomByBearing(EnteranceBearing bearing) {
        foreach(RoomConnection connectedRoom in ConnectedRooms) {
            if(connectedRoom.Bearing != bearing) continue;

            return connectedRoom;
        }
        return null;
    }
    public void SetConnectedRoomByBearing(EnteranceBearing bearing, Room other) {
        RoomConnection existingConnection = FindConnectedRoomByBearing(bearing);
        if (existingConnection != null) {
            ConnectedRooms.Remove(existingConnection);
        }

        ConnectedRooms.Add(new RoomConnection(other, bearing));
    }
    public Enterance FindEnteranceByBearing(EnteranceBearing bearing) {
        foreach(Enterance enteranceToUpdate in Enterances) {
            if(enteranceToUpdate.Bearing != bearing) continue;

            return enteranceToUpdate;
        }
        return null;
    }
    public void SetEnteranceType(EnteranceBearing bearing, EnteranceType newType) {
        Enterance enteranceToUpdate = FindEnteranceByBearing(bearing);
        if(enteranceToUpdate == null) return;

        enteranceToUpdate.BlankWall.SetActive(newType == EnteranceType.BlankWall);
        enteranceToUpdate.Doorway.SetActive(newType == EnteranceType.Doorway);
    }

    public void RefereshEnterances(bool cascade = true) {
        Collider2D newCollider = GetComponent<Collider2D>(); // the trigger collider used to check for neighboring rooms.
        Vector3 colliderPos = newCollider.transform.position;

        // check for overlapping collidersof layer "Room"
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Room"));
        filter.useTriggers = true;
        List<Collider2D> results = new List<Collider2D>();
        newCollider.OverlapCollider(filter, results);

        foreach(Collider2D overlappingRoomCollider in results) {
            if(!overlappingRoomCollider.CompareTag("Room")) continue;

            // the closest point on myself to the overlapping room
            Vector3 closestPoint = newCollider.ClosestPoint(overlappingRoomCollider.transform.position);
            Room overlappingRoom = overlappingRoomCollider.GetComponent<Room>();

            if(cascade)
                overlappingRoom.RefereshEnterances(false);

            if(Mathf.Abs(closestPoint.y - colliderPos.y) < 1) { // is on the same y level
                if(closestPoint.x - colliderPos.x > 0) { // is to the east
                    if(overlappingRoom.FindEnteranceByBearing(Room.EnteranceBearing.West) != null) { // check if the room to be connected to can connect.
                        SetConnectedRoomByBearing(Room.EnteranceBearing.East, overlappingRoom);
                        SetEnteranceType(Room.EnteranceBearing.East, Room.EnteranceType.Doorway);
                    }
                } else { // is to the west
                    if(overlappingRoom.FindEnteranceByBearing(Room.EnteranceBearing.East) != null) {
                        SetConnectedRoomByBearing(Room.EnteranceBearing.West, overlappingRoom);
                        SetEnteranceType(Room.EnteranceBearing.West, Room.EnteranceType.Doorway);
                    }
                }
            } else if(Mathf.Abs(closestPoint.x - colliderPos.x) < 1) { // is on the same x level
                if(closestPoint.y - colliderPos.y > 0) { // is to the north
                    if(overlappingRoom.FindEnteranceByBearing(Room.EnteranceBearing.South) != null) {
                        SetConnectedRoomByBearing(Room.EnteranceBearing.North, overlappingRoom);
                        SetEnteranceType(Room.EnteranceBearing.North, Room.EnteranceType.Doorway);
                    }
                } else { // is to the south
                    if(overlappingRoom.FindEnteranceByBearing(Room.EnteranceBearing.North) != null) {
                        SetConnectedRoomByBearing(Room.EnteranceBearing.South, overlappingRoom);
                        SetEnteranceType(Room.EnteranceBearing.South, Room.EnteranceType.Doorway);
                    }
                }
            }
        }
    }

}

[System.Serializable]
public class Enterance {
    public Room.EnteranceBearing Bearing;
    public GameObject BlankWall;
    public GameObject Doorway;

    public Enterance(Room.EnteranceBearing bearing) {
        Bearing = bearing;
    }
}
[System.Serializable]
public class RoomConnection {
    public Room.EnteranceBearing Bearing;
    public Room RoomConnected;

    public RoomConnection(Room other, Room.EnteranceBearing bearing) {
        Bearing = bearing;
        RoomConnected = other;
    }
}