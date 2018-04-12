using UnityEngine;
using UnityEngine.UI;

public class ManagerState : Singleton<ManagerState>
{
    [SerializeField] private int score;
    [SerializeField] private Text uiTextScore;
    
    public void AddPoints(int points)
    {
        this.score += points;
        this.uiTextScore.text = this.score.ToString();
    }
}
