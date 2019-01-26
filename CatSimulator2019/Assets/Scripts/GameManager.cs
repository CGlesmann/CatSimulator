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
    public float playerScore = 0f;
    public float playerScoreMult = 1f;

    [Header("GUI References")]
    [SerializeField] private Image staminaBar = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;

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
    }

    /// <summary>
    /// Updating the GUI
    /// </summary>
    private void UpdateGUI()
    {
        // Updating the Stamina Bar
        float fill = (CatController.cat.stamina / 100f);
        staminaBar.fillAmount = fill;

        // Updating the Score Text
        scoreText.text = "Score: " + playerScore.ToString("F0");
    }
}
