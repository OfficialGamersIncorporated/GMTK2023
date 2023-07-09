using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour {

    public int closest = 55;
    public int furthest = 10;
    public float CameraMoveSpeed = 5;
    private PixelPerfectCamera pixelCamera;
    private Camera mainCamera;
    EventSystem eventSystem;

    void Start() {
        pixelCamera = GetComponent<PixelPerfectCamera>();
        mainCamera = Camera.main;
        eventSystem = EventSystem.current;
    }
    void LateUpdate() {
        // camera lateral movement
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * CameraMoveSpeed * Time.deltaTime;

        //camera scroll wheel movement
        int mouseInput = (int)(Input.GetAxis("Mouse ScrollWheel") * CameraMoveSpeed);
        if(pixelCamera.assetsPPU + mouseInput > closest || pixelCamera.assetsPPU + mouseInput < furthest || eventSystem.IsPointerOverGameObject()) {
            return;
        }
        pixelCamera.assetsPPU += mouseInput * 2;
        pixelCamera.assetsPPU = (int)Mathf.Round(pixelCamera.assetsPPU / 2) * 2; // round to the nearest multiple of 2.

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.

        // when you are zooming, go a little towards the players mouse
        Vector3 direction = mouseWorldPos - transform.position;
        direction.Scale(new Vector3(1, 1, 0)); // remove the Z component so this will just have the X and Y position of the mouse in the world.

        if(mouseInput > 0) {
            transform.position += direction * .1f;
            
        } else if(mouseInput < 0) {
            transform.position -= direction * .1f;
        }

    }
}
