using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Instance Variable
    public static GameManager manager = null;

    [Header("Game State Variables")]
    public bool playingGame = false;
    [SerializeField] private bool startGame = false;
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private float playerScoreMult = 1f;

    [Header("Game Time Variables")]
    public float timeElapised = 0f;
    public float timeAlloted = 60f;

    [SerializeField] private Animator sunAnimator;
    public float normalizedTime = 0f;

    [Header("Sound References")]
    [SerializeField] private AudioSource sfxPlayer = null;
    [SerializeField] private AudioSource musicPlayer = null;

    [SerializeField] private AudioClip startMusic = null;
    [SerializeField] private AudioClip music = null;
    [SerializeField] private AudioClip endWarningSFX = null;
    [SerializeField] private AudioClip endGameSFX = null;
    [SerializeField] private AudioClip mouseClick = null;

    public float introVolume;
    public float musicVolume;
    public float alarmVolume;
    public float tickingVolume;

    [Header("Start Game Menu Refereces")]
    [SerializeField] private GameObject startMenu = null;
    [SerializeField] private GameObject creditMenu = null;

    [Header("In-Game GUI References")]
    [SerializeField] private Transform canvas = null;
    [SerializeField] private GameObject inGameGUI = null;
    [SerializeField] private Image staminaBar = null;
    [SerializeField] private TextMeshProUGUI multText = null;
    [SerializeField] private RectTransform timerHand = null;

    [Header("End-Game GUI References")]
    [SerializeField] private GameObject endGameMenu = null;
    [SerializeField] private TextMeshProUGUI endGameScoreText = null;
    [SerializeField] private TextMeshProUGUI endGameHighScoreText = null;
    [SerializeField] private TextMeshProUGUI endGameNewHighScoreText = null;

    [Header("Effect References")]
    [SerializeField] private GameObject mult2x_Effect = null;
    [SerializeField] private GameObject mult3x_Effect = null;
    [SerializeField] private GameObject mult4x_Effect = null;

    private bool timeWarning = false;

    /// <summary>
    /// Setting Static Manager Reference
    /// </summary>
    private void Awake()
    {
        // Disabling the Start Menu
        startMenu.SetActive(true);

        // Enabling In-Game Menu
        inGameGUI.SetActive(false);

        // Setting the Global Static Reference
        if (manager == null)
            manager = this;
        else
            GameObject.Destroy(gameObject);

        // Start the Track
        musicPlayer.clip = startMusic;
        musicPlayer.volume = introVolume;
        musicPlayer.Play();
    }

    /// <summary>
    /// Calling Update Calls
    /// Checking for finish
    /// </summary>
    private void Update()
    {
        // Updating the Time
        if (playingGame)
        {
            // Updating the GUI
            UpdateGUI();

            timeElapised += Time.deltaTime;

            normalizedTime = timeElapised / timeAlloted;
            sunAnimator.SetFloat("normalizedTime", normalizedTime);

            if (timeElapised >= timeAlloted)
            {
                EndCurrentGame();
            } else if (timeElapised >= (timeAlloted - 10f) && !timeWarning)
            {
                sfxPlayer.clip = endWarningSFX;
                sfxPlayer.volume = tickingVolume;
                sfxPlayer.Play();

                timeWarning = true;
            }
        }
    }

    public void StartGame()
    {
        // Disabling the Start Menu
        startMenu.SetActive(false);

        // Enabling In-Game Menu
        inGameGUI.SetActive(true);

        // Setting Game Variables
        playingGame = true;

        // Start the Track
        musicPlayer.Stop();
        musicPlayer.clip = music;
        musicPlayer.volume = musicVolume;
        musicPlayer.Play();

        // Starting Game for Cat
        CatController.cat.StartGame();
    }

    public void ShowCreditScreen()
    {
        // Show Credit Screen
        creditMenu.SetActive(true);

        // Disable Start Screen
        startMenu.SetActive(false);
    }

    public void ExitCreditScreen()
    {
        // Disable Credit Screen
        creditMenu.SetActive(false);

        // Enable Start Screen
        startMenu.SetActive(true);
    }

    /// <summary>
    /// Updating the GUI
    /// </summary>
    private void UpdateGUI()
    {
        // Updating the Stamina Bar
        float fill = (CatController.cat.stamina / CatController.cat.staminaMax);
        staminaBar.fillAmount = fill;

        // Updating the Multiplier Text
        multText.text = "x" + playerScoreMult.ToString("F2");

        // Updating the Timer Hand
        timerHand.localRotation = Quaternion.Euler(0f, 0f, (timeElapised / timeAlloted) * -360f);
    }

    public void IncreaseScore(float inc)
    {
        if (playingGame)
            playerScore += (inc) * playerScoreMult;
    } 

    public void ResetMultiplier()
    {
        if (playingGame)
            playerScoreMult = 1f;
    }

    public void IncreaseMultiplier()
    {
        if (playingGame)
        {
            playerScoreMult += 0.25f;

            // Checking for MileStones
            if (playerScoreMult == 2f)
            {
                GameObject newEffect = Instantiate(mult2x_Effect);
                newEffect.transform.SetParent(canvas);
                newEffect.transform.localPosition = Vector3.zero;

            }

            // Checking for MileStones
            if (playerScoreMult == 3f)
            {
                GameObject newEffect = Instantiate(mult3x_Effect);
                newEffect.transform.SetParent(canvas);
                newEffect.transform.localPosition = Vector3.zero;
            }

            // Checking for MileStones
            if (playerScoreMult == 4f)
            {
                GameObject newEffect = Instantiate(mult4x_Effect);
                newEffect.transform.SetParent(canvas);
                newEffect.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void EndCurrentGame()
    {
        // Play End Game SFX
        sfxPlayer.clip = endGameSFX;
        sfxPlayer.volume = alarmVolume;
        sfxPlayer.Play();

        playingGame = false;

        // Displaying Results
        endGameMenu.SetActive(true);
        endGameScoreText.text = playerScore.ToString("F0");

        // Updating HighScore if needed
        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (highScore < playerScore)
        {
            PlayerPrefs.SetFloat("HighScore", playerScore);
            endGameNewHighScoreText.gameObject.SetActive(true);
            endGameNewHighScoreText.GetComponent<Animator>().SetBool("newScore", true);
        }

        // Setting the Text
        endGameHighScoreText.text = PlayerPrefs.GetFloat("HighScore", 0).ToString("F0");
    }

    public void RestartGame()
    {
        sfxPlayer.clip = mouseClick;
        sfxPlayer.Play();

        // Reloading the Current Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        sfxPlayer.clip = mouseClick;
        sfxPlayer.Play();

        Application.Quit();
    }
}
