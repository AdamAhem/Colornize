using TMPro;
using UnityEngine;

public class GridCellVisual : MonoBehaviour {

    [SerializeField] private GridCell gridCell;
    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer highlightSprite;
    public void Awake() {
        gridCell.OnCellStateChange += GridCell_OnCellStateChange;
        gridCell.OnChangeHighlight += GridCell_OnChangeHighlight;
    }

    private void Start() {
        highlightSprite.gameObject.SetActive(false);
    }

    private void GridCell_OnChangeHighlight(bool trigger) {
        highlightSprite.color = gridCell.GetOccupiedState().highlightColor;
        highlightSprite.gameObject.SetActive(trigger);
    }

    private void GridCell_OnCellStateChange(object sender, System.EventArgs e) {
        sprite.GetComponent<SpriteRenderer>().color = gridCell.GetOccupiedState().stateColor;
    }
}
