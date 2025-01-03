using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectTeleportGate : MonoBehaviour {

    [SerializeField] private TeleportGate gatePrefab;

    [SerializeField] private TMP_Text gateText;
    private Image image;



    public void Start() {
        Image image = GetComponent<Image>();
        image.color = gatePrefab.GetComponent<GridObject>().GetCellStateSO().stateColor;
    }

    public TeleportGate GetGatePrefab() {
        gatePrefab.GetComponent<GridObject>().SetObjectText($"G{gatePrefab.GetGateGroup()}");
        gatePrefab.GetComponent<GridObject>().SetObjectEditorName($"Gate {gatePrefab.GetGateGroup()}");
        return gatePrefab;
    }

    public void UpdateGateSelectUI(int gateGroup, string text) {
        gateText.text = $"G{text}";
        gatePrefab.SetGateGroup(gateGroup);
    }



}
