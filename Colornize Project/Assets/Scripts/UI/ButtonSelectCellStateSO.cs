using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectCellStateSO : MonoBehaviour {

    [SerializeField] private CellOccupiedStateSO cellStateSO;

    private Image image;

    private void Start() {
        image = gameObject.GetComponent<Image>();
        image.color = cellStateSO.stateColor;
    }

    public CellOccupiedStateSO GetCellStateSO() {
        return cellStateSO;
    }
}