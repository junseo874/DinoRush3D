// 수정된 LoginClient.cs - GameSession 확인 후 StartPanel만 표시
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private bool isConnected = false;

    private readonly Queue<string> chatQueue = new Queue<string>();
    private readonly Queue<string[]> rankingQueue = new Queue<string[]>();

    private string username;
    private int highScore;
    private bool loginSuccessFlag = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Destroy(this.gameObject);
            return;
        }

        Time.timeScale = 0;

        // 일단 두 패널 다 끄고 시작
        loginPanel.SetActive(false);
        startPanel.SetActive(false);

        ConnectToServer("127.0.0.1", 7777);

        // UI 버튼 연결
        if (loginBtn != null) loginBtn.onClick.AddListener(OnLogin);
        if (sendBtn != null) sendBtn.onClick.AddListener(SendChatMessage);
        if (startBtn != null) startBtn.onClick.AddListener(OnStartGame);
        if (quitBtn != null) quitBtn.onClick.AddListener(OnQuitGame);

        // 클라이언트 세션이 유지된 상태면 StartPanel만 띄우고 정보 복원
        if (GameSession.Instance != null && GameSession.Instance.Client != null)
        {
            loginPanel.SetActive(false);
            startPanel.SetActive(true);

            username = GameSession.Instance.Username;
            highScore = GameSession.Instance.HighScore;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

            RequestRankings();
            Debug.Log("[LoginClient] 이전 세션 유지 - StartPanel 표시");
        }
        else
        {
            // 처음 실행 시: 로그인 패널 띄우기
            loginPanel.SetActive(true);
        }
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

            Debug.Log("[Client] 서버에 연결됨");
        }
        catch (Exception e)
        {
            Debug.LogError("[Client] 연결 실패: " + e.Message);
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

                Debug.Log("[Client] 수신 메시지: " + message);

                if (message.StartsWith("LOGIN_SUCCESS"))
                {
                    string[] parts = message.Split(':');
                    username = parts[1];
                    highScore = int.Parse(parts[2]);

                    lock (chatQueue)
                    {
                        chatQueue.Enqueue($"로그인 성공: {username} / 최고 점수: {highScore}");
                    }

                    loginSuccessFlag = true;
                }
                else if (message.StartsWith("RANKINGS:"))
                {
                    string raw = message.Substring("RANKINGS:".Length);
                    string[] lines = raw.Split('|');

                    lock (rankingQueue)
                    {
                        rankingQueue.Enqueue(lines);
                    }
                }
                else
                {
                    lock (chatQueue)
                    {
                        chatQueue.Enqueue(message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Client] 수신 오류 (무시): " + e.Message);
                isConnected = false;
            }
        }
    }

    void Update()
    {
        if (loginSuccessFlag)
        {
            loginSuccessFlag = false;

            if (loginPanel != null) loginPanel.SetActive(false);
            if (startPanel != null) startPanel.SetActive(true);
            Time.timeScale = 0;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

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
                string[] lines = rankingQueue.Dequeue();
                for (int i = 0; i < rankingDisplays.Length; i++)
                {
                    if (i < lines.Length)
                    {
                        string[] parts = lines[i].Split(',');
                        if (parts.Length == 2)
                        {
                            rankingDisplays[i].text = $"[{i + 1}등] {parts[0]} : {parts[1]}점";
                        }
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

        string msg = $"LOGIN:{id}:{pw}";
        writer.WriteLine(msg);
    }

    public void SendChatMessage()
    {
        if (!isConnected || string.IsNullOrWhiteSpace(chatInput.text)) return;

        string chatMsg = $"{username}: {chatInput.text}";
        writer.WriteLine(chatMsg);
        chatInput.text = "";
    }

    public void RequestRankings()
    {
        writer.WriteLine("GET_RANKINGS");
    }

    void OnStartGame()
    {
        Debug.Log("[LoginClient] 게임 시작 - GameSession에 정보 저장 후 GameScene 로드");

        if (GameSession.Instance != null)
        {
            GameSession.Instance.Initialize(username, highScore, client);
        }
        else
        {
            Debug.LogError("[LoginClient] GameSession.Instance가 존재하지 않습니다.");
        }

        var scoreManagerObj = GameObject.Find("ScoreManager");
        if (scoreManagerObj != null)
        {
            DontDestroyOnLoad(scoreManagerObj);
        }

        SceneManager.LoadScene("GameScene");
    }

    void OnQuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        isConnected = false;
        reader?.Close();
        writer?.Close();
        client?.Close();
    }
}
