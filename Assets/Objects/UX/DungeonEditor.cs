using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DungeonEditor : MonoBehaviour {
    public enum EditorPlacementType { None, Room, Unit }

    [SerializeField]
    EditorPlacementType CurrentPlacementMode = EditorPlacementType.None;

    // room placement
    public Room RoomToPlace;
    public Vector2 RoomGridSize = new Vector2(12, 8);

    // unit placement
    public Transform UnitToPlace;

    public Color HoverColorInvalid = Color.red;
    public Color HoverColorValid = Color.green;
    Transform selectionHighlight;
    Camera cam;
    EventSystem eventSystem;


    public void SetPlacementMode(int index) {
        CurrentPlacementMode = (EditorPlacementType)index;

        if (selectionHighlight != null)
            Destroy(selectionHighlight.gameObject);
        if(CurrentPlacementMode == EditorPlacementType.Room) {
            selectionHighlight = Instantiate<Room>(RoomToPlace).transform;
            ColorRoom(selectionHighlight, HoverColorInvalid);
            ClearColliders(selectionHighlight);
            DeltaRoomOrderInLayer(selectionHighlight, 1);
            selectionHighlight.parent = MainGrid.Singleton.transform;
        } else if (CurrentPlacementMode == EditorPlacementType.Unit) {
            selectionHighlight = Instantiate<Transform>(UnitToPlace);
            ClearColliders(selectionHighlight);
        }
    }
    void ColorRoom(Transform room, Color color) {
        foreach(Transform child in room.transform) {
            Tilemap tilemap = child.GetComponent<Tilemap>();
            if(tilemap != null)
                tilemap.color = color;
        }
    }
    void ColorUnit(Transform unit, Color color) {
        //  this code it temporary atm.
        MeshRenderer renderer = unit.GetComponent<MeshRenderer>();
        if(renderer != null)
            renderer.material.color = color;
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
    bool CheckUnitPlacementValid(Vector3 worldPos) {
        bool foundRoom = false;
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(worldPos, .5f);
        foreach(Collider2D overlappingCollider in overlappingColliders) {
            if(!overlappingCollider.isTrigger) return false;
            if(overlappingCollider.CompareTag("UnitPlacementZone")) foundRoom = true;
        }
        return foundRoom;
    }
    void TryPlaceUnit(Vector3 worldPos) {
        if(!CheckUnitPlacementValid(worldPos)) return;

        Transform newUnit = Instantiate<Transform>(UnitToPlace, worldPos, Quaternion.identity, MainUnitHolder.Singleton.Goons);
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
        eventSystem = EventSystem.current;

        cam = GetComponent<Camera>();
    }
    void Update() {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.

        if(CurrentPlacementMode == EditorPlacementType.Room) {

            Vector3 SnappedHoverPoint = RoundV3To(mouseWorldPos, new Vector3(RoomGridSize.x, RoomGridSize.y, 1)); // a position snapped to the the grid rooms are placed on.

            selectionHighlight.position = SnappedHoverPoint;
            if(CheckRoomPlacementValid(SnappedHoverPoint))
                ColorRoom(selectionHighlight, HoverColorValid);
            else
                ColorRoom(selectionHighlight, HoverColorInvalid);

            if(Input.GetButtonDown("Fire1") && !eventSystem.IsPointerOverGameObject()) {
                TryPlaceRoom(SnappedHoverPoint);
            }

        } else if (CurrentPlacementMode == EditorPlacementType.Unit) {
            selectionHighlight.position = mouseWorldPos;

            if(CheckUnitPlacementValid(mouseWorldPos))
                ColorUnit(selectionHighlight, HoverColorValid);
            else
                ColorUnit(selectionHighlight, HoverColorInvalid);

            if(Input.GetButtonDown("Fire1") && !eventSystem.IsPointerOverGameObject()) {
                TryPlaceUnit(mouseWorldPos);
            }
        }
    }
}
