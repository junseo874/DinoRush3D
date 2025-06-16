using System.Net.Sockets;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public string Username;
    public int HighScore;
    public TcpClient Client;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(string username, int highScore, TcpClient client)
    {
        this.Username = username;
        this.HighScore = highScore;
        this.Client = client;
    }
}