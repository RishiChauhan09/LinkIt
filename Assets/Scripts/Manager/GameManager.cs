using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public Board board;
    public ScoringSystem scoringSystem;
    public AllAssets allAssets;
    public UIManager uiManager;

    [HideInInspector] public bool isGameStarted = false;

    public event EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs : EventArgs {
        public bool isHighScore;
        public long currentScore;
    }   

    private void Awake() {
        Time.timeScale = 1f;
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        isGameStarted = false;
    }

    public void InvokeGameOver(bool isHighScore, long currentScore) {
        OnGameOver?.Invoke(this, new OnGameOverEventArgs { isHighScore = isHighScore,
                                                           currentScore = currentScore});
    }

}