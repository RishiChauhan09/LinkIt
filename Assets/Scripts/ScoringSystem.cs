using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour {
    [Header("Scores Text")]
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text currentHighScoreText;
    [SerializeField] private TMP_Text scoreAdded;

    [Space(2)]
    [Header("Filling Bars Variables")]
    [SerializeField] private float totalTimer = 30f;
    [SerializeField] private Image timerFillBar;
    private UnityEngine.UI.Extensions.Gradient timerFillGradient;
    [SerializeField] private Image comboFillBar;
    private float currentTimer;

    [Space(2)]
    [Header("Other Variables")]
    private int currentCombo;
    private bool isComboActive;
    private float comboTimer;
    [SerializeField] private GameObject highScoreText;
    [SerializeField] private ParticleSystem confettiParticleSystem;
    [SerializeField] private float comboTotalTime;
    [SerializeField] private float cameraShakeIntensity = 1f;
    [SerializeField] private float uiShakeMultiplier = 5f;
    [SerializeField] private float comboShakeScreenTime = .5f;

    private Vector3 startingScoreAddPos;

    private int displayScoreValue;
    private bool isGameOver;

    [Space(2)]
    [Header("Scripts")]
    [SerializeField] private ComboUI comboUI;

    private long currentScoreValue;

    private bool haveBeatenHighScore = false;

    private void Awake() {
        isGameOver = false;
        timerFillGradient = timerFillBar.GetComponent<UnityEngine.UI.Extensions.Gradient>();

        currentScoreText.text = "0";
        currentHighScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        currentCombo = 0;
        isComboActive = false;
        currentTimer = totalTimer;
        haveBeatenHighScore = false;
        startingScoreAddPos = scoreAdded.GetComponent<RectTransform>().anchoredPosition;
    }

    private void Start() {
        highScoreText.SetActive(false);
        GameManager.Instance.board.OnTilesDestroyed += (object sender, Board.OnTilesDestroyedEventArgs e) => {
            CalculateScore(e.numberOfTilesDestroyed);
        };
    }

    private void Update() {
        if(Time.timeScale == 0)
            return;

        if(isGameOver)
            return;

        if(isComboActive) {
            comboTimer -= Time.deltaTime;
            comboFillBar.fillAmount = comboTimer / comboTotalTime;
            if(comboTimer <= 0f) {
                isComboActive = false;
                currentCombo = 0;
                comboFillBar.fillAmount = 0f;
            }
        }

        // if game is not started then just return don't start countdown
        if(!GameManager.Instance.isGameStarted)
            return;

        currentTimer -= Time.deltaTime;
        timerFillBar.fillAmount = currentTimer / totalTimer;
        UpdateBarColor();
        if(currentTimer <= 0f) {
            isGameOver = true;
            GameManager.Instance.InvokeGameOver(haveBeatenHighScore, currentScoreValue);
        }
    }

    public void CalculateScore(int numberTilesDestroyed) {
        if(numberTilesDestroyed > 4) {
            float timerToChange = currentTimer + (numberTilesDestroyed - 4);
            currentTimer = Mathf.Min(timerToChange, totalTimer);
            timerFillBar.fillAmount = currentTimer / totalTimer;
        }

        int score = numberTilesDestroyed * (currentCombo == 0 ? 1 : currentCombo);
        currentScoreValue += score;
        ShowScoreAdded(score);
        ChangeScoreText(currentScoreText, (int)currentScoreValue, score);

        UpdateCombo();

        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if(currentHighScore == 0)
            haveBeatenHighScore = true;

        if(currentScoreValue > currentHighScore) {
            if(haveBeatenHighScore) {
                ChangeScoreText(currentHighScoreText, (int)currentScoreValue, score);
                PlayerPrefs.SetInt("HighScore", (int)currentScoreValue);
            } else {
                ChangeScoreText(currentHighScoreText, (int)currentScoreValue, score);
                PlayerPrefs.SetInt("HighScore", (int)currentScoreValue);
                haveBeatenHighScore = true;

                confettiParticleSystem.Play();
                highScoreText.SetActive(true);
                highScoreText.GetComponent<CanvasGroup>().alpha = 0;
                highScoreText.GetComponent<CanvasGroup>().DOFade(1f, .25f).OnComplete(() => {
                    Time.timeScale = 0f;
                    StartCoroutine(NewHighScoreCoroutine());
                }).SetUpdate(true);
            }
        }
    }

    IEnumerator NewHighScoreCoroutine() {
        yield return new WaitForSecondsRealtime(1f);
        highScoreText.GetComponent<CanvasGroup>().DOFade(0f, .25f).OnComplete(() => {
            Time.timeScale = 1f;
            highScoreText.SetActive(false);
        }).SetUpdate(true);
    }

    private void UpdateCombo() {
        currentCombo++;
        isComboActive = true;
        comboTimer = comboTotalTime;
        comboFillBar.fillAmount = 1f;

        if(currentCombo > 5) {
            CameraController.Instance.ShakeCamera(cameraShakeIntensity, comboShakeScreenTime);
            GameManager.Instance.uiManager.ShakeUI(cameraShakeIntensity * uiShakeMultiplier, comboShakeScreenTime);
        }
        if(currentCombo > 1) {
            comboUI.ComboTextAnimation(currentCombo);
        }

        AudioManager.Instance.PlayAudio("combo", currentCombo);
    }

    private void ChangeScoreText(TMP_Text tmpText, int valueToAim, int scoreAdded) {
        displayScoreValue = valueToAim - scoreAdded;

        DOTween.Kill(tmpText);

        DOTween.To(
            () => displayScoreValue,
            x => {
                displayScoreValue = x;
                tmpText.text = displayScoreValue.ToString();
            },
            valueToAim,
            .5f
            )
            .SetEase(Ease.OutCubic)
            .SetTarget(tmpText)
            .SetUpdate(true);
    }

    private void ShowScoreAdded(int score) {
        scoreAdded.DOKill();

        RectTransform rect = scoreAdded.GetComponent<RectTransform>();
        rect.anchoredPosition = startingScoreAddPos;
        float moveDistance = 100f;
        scoreAdded.GetComponent<CanvasGroup>().alpha = 1;
        scoreAdded.text = "+" + score;
        scoreAdded.gameObject.SetActive(true);

        rect.DOAnchorPosY(startingScoreAddPos.y + moveDistance, .5f).SetUpdate(true);
        scoreAdded.GetComponent<CanvasGroup>().DOFade(0f, .5f).SetEase(Ease.InCirc).SetUpdate(true)
            .OnComplete(() => {
                scoreAdded.gameObject.SetActive(false);
            });

        rect.anchoredPosition = startingScoreAddPos;
    }

    private void UpdateBarColor() {
        if(timerFillBar.fillAmount > .5f)
            timerFillGradient.DoGradient(GameManager.Instance.allAssets.fullEnergy, .2f).SetUpdate(true);
        else if(timerFillBar.fillAmount > .25f)
            timerFillGradient.DoGradient(GameManager.Instance.allAssets.midEnergy, .2f).SetUpdate(true);
        else if(timerFillBar.fillAmount > .1f)
            timerFillGradient.DoGradient(GameManager.Instance.allAssets.lowEnergy, .2f).SetUpdate(true);
        else
            timerFillGradient.DoGradient(GameManager.Instance.allAssets.criticalEnergy, .2f).SetUpdate(true);
    }

}