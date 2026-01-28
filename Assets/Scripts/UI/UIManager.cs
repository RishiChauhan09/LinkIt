using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] private SettingsPanel settingsPanel;
    [SerializeField] private GameObject inGameUI;

    public void ShowSettingsScreen() {
        Time.timeScale = 0f;
        settingsPanel.gameObject.SetActive(true);
        AudioManager.Instance.ChangeMusicLowpassFrequency(true);
    }

    public void ShakeUI(float strength, float duration) {
        RectTransform rect = inGameUI.GetComponent<RectTransform>();

        Vector2 startPos = rect.anchoredPosition;

        rect.DOKill();
        rect.DOShakeAnchorPos(duration, strength)
            .SetUpdate(true)
            .OnComplete(() => {
                rect.anchoredPosition = startPos;
            });
    }

}