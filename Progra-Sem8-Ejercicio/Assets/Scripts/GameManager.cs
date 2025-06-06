using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using GameJolt.API;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int enemiesKilled;
    public float timeSurvived;
    public int level;

    public Transform PlayerTransform;

    public event Action OnEnemyKill;
    public event Action OnGameOver;
    public event Action OnLevelUp;
        
    private BossSpawner bossSpawner;

    private float levelUpTimeThreshold = 30f;
    private float lastLevelUpTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (PlayerTransform == null)
        {
            Debug.LogError("GameManager: No se encontró el jugador con la etiqueta 'Player'.");
        }
                
        bossSpawner = FindObjectOfType<BossSpawner>();
        if (bossSpawner == null)
        {
            Debug.LogWarning("GameManager: No se encontró BossSpawner en la escena.");
        }

        enemiesKilled = 0;
        timeSurvived = 0f;
        level = 1;
        lastLevelUpTime = Time.time;

        OnLevelUp?.Invoke();
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;

        if (Time.time - lastLevelUpTime >= levelUpTimeThreshold)
        {
            level++;
            lastLevelUpTime = Time.time;
            OnLevelUp?.Invoke();
            Debug.Log($"¡Nivel Subido! Nivel actual: {level}");
        }

        if (timeSurvived >= 60f)
        {
            Trophies.Unlock(269855);
            Debug.Log("¡Has sobrevivido 60 segundos!");
        }
    }

    public void EnemyKilled()
    {

        if (enemiesKilled == 1)
        {
            Trophies.Unlock(269854);
            Debug.Log("¡Primer enemigo eliminado!");            
        }

        enemiesKilled++;
        OnEnemyKill?.Invoke();
                
        if (bossSpawner != null)
        {
            bossSpawner.OnEnemyKilled();
        }
    }

    public void GameOver()
    {
        Debug.Log("¡Juego Terminado!");
        OnGameOver?.Invoke();

        GameOverData.Save(enemiesKilled, timeSurvived, level);

        SceneManager.LoadScene("GameOver");

        int time = Mathf.FloorToInt(timeSurvived);
        Scores.Add(time, $"{time} segundos", 1010348, "", success =>
        {
            Debug.Log("¿Score enviado? " + success);
        });

        int score = Mathf.FloorToInt(enemiesKilled);
        Scores.Add(score, $"{score} segundos", 1010618, "", success =>
        {
            Debug.Log("¿Score enviado? " + success);

        });
    }

    public void ResetGame()
    {
        enemiesKilled = 0;
        timeSurvived = 0f;
        level = 1;
    }
}
