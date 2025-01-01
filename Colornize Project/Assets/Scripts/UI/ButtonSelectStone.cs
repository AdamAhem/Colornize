using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectStone : MonoBehaviour {

    [SerializeField] private Stone stonePrefab;

    private Image image;

    public void Start() {
        Image image = GetComponent<Image>();
        image.color = stonePrefab.GetComponent<GridObject>().GetCellStateSO().stateColor;
    }

    public Stone GetGatePrefab() {
        return stonePrefab;
    }

}
