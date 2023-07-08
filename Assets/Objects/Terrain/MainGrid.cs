using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGrid : MonoBehaviour {

    static public MainGrid Singleton;

    public void Awake() {
        Singleton = this;
    }

}
