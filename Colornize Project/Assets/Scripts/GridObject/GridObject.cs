using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridObject : MonoBehaviour {

    // parent class for grid objects eg piece, teleport gate

    [SerializeField] private CellOccupiedStateSO cellOccupiedStateSO; // state grid object turns cell into

    [SerializeField] private GameObject objectSprite;
    [SerializeField] private GameObject objectText;
    [SerializeField] private bool hasSprite;

    [SerializeField] private string objectUIName; // use for UI

    private string objectEditorName; // unity game object name, use for debug


    public void Start() {
        objectSprite.SetActive(hasSprite);
        objectText.SetActive(!hasSprite);
    }

    public CellOccupiedStateSO GetCellStateSO() {
        return cellOccupiedStateSO;
    }

    public void SetCellStateSO(CellOccupiedStateSO cellStateSO) {
        this.cellOccupiedStateSO = cellStateSO;
    }

    public string GetDebugString() {
        return gameObject.name;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public string GetObjectEditorName() {
        return objectEditorName;
    }

    public void SetObjectEditorName(string name) {
        gameObject.name = $"GridObject {{Name: {name}, Owner: {cellOccupiedStateSO.stateName}}}";
    }

    public string GetObjectText() {
        return objectText.GetComponent<TextMeshPro>().text;
    }

    public void SetObjectText(string text) {
        objectText.GetComponent<TextMeshPro>().SetText(text);
    }

    public bool HasSprite() {
        return hasSprite;
    }

}
