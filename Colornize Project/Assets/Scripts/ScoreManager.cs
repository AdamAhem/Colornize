using UnityEngine;
using TMPro;

public class ScoreManager : Singleton {

    [SerializeField] private CellOccupiedStateSO emptyStateSO;
    [SerializeField] private CellOccupiedStateSO p1StateSO;
    [SerializeField] private CellOccupiedStateSO p2StateSO;

    [SerializeField] private UserInput userInput;
    [SerializeField] private TMP_Text scoreTextField;

    public void Start() {
        userInput.OnScoreChange += UserInput_OnScoreChange;
    }

    private void UserInput_OnScoreChange(object sender, UserInput.ScoreChangeEventArgs e) {

        int remainingEmpty = e.scoreDict[emptyStateSO];
        int p1score = e.scoreDict[p1StateSO];
        int p2score = e.scoreDict[p2StateSO];
        int totalCell = p1score + p2score + remainingEmpty;

        int p1ToWin = Mathf.FloorToInt((totalCell  + 1)/ 2) - p1score;
        int p2ToWin = Mathf.FloorToInt((totalCell  + 1)/ 2) - p2score;

        string text = $"Score: {p1score} : {p2score} \nP1 need: {p1ToWin} \nP2 need: {p2ToWin} \nRemaining: {remainingEmpty}";
        if (p1ToWin <= 0) {
            text += "\nP1 won";
        } else if (p2ToWin <= 0) {
            text += "\nP2 won";
        }

        scoreTextField.text = text;
    }
}
