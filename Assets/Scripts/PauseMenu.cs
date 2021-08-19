using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject pauseButtonUI;

    private float originalTimeScale;
    private float originalFixedDeltaTime;

    public AudioClip sound;
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    private void Start() {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
        source.volume = 0.5f;
    }

    public void Resume () {
        PlaySound();
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
        pauseButtonUI.SetActive(true);
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;       
    }

    void PlaySound () {
        source.PlayOneShot(sound);
    }

    void Pause () {
        GameIsPaused = true;
        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;
        pauseMenuUI.SetActive(true);
        pauseButtonUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PauseWithButton () {
        PlaySound();
        GameIsPaused = !GameIsPaused;
        if (GameIsPaused) {
            Pause();
        }
        else {
            Resume();
        }
    }

    public void LoadMenu () {
        PlaySound();
        Time.timeScale = 1f;
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
        pauseButtonUI.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame () {
        PlaySound();
        Debug.Log("Quitting Game ...");
        Application.Quit();
    }
}
