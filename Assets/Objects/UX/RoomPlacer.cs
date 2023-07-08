using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacer : MonoBehaviour {

    public Room RoomToPlace;
    public Vector2 RoomGridSize = new Vector2(12, 8);
    Camera cam;

    Vector3 RoundV3To(Vector3 vector, Vector3 roundTo) {
        vector = new Vector3(vector.x / roundTo.x, vector.y / roundTo.y, vector.z / roundTo.z);
        Vector3 rounded = new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        rounded.Scale(roundTo);
        return rounded;
    }
    void TryPlaceRoom(Vector3 snappedHoverPoint) {
        Collider2D[] overlappingColliders = Physics2D.OverlapPointAll(snappedHoverPoint, LayerMask.GetMask("Room"));
        foreach(Collider2D overlappingCollider in overlappingColliders) {
            if(overlappingCollider.CompareTag("Room")) return;
        }

        Room newRoom = Instantiate<Room>(RoomToPlace, snappedHoverPoint, Quaternion.identity, MainGrid.Singleton.transform);
        newRoom.RefereshEnterances();
    }
    void Start() {
        cam = GetComponent<Camera>();
    }
    void Update() {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.
        Vector3 SnappedHoverPoint = RoundV3To(mouseWorldPos, new Vector3(RoomGridSize.x, RoomGridSize.y, 1) ); // a position snapped to the the grid rooms are placed on.

        if(Input.GetButtonDown("Fire1")) {
            TryPlaceRoom(SnappedHoverPoint);
        }
    }
}
