using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPathfinderDebug : MonoBehaviour {

    public Transform start;
    public Transform end;

    List<Vector3> path = new List<Vector3>();

    IEnumerator Start() {
        yield return new WaitUntil(() => Input.GetButtonDown("Fire2"));
        path = RoomPathfinder2D.Pathfind(start.position, end.position);
    }

    void Update() {
        if(Input.GetButtonDown("Fire3")) {

        }

        Vector3 last = start.position;
        foreach(Vector3 pathPoint in path) {
            Debug.DrawLine(last, pathPoint, Color.blue, 1);
            last = pathPoint;
        }
    }
}
