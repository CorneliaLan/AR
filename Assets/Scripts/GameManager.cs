using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { Placing, Playing, GameOver }
    public State state = State.Placing;

    public int lives = 3;
    public int score = 0;

    public HUD hud;
    public Spawner spawner;

    GameObject boardInstance;

    void Start()
    {
        SetState(State.Placing);
        hud.Refresh(this);
    }

    public void OnBoardPlaced(GameObject board)
    {
        boardInstance = board;

        // Spawner konfigurieren
        spawner.ConfigureFromBoard(boardInstance);

        score = 0;
        lives = 3;
        SetState(State.Playing);
        spawner.Begin();
        hud.Refresh(this);
    }

    public void AddScore(int amount)
    {
        score += amount;
        hud.Refresh(this);
    }

    public void DamageBase(int amount)
    {
        lives -= amount;
        if (lives <= 0)
        {
            lives = 0;
            GameOver();
        }
        hud.Refresh(this);
    }

    void GameOver()
    {
        SetState(State.GameOver);
        spawner.Stop();
    }

    void SetState(State s)
    {
        state = s;
        hud.Refresh(this);
    }

    public void Restart()
    {
        // Einfacher Restart: Szene neu laden wäre auch ok,
        // aber hier machen wir’s ohne SceneManagement:
        score = 0;
        lives = 3;

        if (boardInstance != null) Destroy(boardInstance);
        spawner.Stop();

        SetState(State.Placing);
    }
}
