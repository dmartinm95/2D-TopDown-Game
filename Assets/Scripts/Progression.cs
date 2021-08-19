using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Progression : MonoBehaviour
{
    public static int Score;
    public static float Growth;
    public static bool IsGrowing;

    public static Progression instance;

    public Level[] levels;

    public GameObject levelUpEffect;
     
    public Player player;
    public PlayerShooting playerShooting;
    public EnemySpawner enemySpawner;

    public TextMeshProUGUI scoreText;
    public Slider scoreSlider;

    public AudioClip levelUpSound;

    public AudioSource backgroundSong;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        Score = 0;
        scoreText.text = Score.ToString();

        Growth = 1f;
        IsGrowing = false;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        scoreSlider.interactable = false;

    }

    private void Start() {
        StartCoroutine(StartTheGame());         
        enemySpawner.currentWave = levels[0].wave;
        levels[0].isUnlocked = true;
        scoreSlider.maxValue = levels[1].unlockScore;       
        
    }

    private void Update() {
        if (IsGrowing) {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
		    foreach (GameObject bullet in bullets)
		    {
                Debug.Log("Got one!");
			    bullet.GetComponent<Bullet>().Remove();
		    }
        }

        if (PauseMenu.GameIsPaused) {
            backgroundSong.Pause();
        }
        else {
            backgroundSong.UnPause();
        }
    }

    public void AddScore (int amount) {
        Score += amount;

        scoreText.text = Score.ToString();
        scoreSlider.value = Score;

        // Instead of ending the game we can go back to the first level but keep the growth increasing

        for (int i = 0; i < levels.Length; i++) {
            if (!levels[i].isUnlocked && Score >= levels[i].unlockScore) {
                if (levels[i].endTheGame) {
                    // Last Level
                    StartCoroutine(EndTheGame());
                }
                else {
                    Debug.Log(PauseMenu.GameIsPaused);
                    UnlockReward(levels[i].reward);
                }

                enemySpawner.currentWave = levels[i].wave;

                if (i < levels.Length - 1) {
                    scoreSlider.minValue = Score;
                    scoreSlider.maxValue = levels[i+1].unlockScore;
                }

                levels[i].isUnlocked = true;
            }
        }

    }

    IEnumerator StartTheGame () {

        float t = 5f;
        while (5 > 0f) {
            t -= Time.fixedDeltaTime * 1f;
            yield return null;
        }

    }

    IEnumerator EndTheGame () {
        IsGrowing = true;

        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
 
        float t = 3f;

        while (t > 0f) {
            if (!PauseMenu.GameIsPaused) {
                t -= Time.fixedDeltaTime * 1f;
            }
            yield return 0;
        }
        //yield return new WaitForSecondsRealtime(5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator LevelUp () {
        IsGrowing = true;

        GetComponent<AudioSource>().PlayOneShot(levelUpSound, 1);

        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(0.1f);

        while (PauseMenu.GameIsPaused) {
            yield return 0;
        }

        float baseScale = Growth;
        float factor = 1.3f;

        float t = 0f;
        while (t < 1f) {
            float growth = Mathf.Lerp(1f, factor, t);
            Growth = baseScale * growth;
            t += Time.fixedDeltaTime * 1f;
            yield return 0;
        }

        Growth = baseScale * factor;

        Debug.Log("Growth = " + Growth);

        while (PauseMenu.GameIsPaused) {
            yield return 0;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        IsGrowing = false;
    }


    void UnlockReward (RewardTier reward) {
        Debug.Log ("LEVEL UP!");

        player.health += reward.healthBonus;
        playerShooting.currentWeapon = reward.weapon;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<Enemy>().Remove();
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
		foreach (GameObject bullet in bullets)
		{
			bullet.GetComponent<Bullet>().Remove();
		}

        StartCoroutine(LevelUp());
    }
}
