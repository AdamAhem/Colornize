using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownUIPieceSelect : MonoBehaviour {

    [SerializeField] private TMP_Text pieceText;
    [SerializeField] private ButtonSelectPiece buttonSelectPiece;
    private TMP_Dropdown dropdownUI;

    private void Awake() {
        dropdownUI = GetComponent<TMP_Dropdown>();
    }

    private void Start() {
        for (char c = 'A'; c <= 'Z'; c++) {
            dropdownUI.options.Add(new TMP_Dropdown.OptionData(c.ToString(), null));
        }
        ChangePieceSelectUI();
    }

    public void ChangePieceSelectUI() {
        int pickedEntryIndex = dropdownUI.value;
        string selectedOptionText = dropdownUI.options[pickedEntryIndex].text;
        buttonSelectPiece.UpdatePieceSelectUI(pickedEntryIndex, selectedOptionText);
    }

}