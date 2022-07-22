using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioEmitter MusicPlayer;

    private AudioSource audioSource;

    private bool isMenuMusic = true;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("musicVolume");
        AudioListener.volume = PlayerPrefs.GetFloat("generalVolume");

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Menu")
        {
            if (!audioSource.isPlaying)
            {
                MusicPlayer.PlayOneShot(1);
                isMenuMusic = true;
                return;
            }
            else
            {
                if (!isMenuMusic)
                {
                    MusicPlayer.Stop();
                    MusicPlayer.PlayOneShot(1);
                    isMenuMusic = true;
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        
        bool sceneIspresent = scene.name == "Game" || scene.name == "Deathmatch";
        if (sceneIspresent)
        {
            if (!audioSource.isPlaying)
            {
                MusicPlayer.PlayOneShot(0);
                isMenuMusic = false;
                return;
            }
            else
            {
                if (isMenuMusic)
                {
                    MusicPlayer.Stop();
                    MusicPlayer.PlayOneShot(0);
                    isMenuMusic = false;
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        Debug.Log("point 7");
    }

    public void StopSound()
    {
        MusicPlayer.Stop();
    }


}
