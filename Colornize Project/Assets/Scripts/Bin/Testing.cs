using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    [SerializeField] private GameObject testObject;
    [SerializeField] private GameObject testParent1;
    [SerializeField] private GameObject testParent2;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            testObject.transform.SetParent(testParent2.transform);
        }
    }
}