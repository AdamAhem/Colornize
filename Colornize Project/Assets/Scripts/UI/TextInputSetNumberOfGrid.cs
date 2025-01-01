using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInputSetNumberOfGrid : MonoBehaviour {

    [SerializeField] private UserInput userInput;
    private TMP_InputField inputField;

    public void Start() {
        inputField = GetComponent<TMP_InputField>();
    }

    public void UpdateNumberOfGrid() {
        if (inputField != null && int.TryParse(inputField.text, out int inputNumberOfGrid)) {
            userInput.SetNumberOfGrid(inputNumberOfGrid);
        } else {
            Debug.Log("number of grid entry error, enter int please");
        }
    }
}
