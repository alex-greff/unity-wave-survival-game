using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    private static SoundManager instance;

    private AudioSource audioSource;

    public static SoundManager Instance {
        get {
            return instance;
        }
    }

    void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

	public void PlayAudio (AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }

    public void PlayMusic (AudioClip music) {
        audioSource.clip = music;
        audioSource.Play();
    }
}
