using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour {

    [Header("Game over screen")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text scoreTexGO;
    [SerializeField] private TMP_Text highScoreTextGO;

    [Space(2)]
    [Header("Best score screen")]
    [SerializeField] private GameObject highScoreScreen;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private ParticleSystem leftParticleSystem;
    [SerializeField] private ParticleSystem rightParticleSystem;

    private void Start() {
        GameManager.Instance.OnGameOver += GameOver;
    }

    public void GameOver(object sender, GameManager.OnGameOverEventArgs e) {
        gameObject.SetActive(true);

        if(e.isHighScore) {
            gameOverScreen.SetActive(false);
            highScoreScreen.SetActive(true);
            leftParticleSystem.Play();
            rightParticleSystem.Play();
            highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        } else {
            highScoreScreen.SetActive(false);
            gameOverScreen.SetActive(true);
            scoreTexGO.text = e.currentScore.ToString();
            highScoreTextGO.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackButton() {
        // 0 main menu scene index
        SceneManager.LoadScene(0);
    }
}