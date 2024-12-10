using System;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private void Start() 
    {
        DOTSEventsManager.Instance.OnHQDead += DOTSEventsManager_OnHQDead;

        Hide();    
    }

    private void DOTSEventsManager_OnHQDead(object sender, EventArgs e)
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
