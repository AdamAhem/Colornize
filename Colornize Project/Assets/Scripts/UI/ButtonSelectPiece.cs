using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonSelectPiece : MonoBehaviour {

    [SerializeField] private List<Piece> piecePrefabList; // List of piece prefabs corresponding to dropdown ui select (A-Z)
    [SerializeField] private CellOccupiedStateSO pieceOwnerCellStateSO;

    [SerializeField] private TMP_Text pieceText;

    private Piece returnPiecePrefab;

    public void Start() {
        Image colorImage = GetComponent<Image>();
        colorImage.color = pieceOwnerCellStateSO.stateColor;
    }

    public Piece GetPiecePrefab() {
        // set owner
        returnPiecePrefab.GetComponent<GridObject>().SetCellStateSO(pieceOwnerCellStateSO);
        // set name
        returnPiecePrefab.GetComponent<GridObject>().SetObjectEditorName($"Piece {pieceText.text}");
        returnPiecePrefab.GetComponent<GridObject>().SetObjectText(pieceText.text);
        return returnPiecePrefab;
    }

    public void UpdatePieceSelectUI(int pieceIndex, string text) {
        // update sprite and text according to index selected
        Image spriteImage = transform.GetChild(0).GetComponent<Image>();
        Piece selectedPiece = piecePrefabList[pieceIndex];
        returnPiecePrefab = piecePrefabList[pieceIndex];
        pieceText.text = text;
        if (selectedPiece.GetComponent<GridObject>().GetCellStateSO()) {
            spriteImage.sprite = piecePrefabList[pieceIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        spriteImage.gameObject.SetActive(selectedPiece.GetComponent<GridObject>().HasSprite());
        pieceText.gameObject.SetActive(!selectedPiece.GetComponent<GridObject>().HasSprite());
    }
}