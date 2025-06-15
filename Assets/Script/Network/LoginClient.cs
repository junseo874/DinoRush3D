using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    private readonly Queue<string> rankingQueue = new Queue<string>();

    private string username;
    private int highScore;
    private bool loginSuccessFlag = false;

    void Start()
    {
        Time.timeScale = 0;
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
                Debug.Log("[Client] 수신 메시지: " + message);

                if (string.IsNullOrEmpty(message)) continue;

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
                    string data = message.Substring("RANKINGS:".Length);
                    Debug.Log("[Client] 랭킹 데이터 수신됨: " + data);
                    lock (rankingQueue)
                    {
                        rankingQueue.Enqueue(data);
                    }
                }
                else
                {
                    Debug.Log("[Client] 일반 메시지 처리됨: " + message);
                    lock (chatQueue)
                    {
                        chatQueue.Enqueue(message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Client] 수신 오류: " + e.Message);
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
            Time.timeScale = 1;

            playerNameText.text = username;
            playerHighScoreText.text = $"최고 점수: {highScore}";

            RequestRankings();
        }

        lock (chatQueue)
        {
            while (chatQueue.Count > 0)
            {
                string msg = chatQueue.Dequeue();
                Debug.Log("[Client] 채팅 출력: " + msg);
                chatDisplay.text += "\n" + msg;
            }
        }

        lock (rankingQueue)
        {
            while (rankingQueue.Count > 0)
            {
                string[] lines = rankingQueue.Dequeue().Split('|');
                Debug.Log("[Client] 랭킹 분리 결과 (lines): " + string.Join(", ", lines));
                for (int i = 0; i < rankingDisplays.Length; i++)
                {
                    if (i < lines.Length)
                    {
                        string[] parts = lines[i].Split(',');
                        Debug.Log($"[Client] {i + 1}등 파싱: {lines[i]}");
                        if (parts.Length >= 2)
                        {
                            string uname = parts[0];
                            string score = parts[1];
                            rankingDisplays[i].text = $"[{i + 1}등] {uname} : {score}점";
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
        Debug.Log("게임 시작");
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