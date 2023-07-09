using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPlacer : MonoBehaviour {

    public Room RoomToPlace;
    public Vector2 RoomGridSize = new Vector2(12, 8);
    public Color HoverColorInvalid = Color.red;
    public Color HoverColorValid = Color.green;
    Camera cam;

    Transform selectionHighlight;

    void ColorRoom(Transform room, Color color) {
        foreach(Transform child in room.transform) {
            Tilemap tilemap = child.GetComponent<Tilemap>();
            if(tilemap != null)
                tilemap.color = color;
        }
    }
    void ClearColliders(Transform room) {
        Collider2D mainCollider = room.GetComponent<Collider2D>();
        if(mainCollider)
            Destroy(mainCollider);

        foreach(Transform child in room.transform) {
            Collider2D collider = child.GetComponent<Collider2D>();
            if(collider)
                Destroy(collider);
        }
    }
    void DeltaRoomOrderInLayer(Transform room, int orderInLayerDelta) {
        foreach(Transform child in room.transform) {
            TilemapRenderer tilemap = child.GetComponent<TilemapRenderer>();
            if(tilemap != null)
                tilemap.sortingOrder += orderInLayerDelta;
        }
    }
    Vector3 RoundV3To(Vector3 vector, Vector3 roundTo) {
        vector = new Vector3(vector.x / roundTo.x, vector.y / roundTo.y, vector.z / roundTo.z);
        Vector3 rounded = new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        rounded.Scale(roundTo);
        return rounded;
    }
    bool CheckRoomPlacementValid(Vector3 snappedHoverPoint) {
        Collider2D[] overlappingColliders = Physics2D.OverlapPointAll(snappedHoverPoint, LayerMask.GetMask("Room"));
        foreach(Collider2D overlappingCollider in overlappingColliders) {
            if(overlappingCollider.CompareTag("Room")) return false;
        }
        return true;
    }
    void TryPlaceRoom(Vector3 snappedHoverPoint) {
        if(!CheckRoomPlacementValid(snappedHoverPoint)) return;

        Room newRoom = Instantiate<Room>(RoomToPlace, snappedHoverPoint, Quaternion.identity, MainGrid.Singleton.transform);
        newRoom.RefereshEnterances();
    }
    void Start() {
        cam = GetComponent<Camera>();

        selectionHighlight = Instantiate<Room>(RoomToPlace).transform;
        ColorRoom(selectionHighlight, HoverColorInvalid);
        ClearColliders(selectionHighlight);
        DeltaRoomOrderInLayer(selectionHighlight, 1);
        selectionHighlight.parent = MainGrid.Singleton.transform;
    }
    void Update() {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.
        Vector3 SnappedHoverPoint = RoundV3To(mouseWorldPos, new Vector3(RoomGridSize.x, RoomGridSize.y, 1) ); // a position snapped to the the grid rooms are placed on.

        selectionHighlight.position = SnappedHoverPoint;
        if(CheckRoomPlacementValid(SnappedHoverPoint))
            ColorRoom(selectionHighlight, HoverColorValid);
        else
            ColorRoom(selectionHighlight, HoverColorInvalid);

        if(Input.GetButtonDown("Fire1")) {
            TryPlaceRoom(SnappedHoverPoint);
        }
    }
}
