using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseButton;


    public static GameManager Instance;

    public GameObject BGMNotice;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2* 10f;

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 7, 15, 30, 50, 75, 100, 150, 200, 250 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;

    WaitForSecondsRealtime wait;

    void Awake()
    {
        Instance = this;
        wait = new WaitForSecondsRealtime(5);

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    }



    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);
        Resume();
        StartCoroutine(Bgmnotice());

        AudioManager.Instance.PlayBgm(true);
        AudioManager.Instance.PlaySfx(AudioManager.sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.3f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.Instance.PlayBgm(false);
        AudioManager.Instance.PlaySfx(AudioManager.sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.Instance.PlayBgm(false);
        AudioManager.Instance.PlaySfx(AudioManager.sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (isLive == false)
        {
            return;
        }
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void GetExp()
    {
        if (!isLive)
        {
            return;
        }

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }
    }

    IEnumerator Bgmnotice()
    {
        BGMNotice.SetActive(true);
        yield return wait;
        BGMNotice.SetActive(false);
    }

    public void SelectEffect()
    {
        AudioManager.Instance.PlaySfx(AudioManager.sfx.Select);
    }


}
