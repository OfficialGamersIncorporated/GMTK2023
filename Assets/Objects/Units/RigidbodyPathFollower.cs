using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPathFollower : MonoBehaviour {

    GoonBehavior Behavior;
    new Rigidbody2D rigidbody;

    Room targetLastRoom;
    List<Vector3> CurrentPath;
    int CurrentPathWaypointIndex = 0;

    float PathCreatedTick = -100;

    float PathMaxAge = 1;
    public float WaypointRegistrationDistance = .5f;

    private void Start() {
        Behavior = GetComponent<GoonBehavior>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (Behavior.target) {
            Room targetCurrentRoom = RoomPathfinder2D.GetClosestRoom(Behavior.target.transform.position);
            Room currentRoom = RoomPathfinder2D.GetClosestRoom(transform.position);
            if (currentRoom == targetCurrentRoom) {
                CurrentPath = null;
                return;
            }

            if(CurrentPath == null || targetCurrentRoom != targetLastRoom) { //|| PathCreatedTick + PathMaxAge > Time.time) {
                PathCreatedTick = Time.time;
                targetLastRoom = targetCurrentRoom;
                CurrentPath = RoomPathfinder2D.Pathfind(transform.position, Behavior.target.transform.position);
                CurrentPathWaypointIndex = 0;
                print("FOUND PATH!");
                RoomPathfinder2D.DebugDrawPath(transform.position, CurrentPath);
            }
        }

        if(CurrentPath != null) {


            Vector3 currentWaypoint = CurrentPath[CurrentPathWaypointIndex];
            Vector3 toWaypoint = (currentWaypoint - transform.position);
            rigidbody.velocity = toWaypoint.normalized * Behavior.moveSpeed;

            if(toWaypoint.magnitude <= WaypointRegistrationDistance) {
                print("Reached index #" + CurrentPathWaypointIndex.ToString());
                CurrentPathWaypointIndex++;
                print("next index is #" + CurrentPathWaypointIndex);
                if(CurrentPath.Count < CurrentPathWaypointIndex - 1)
                    CurrentPath = null;
            }

        }
    }

}
