using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoginClient : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject startPanel;

    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public Button loginBtn;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHighScoreText;
    public TextMeshProUGUI[] rankingDisplays;

    public TMP_InputField chatInput;
    public TextMeshProUGUI chatDisplay;
    public Button sendBtn;

    public Button startBtn;
    public Button quitBtn;

    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;
    private Thread receiveThread;

    private string username;
    private int highScore;
    private bool loginSuccessFlag = false;

    private readonly Queue<string> chatQueue = new Queue<string>();
    private readonly Queue<string[]> rankingQueue = new Queue<string[]>();

    private bool isConnected = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 재시작 시 유지
    }

    void Start()
    {
        Time.timeScale = 0f;

        loginPanel.SetActive(true);
        startPanel.SetActive(false);

        ConnectToServer("127.0.0.1", 7777);

        loginBtn.onClick.AddListener(OnLogin);
        sendBtn.onClick.AddListener(SendChatMessage);
        startBtn.onClick.AddListener(OnStartGame);
        quitBtn.onClick.AddListener(OnQuitGame);
    }

    void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient(ip, port);
            NetworkStream stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            isConnected = true;

            receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("[LoginClient] 서버 연결 성공");
        }
        catch (Exception e)
        {
            Debug.LogError("[LoginClient] 서버 연결 실패: " + e.Message);
        }
    }

    void ReceiveMessages()
    {
        while (isConnected)
        {
            try
            {
                string message = reader.ReadLine();
                if (string.IsNullOrEmpty(message)) continue;

                Debug.Log("[서버 수신] " + message);

                if (message.StartsWith("LOGIN_SUCCESS"))
                {
                    string[] parts = message.Split(':');
                    username = parts[1];
                    highScore = int.Parse(parts[2]);

                    loginSuccessFlag = true;

                    lock (chatQueue)
                        chatQueue.Enqueue($"[시스템] 로그인 성공 - {username} / 최고 점수: {highScore}");
                }
                else if (message.StartsWith("RANKINGS:"))
                {
                    string[] rankings = message.Substring("RANKINGS:".Length).Split('|');
                    lock (rankingQueue)
                        rankingQueue.Enqueue(rankings);
                }
                else
                {
                    lock (chatQueue)
                        chatQueue.Enqueue(message);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[서버 수신 오류] " + e.Message);
                isConnected = false;
            }
        }
    }

    void Update()
    {
        if (loginSuccessFlag)
        {
            loginSuccessFlag = false;

            loginPanel.SetActive(false);
            startPanel.SetActive(true);
            Time.timeScale = 0f;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

            ScoreManager.Instance.Initialize(username, client, highScore);
            RequestRankings();
        }

        lock (chatQueue)
        {
            while (chatQueue.Count > 0)
            {
                string msg = chatQueue.Dequeue();
                chatDisplay.text += "\n" + msg;
            }
        }

        lock (rankingQueue)
        {
            while (rankingQueue.Count > 0)
            {
                string[] ranks = rankingQueue.Dequeue();
                for (int i = 0; i < rankingDisplays.Length; i++)
                {
                    if (i < ranks.Length)
                    {
                        string[] parts = ranks[i].Split(',');
                        rankingDisplays[i].text = $"[{i + 1}등] {parts[0]} : {parts[1]}점";
                    }
                    else
                    {
                        rankingDisplays[i].text = "";
                    }
                }
            }
        }
    }

    void OnLogin()
    {
        if (!isConnected) return;

        string id = userNameInput.text.Trim();
        string pw = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw)) return;

        string loginMessage = $"LOGIN:{id}:{pw}";
        writer.WriteLine(loginMessage);
    }

    public void SendChatMessage()
    {
        if (!isConnected || string.IsNullOrEmpty(chatInput.text)) return;

        string message = $"{username}: {chatInput.text}";
        writer.WriteLine(message);
        chatInput.text = "";
    }

    public void RequestRankings()
    {
        writer.WriteLine("GET_RANKINGS");
    }

    void OnStartGame()
    {
        Debug.Log("[시작] 게임 시작 버튼 클릭됨");
        Time.timeScale = 1f;

        // 씬 리로드 전 점수 리셋
        ScoreManager.Instance.ResetScore();

        // 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnQuitGame()
    {
        Debug.Log("[종료] 게임 종료 버튼 클릭됨");
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        isConnected = false;
        reader?.Close();
        writer?.Close();
        client?.Close();
    }

    public void SendScoreToServer(int finalScore)
    {
        if (writer == null || !client.Connected) return;

        string message = $"SAVE_SCORE/{username}/{finalScore}";
        writer.WriteLine(message);
        Debug.Log($"[LoginClient] 서버로 점수 전송: {message}");
    }
}