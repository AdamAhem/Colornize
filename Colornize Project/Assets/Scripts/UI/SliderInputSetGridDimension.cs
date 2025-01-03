using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputSetGridDimension : MonoBehaviour {

    [SerializeField] private UserInput userInput;
    [SerializeField] private TMP_InputField inputField;
    private Slider slider;


    public void Start() {
        slider = GetComponent<Slider>();
    }

    public void UpdateGridDimension() {
        userInput.SetGridDimension((int)slider.value);
        inputField.text = slider.value.ToString();
    }

}