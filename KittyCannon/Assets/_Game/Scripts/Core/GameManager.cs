using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State { Menu, Playing, GameOver }
    public State Current { get; private set; } = State.Menu;

    [Header("UI Panels")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject hudPanel;
    [SerializeField] GameObject gameOverPanel;

    [Header("Refs")]
    [SerializeField] CannonController cannon;

    void Start() => GoMenu();

    public void GoMenu()
    {
        Current = State.Menu;
        menuPanel?.SetActive(true);
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        // reset any scene state if needed
    }

    public void StartGame()
    {
        Current = State.Playing;
        menuPanel?.SetActive(false);
        hudPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
    }

    public void EndGame()
    {
        Current = State.GameOver;
        menuPanel?.SetActive(false);
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
    }

    // Hook buttons in the UI to StartGame / GoMenu, etc.
}