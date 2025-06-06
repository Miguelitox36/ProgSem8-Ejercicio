using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameJolt.API;
using GameJolt.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button trophiesButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button rankingButton; 
    [SerializeField] private Button quitButton;

    private void Awake()
    {        
        if (trophiesButton != null)
        {
            trophiesButton.onClick.AddListener(() =>
            {                
                GameJoltUI.Instance.ShowTrophies();
            });
        }
        else Debug.LogError("Trophies Button no asignado en MenuManager.");

        if (playButton != null)
        {
            playButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GamePlayScene"); 
            });
        }
        else Debug.LogError("Play Button no asignado en MenuManager.");

        if (rankingButton != null)
        {
            rankingButton.onClick.AddListener(() =>
            {
                GameJoltUI.Instance.ShowLeaderboards();
            });
        }
        else Debug.LogError("Ranking Button no asignado en MenuManager.");

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false; // Para salir del editor
            });
        }
        else Debug.LogError("Quit Button no asignado en MenuManager.");
    }      
}