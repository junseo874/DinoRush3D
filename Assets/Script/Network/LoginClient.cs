// ============================================================================
// LoginClient.cs - 서버 통신 기반 로그인/채팅/랭킹 UI 제어 클라이언트
// 목적: TCP로 로그인 및 랭킹 정보 송수신, UI 동기화 및 GameSession 연결 처리
// 작성자: 이준서
// 작성일: 2025년 06월 15일
// 구조: 서버 연결 → 메시지 수신 스레드 → UI 갱신 → GameScene 진입 시 상태 전달
// ============================================================================

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
    // UI 오브젝트 연결
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

    // 통신 관련
    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;
    private Thread receiveThread;
    private bool isConnected = false;

    // 수신 메시지 큐
    private readonly Queue<string> chatQueue = new Queue<string>();
    private readonly Queue<string[]> rankingQueue = new Queue<string[]>();

    // 로그인 정보
    private string username;
    private int highScore;
    private bool loginSuccessFlag = false;

    void Start()
    {
        // GameScene에서는 자동 파괴
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Destroy(this.gameObject);
            return;
        }

        Time.timeScale = 0;
        loginPanel.SetActive(false);
        startPanel.SetActive(false);

        ConnectToServer("127.0.0.1", 7777);

        // 버튼 리스너 등록
        if (loginBtn != null) loginBtn.onClick.AddListener(OnLogin);
        if (sendBtn != null) sendBtn.onClick.AddListener(SendChatMessage);
        if (startBtn != null) startBtn.onClick.AddListener(OnStartGame);
        if (quitBtn != null) quitBtn.onClick.AddListener(OnQuitGame);

        // 이전 세션 정보가 있을 경우 자동 로그인 처리
        if (GameSession.Instance != null && GameSession.Instance.Client != null)
        {
            username = GameSession.Instance.Username;
            highScore = GameSession.Instance.HighScore;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

            loginPanel.SetActive(false);
            startPanel.SetActive(true);

            RequestRankings();
            Debug.Log("[LoginClient] 이전 세션 유지 - StartPanel 표시");
        }
        else
        {
            loginPanel.SetActive(true);
            startPanel.SetActive(false);
        }
    }

    // 서버에 연결
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

    // 메시지 수신 스레드
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

            loginPanel?.SetActive(false);
            startPanel?.SetActive(true);
            Time.timeScale = 0;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

            RequestRankings();
        }

        // 채팅 메시지 출력
        lock (chatQueue)
        {
            while (chatQueue.Count > 0)
            {
                string msg = chatQueue.Dequeue();
                chatDisplay.text += "\n" + msg;
            }
        }

        // 랭킹 정보 출력
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

    // 로그인 메시지 전송
    void OnLogin()
    {
        if (!isConnected) return;

        string id = userNameInput.text.Trim();
        string pw = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw)) return;

        string msg = $"LOGIN:{id}:{pw}";
        writer.WriteLine(msg);
    }

    // 채팅 전송
    public void SendChatMessage()
    {
        if (!isConnected || string.IsNullOrWhiteSpace(chatInput.text)) return;

        string chatMsg = $"{username}: {chatInput.text}";
        writer.WriteLine(chatMsg);
        chatInput.text = "";
    }

    // 서버에 랭킹 요청
    public void RequestRankings()
    {
        writer.WriteLine("GET_RANKINGS");
    }

    // 게임 시작 버튼 클릭 시
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