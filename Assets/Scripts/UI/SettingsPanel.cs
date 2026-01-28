using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {

    [Header("Sound Things")]
    [SerializeField] private Image musicImage;
    [SerializeField] private Button musicButton;
    [SerializeField] private Image sfxImage;
    [SerializeField] private Button sfxButton;

    private Color buttonColor;

    private void Awake() {
        buttonColor = musicButton.GetComponent<Image>().color;
        AudioManager.Instance.UpdateImage(musicImage, SoundType.music);
        AudioManager.Instance.UpdateImage(sfxImage, SoundType.sound);
    }

    private void Start() {
        musicButton.onClick.AddListener(MusicToggle);
        sfxButton.onClick.AddListener(SFXToggle);
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        AudioManager.Instance.ChangeMusicLowpassFrequency(false);
    }

    public void ReplayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        AudioManager.Instance.ChangeMusicLowpassFrequency(false);
    }

    public void HomeButton() {
        SceneManager.LoadScene("Main Menu");
    }

    public void MusicToggle() {
        if(AudioManager.Instance.ToggleMusic(musicImage)) {
            musicButton.GetComponent<Image>().color = Color.white;
        } else {
            musicButton.GetComponent<Image>().color = buttonColor;
        }
    }

    public void SFXToggle() {
        if(AudioManager.Instance.ToggleSFX(sfxImage)) {
            sfxButton.GetComponent<Image>().color = Color.white;
        } else {
            sfxButton.GetComponent<Image>().color = buttonColor;
        }
    }

}