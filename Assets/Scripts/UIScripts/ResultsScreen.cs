using System.Collections;
using TMPro; // Needed for TextMeshPro
using UnityEngine;
using Scripts.Management;

public class ResultsScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panelContent; // Assign the visual child object here
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI ratioText;

    [Header("Settings")]
    [SerializeField] private float delayAfterGameOver = 2.5f; 

    private void Start()
    {
        if(panelContent != null) panelContent.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState newState)
    {
        if (newState == GameState.GameOver)
        {
            StartCoroutine(ShowResultsRoutine());
        }
        else
        {
            if(panelContent is not null) panelContent.SetActive(false);
        }
    }

    public IEnumerator ShowResultsRoutine()
    {
        // wait for the main "Game Over" message to have its moment
        yield return new WaitForSeconds(delayAfterGameOver);
        yield return null;

        // get data from ShootingStats
        if (ShootingStats.Instance != null)
        {
            float shots = ShootingStats.Instance.ShotsFired;
            float hits = ShootingStats.Instance.EnemiesHit;
            float ratio = ShootingStats.Instance.HitMissRatio();

            if (shotsText != null) shotsText.text = shots.ToString("0"); // "0" removes decimals
            if (hitsText != null) hitsText.text = hits.ToString("0");
            
            
            if (ratioText != null) ratioText.text = $"{ratio.ToString("F1")} %"; // "F1" formats to 1 decimal place (e.g., 54.5 %)
        }

        if (panelContent != null) panelContent.SetActive(true);
    }
}