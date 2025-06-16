// ============================================================================
// GameSession.cs - 게임 세션 정보 관리 싱글톤
// 목적: 로그인 후 사용자 이름, 최고 점수, TCP 클라이언트 객체를 씬 간에 유지
// 작성자: 이준서
// 작성일: 2025년 06월 10일
// 주요 기능:
// - Username, HighScore, TcpClient를 다른 씬에서도 유지하도록 저장
// - 싱글톤 패턴으로 전역 접근 가능
// - DontDestroyOnLoad로 씬 전환에도 파괴되지 않음
// ============================================================================

using System.Net.Sockets;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameSession Instance { get; private set; }

    // 로그인한 사용자 이름
    public string Username;

    // 사용자 최고 점수
    public int HighScore;

    // 서버와의 TCP 연결 객체
    public TcpClient Client;

    void Awake()
    {
        // 이미 존재하는 인스턴스가 있다면 자신은 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 싱글톤 인스턴스로 설정 및 파괴 방지
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 로그인 시 초기화 함수
    public void Initialize(string username, int highScore, TcpClient client)
    {
        this.Username = username;
        this.HighScore = highScore;
        this.Client = client;
    }
}