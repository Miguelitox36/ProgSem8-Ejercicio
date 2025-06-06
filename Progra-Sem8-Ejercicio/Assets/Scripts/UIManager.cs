using UnityEngine;
using TMPro; 
public class UIManager : MonoBehaviour
{    
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKill += UpdateKillsUI; 
            GameManager.Instance.OnLevelUp += UpdateLevelUI;  
                       
            UpdateKillsUI();
            UpdateLevelUI();
        }
        else
        {
            Debug.LogError("UIManager: GameManager.Instance no encontrado. Asegúrate de que GameManager esté en la escena y sea un Singleton.");
        }
    }

    void Update()
    {        
        if (GameManager.Instance != null)
        {
            timeText.text = "Tiempo: " + GameManager.Instance.timeSurvived.ToString("F1") + "s";
        }
    }

    void UpdateKillsUI()
    {
        if (GameManager.Instance != null)
        {
            killsText.text = "Kills: " + GameManager.Instance.enemiesKilled;
        }
    }

    void UpdateLevelUI()
    {
        if (GameManager.Instance != null)
        {
            levelText.text = "Nivel: " + GameManager.Instance.level;
        }
    }
}