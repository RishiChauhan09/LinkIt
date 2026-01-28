using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;

    [SerializeField] private float defaultAudioVolume = .6f;
    [SerializeField] private List<AudioClipDetails> sfxSounds;
    [SerializeField] private List<AudioClipDetails> musicSounds;

    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [SerializeField] private AudioMixer musicMixer;

    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;


    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += SceneChanged;
    }

    private void SceneChanged(Scene scene, LoadSceneMode arg1) {
        if(scene.buildIndex == 0)
            musicAudioSource.Stop();
        else
            musicAudioSource.Play();
    }


    public void PlayAudio(string name) {
        foreach(AudioClipDetails clipDetails in sfxSounds) {
            if(clipDetails.clipName == name) {
                sfxAudioSource.volume = defaultAudioVolume;
                sfxAudioSource.PlayOneShot(clipDetails.clip);
                return;
            }
        }
    }

    public void PlayAudio(string name, int comboNumber) {
        foreach(AudioClipDetails clipDetails in sfxSounds) {
            if(clipDetails.clipName == name) {

                int clampedCombo = Mathf.Clamp(comboNumber, 1, 7);

                float t = (clampedCombo - 1) / 6f; 

                float volume = Mathf.Lerp(defaultAudioVolume, 1f, t);
                float pitch = Mathf.Lerp(1f, 1.25f, t);

                sfxAudioSource.volume = volume;
                sfxAudioSource.pitch = pitch;

                sfxAudioSource.PlayOneShot(clipDetails.clip);
                return;
            }
        }
    }

    public void PlayMusic(string name) {
        foreach(AudioClipDetails clipDetails in musicSounds) {
            if(clipDetails.clipName == name) {
                musicAudioSource.volume = defaultAudioVolume;
                musicAudioSource.clip = clipDetails.clip;
                musicAudioSource.Play();
                return;
            }
        }
    }

    public bool ToggleSFX(Image imageComponent) {
        if(sfxAudioSource.mute) {
            sfxAudioSource.mute = false;
            imageComponent.sprite = sfxOnSprite;
            return false;
        } else {
            sfxAudioSource.mute = true;
            imageComponent.sprite = sfxOffSprite;
            return true;
        }
    }

    public bool ToggleMusic(Image imageComponent) {
        if(musicAudioSource.mute) {
            musicAudioSource.mute = false;
            imageComponent.sprite = musicOnSprite;
            return false;
        } else {
            musicAudioSource.mute = true;
            imageComponent.sprite = musicOffSprite;
            return true;
        }
    }

    public void UpdateImage(Image imageComponent, SoundType soundType) {
        if(soundType == SoundType.music) {
            if(musicAudioSource.mute) 
                imageComponent.sprite = musicOffSprite;
            else 
                imageComponent.sprite = musicOnSprite;
        } else {
            if(sfxAudioSource.mute) 
                imageComponent.sprite = sfxOffSprite;
            else 
                imageComponent.sprite = sfxOnSprite;
        }
    }

    public void ChangeMusicLowpassFrequency(bool setLow) {
        if(setLow) {
            musicMixer.SetFloat("LowpassCutoff", 1000);
        } else {
            musicMixer.SetFloat("LowpassCutoff", 5000);
        }
    }

}

[Serializable]
public class AudioClipDetails {
    public string clipName;
    public AudioClip clip;
}

public enum SoundType {
    music, 
    sound
}