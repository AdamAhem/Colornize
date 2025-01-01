using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownUIGateSelect : MonoBehaviour {

    [SerializeField] private TMP_Text pieceText;
    [SerializeField] private ButtonSelectTeleportGate buttonSelectGate;
    private TMP_Dropdown dropdownUI;

    private void Awake() {
        dropdownUI = GetComponent<TMP_Dropdown>();
    }

    private void Start() {
        for (int i = 0; i <= 10; i++) {
            dropdownUI.options.Add(new TMP_Dropdown.OptionData(i.ToString(), null));
        }
        ChangeGateSelectUI();
    }

    public void ChangeGateSelectUI() {
        int pickedEntryIndex = dropdownUI.value;
        Debug.Log(pickedEntryIndex);
        string selectedOptionText = dropdownUI.options[pickedEntryIndex].text;
        buttonSelectGate.UpdateGateSelectUI(pickedEntryIndex, selectedOptionText);
    }


}