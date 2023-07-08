using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float CameraMoveSpeed = 5;

    void Start() {

    }
    void LateUpdate() {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * CameraMoveSpeed * Time.deltaTime;
    }
}
