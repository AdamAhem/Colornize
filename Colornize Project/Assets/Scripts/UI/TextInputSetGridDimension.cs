using TMPro;
using UnityEngine;

public class TextInputSetGridDimension : MonoBehaviour {

    [SerializeField] private UserInput userInput;
    private TMP_InputField inputField;

    public void Start() {
        inputField = GetComponent<TMP_InputField>();
    }

    public void UpdateGridDimension() {
        if (inputField != null && int.TryParse(inputField.text, out int inputDimension)) {
            userInput.SetGridDimension(inputDimension);
        } else {
            Debug.Log("grid dimension entry error, enter int please");
        }
    }
}