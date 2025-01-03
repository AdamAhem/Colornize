using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputSetNumberOfGrid : MonoBehaviour {

    [SerializeField] private UserInput userInput;
    [SerializeField] private TMP_InputField inputField;
    private Slider slider;

    public void Start() {
        slider = GetComponent<Slider>();
    }

    public void UpdateNumberOfGrid() {
        userInput.SetNumberOfGrid((int)slider.value);
        inputField.text = slider.value.ToString();
    }

}