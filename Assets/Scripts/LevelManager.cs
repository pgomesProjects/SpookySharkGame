using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [SerializeField] private Image threatUI;
    [SerializeField] private Image threatUIIcon;
    [SerializeField] private Color[] threatColors;
    [SerializeField] private TextMeshProUGUI personalBestText;
    [SerializeField] private Slider oxygenBar;
    [SerializeField] private TextMeshProUGUI oxygenText;

    public TextMeshProUGUI scoreText;
    public bool isGameOver;

    private AudioManager audioManager;

    private readonly float maxThreatDistance = 600;
    private readonly float minThreatDistance = 10;
    private readonly float maxThreatIconScale = 1.7f;
    private readonly float minThreatIconScale = 0.5f;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        isGameOver = false;
        audioManager = FindObjectOfType<AudioManager>();
        if(audioManager != null)
        {
            GameData.playingSongName = "InGame";
            FindObjectOfType<AudioManager>().Play(GameData.playingSongName, PlayerPrefs.GetFloat("BGMVolume"));
            AudioManager.instance?.Play("WaterAmb", PlayerPrefs.GetFloat("SFXVolume"));
        }

        personalBestText.text = "Personal Best: " + PlayerPrefs.GetInt("HighScore");
    }
    private void Update()
    {
        float sharkDistInterpolant = 1;
        float sharkDist = Vector3.Distance(SharkController.main.transform.position, PlayerController.main.transform.position);
        sharkDistInterpolant = Mathf.Clamp(Mathf.InverseLerp(maxThreatDistance, minThreatDistance, sharkDist), 0, 1);
        threatUIIcon.transform.localScale = Vector3.one * Mathf.Lerp(minThreatIconScale, maxThreatIconScale, sharkDistInterpolant);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateOxygenBar(float oxygenPercentage)
    {
        //If the oxygen value is less than 0, simply display it as 0
        if (oxygenPercentage < 0)
            oxygenPercentage = 0;

        //Update slider and text
        oxygenBar.value = oxygenPercentage;
        oxygenText.text = "O2: " + ((int)oxygenPercentage).ToString() + "%";
    }

    public void UpdateThreatUI(int level)
    {
        Debug.Log("Threat UI: " + level);
        threatUI.color = threatColors[level];
        threatUIIcon.color = threatColors[level];

        //If the level is 0, make icon invisible
        if (level == 0)
            threatUIIcon.color = new Color(0, 0, 0, 0);
    }

    public void StartThreatMusic(int level)
    {
        if (audioManager != null)
        {
            Debug.Log("Starting Heartbeat" + level);
            for(int i = 0; i < (int)SharkController.ThreatLevel.NumberOfThreatLevels; i++)
            {
                if(i != level)
                {
                    //If there's another heartbeat SFX playing, stop it
                    FindObjectOfType<AudioManager>().Stop("Heartbeat" + i);
                }
                //Play the wanted heartbeat SFX
                else
                {
                    FindObjectOfType<AudioManager>().Play("Heartbeat" + level, PlayerPrefs.GetFloat("SFXVolume"));
                    
                    //If threatened, also play the threatened sting
                    if(level == (int)SharkController.ThreatLevel.THREATENED)
                        FindObjectOfType<AudioManager>().Play("ThreatenedSting", PlayerPrefs.GetFloat("SFXVolume"));
                }
            }
        }
    }

    public void StopThreatMusic(int level)
    {
        if (audioManager != null)
        {
            FindObjectOfType<AudioManager>().Stop("Heartbeat" + level);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        if (audioManager != null)
        {
            FindObjectOfType<AudioManager>().PauseAllSounds();
            FindObjectOfType<AudioManager>().Play("GameOverSFX", PlayerPrefs.GetFloat("SFXVolume"));
        }
        SceneManager.LoadScene("GameOver");
    }
}
