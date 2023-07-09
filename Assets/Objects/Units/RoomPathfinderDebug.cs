using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPathfinderDebug : MonoBehaviour {

    public Transform start;
    public Transform end;
    Camera cam;

    List<Vector3> path = new List<Vector3>();

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        if(Input.GetButtonDown("Fire2")) {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.
            //Vector3 SnappedHoverPoint = RoundV3To(mouseWorldPos, new Vector3(RoomGridSize.x, RoomGridSize.y, 1));
            end.position = mouseWorldPos;

            path = RoomPathfinder2D.Pathfind(start.position, end.position);
            Debug.DrawLine(mouseWorldPos + Vector3.left, mouseWorldPos + Vector3.right, Color.yellow, 2);
            Debug.DrawLine(mouseWorldPos + Vector3.up, mouseWorldPos + Vector3.down, Color.yellow, 2);
        }

        if(path == null) return;
        Vector3 last = start.position;
        foreach(Vector3 pathPoint in path) {
            Debug.DrawLine(last, pathPoint, Color.blue, 1);
            last = pathPoint;
        }
    }
}
