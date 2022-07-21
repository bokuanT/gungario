using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Slider musicSlider;

    [SerializeField] private AudioSource music;


    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("generalVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("generalVolume", 1);
            Load();
        }
        else
        {
            Load();
        }

        if (music == null)
        {
            music = FindObjectOfType<AudioSource>();
        }
    }

    public void ChangeGeneralVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    public void ChangeMusicVolume()
    {
        music.volume = musicSlider.value;
        Save();
    }

    private void Load()
    {
        if (music == null)
        {
            music = FindObjectOfType<AudioSource>();
        }

        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        volumeSlider.value = PlayerPrefs.GetFloat("generalVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("generalVolume", volumeSlider.value);
    }

    private void Update()
    {
        Load();
    }
}
