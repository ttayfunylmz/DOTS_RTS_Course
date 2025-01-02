using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWinUI : MonoBehaviour
{


    [SerializeField] private Button mainMenuButton;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        });
    }

    private void Start()
    {
        DOTSEventsManager.Instance.OnGameWin += DOTSEventsManager_OnGameWin;

        Hide();
    }

    private void DOTSEventsManager_OnGameWin(object sender, System.EventArgs e)
    {
        Show();
        Time.timeScale = 0f;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
