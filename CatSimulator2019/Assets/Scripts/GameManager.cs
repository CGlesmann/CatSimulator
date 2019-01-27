using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Instance Variable
    public static GameManager manager = null;

    [Header("Game State Variables")]
    [SerializeField] private float playerScore = 0f;
    [SerializeField] private float playerScoreMult = 1f;

    [Header("Game Time Variables")]
    public float timeElapised = 0f;
    public float timeAlloted = 60f;

    [Header("GUI References")]
    [SerializeField] private Transform canvas = null;
    [SerializeField] private Image staminaBar = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI multText = null;
    [SerializeField] private RectTransform timerHand = null;

    [Header("Effect References")]
    [SerializeField] private GameObject mult2x_Effect = null;
    [SerializeField] private GameObject mult3x_Effect = null;
    [SerializeField] private GameObject mult4x_Effect = null;

    /// <summary>
    /// Setting Static Manager Reference
    /// </summary>
    private void Awake()
    {
        // Setting the Global Static Reference
        if (manager == null)
            manager = this;
        else
            GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Calling Update Calls
    /// Checking for finish
    /// </summary>
    private void Update()
    {
        // Updating the GUI
        UpdateGUI();

        // Updating the Time
        timeElapised += Time.deltaTime;
    }

    /// <summary>
    /// Updating the GUI
    /// </summary>
    private void UpdateGUI()
    {
        // Updating the Stamina Bar
        float fill = (CatController.cat.stamina / CatController.cat.staminaMax);
        staminaBar.fillAmount = fill;

        // Updating the Score Text
        scoreText.text = "Score: " + playerScore.ToString("F0");

        // Updating the Multiplier Text
        multText.text = "x" + playerScoreMult.ToString("F2");

        // Updating the Timer Hand
        timerHand.localRotation = Quaternion.Euler(0f, 0f, (timeElapised / timeAlloted) * -360f);
    }

    public void IncreaseScore(float inc)
    {
        playerScore += (inc) * playerScoreMult;
    } 

    public void ResetMultiplier()
    {
        playerScoreMult = 1f;
    }

    public void IncreaseMultiplier()
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
