using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioClip sound;
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    private void Start() {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
        source.volume = 0.5f;
    }

    public void PlayGame () {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame () {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void PlaySound () {
        source.PlayOneShot(sound);
    }
}
