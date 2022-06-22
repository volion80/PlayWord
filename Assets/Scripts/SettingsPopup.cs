using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] private AudioClip defaultSound;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider soundVolume;

    void Start()
    {
        musicVolume.value = PlayerPrefs.GetFloat("music_volume", 1);
        soundVolume.value = PlayerPrefs.GetFloat("sound_volume", 1);
    }
    
    public void OnSoundToggle()
    {
        Managers.Audio.soundMute = !Managers.Audio.soundMute;
        Managers.Audio.PlaySound(defaultSound);
    }

    public void OnSoundValue(float volume)
    {
        Managers.Audio.soundVolume = volume;
        
        PlayerPrefs.SetFloat("sound_volume", volume);
    }

    public void OnMusicToggle()
    {
        Managers.Audio.musicMute = !Managers.Audio.musicMute;
        Managers.Audio.PlaySound(defaultSound);
    }

    public void OnMusicValue(float volume)
    {
        Managers.Audio.musicVolume = volume;
        
        PlayerPrefs.SetFloat("music_volume", volume);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnPlayMusic(int selector)
    {
        Managers.Audio.PlaySound(defaultSound);

        switch (selector)
        {
            case 1:
                Managers.Audio.PlayIntroMusic();
                break;
            case 2:
                Managers.Audio.PlayLevelMusic();
                break;
            default:
                Managers.Audio.StopMusic();
                break;
        }
    }
}
