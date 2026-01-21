using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TMP_Text statusText;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public Button restartButton;

    GameManager gm;

    void Awake()
    {
        if (restartButton)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    public void Refresh(GameManager gameManager)
    {
        gm = gameManager;
        if (!gm) return;

        if (scoreText) scoreText.text = $"Score: {gm.score}";
        if (livesText) livesText.text = $"Lives: {gm.lives}";

        if (statusText)
        {
            statusText.text = gm.state switch
            {
                GameManager.State.Placing => "Start the game to place the board. May the force be with you!",
                GameManager.State.Playing => "Point at the enemies! Shield the holy avocado with your life!",
                GameManager.State.GameOver => "Game Over",
                _ => ""
            };
        }

        if (restartButton)
            restartButton.gameObject.SetActive(gm.state != GameManager.State.Playing);
    }

    void OnRestartClicked()
    {
        if (gm) gm.Restart();
    }
}
