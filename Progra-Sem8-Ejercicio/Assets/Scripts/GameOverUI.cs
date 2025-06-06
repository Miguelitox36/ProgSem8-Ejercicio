using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{    
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI levelReachedText;
    public TextMeshProUGUI finalScoreText; 

    
    public GameObject restartButton;
    public GameObject menuButton;    

    void Start()
    {        
        (int kills, float time, int level) = GameOverData.Load();
                
        timeSurvivedText.text = "Tiempo Sobrevivido: " + time.ToString("F1") + "s";
        enemiesKilledText.text = "Enemigos Matados: " + kills;
        levelReachedText.text = "Nivel Alcanzado: " + level;
               
        int finalScore = kills * 10 + Mathf.FloorToInt(time) * 5 + level * 100;
        finalScoreText.text = "Puntuación Final: " + finalScore;
                
        if (restartButton != null)
        {
            restartButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(RestartGame);
        }
        if (menuButton != null)
        {
            menuButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(GoToMenu);
        }
    }

    public void RestartGame()
    {        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        SceneManager.LoadScene("GamePlayScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}