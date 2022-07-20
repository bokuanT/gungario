using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioEmitter MusicPlayer;

    private AudioSource audioSource;

    private bool isStopped = false;

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
        if (!audioSource.isPlaying && !isStopped)
        {
            MusicPlayer.PlayOneShot();
        }
    }

    public void StopSound()
    {
        MusicPlayer.Stop();
        isStopped = true;
    }

    public void StartSound()
    {
        MusicPlayer.PlayOneShot();
        isStopped = false;
    }

}
