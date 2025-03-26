using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEditor.PackageManager.Requests;

public class DataBase : MonoBehaviour
{
    private string saveDataURL = "http://localhost/Api/";

    private static DataBase instance = null;
    //�̱���
    private void Awake()
    {
        Singleton();
    }
    private void Singleton()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            if (instance == this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public static DataBase Instance // �̱��� ���ȼ�
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    //-------------------------------------------------------

    private ClientWebSocket ws = new ClientWebSocket();

    async void Start()
    {
        await ws.ConnectAsync(new System.Uri("ws://127.0.0.1:8080"), CancellationToken.None);
        Debug.Log("Connected to WebSocket Server");

        // �޽��� ������
        //await SendMessage("Hello from Unity!");

        // �޽��� ����
        //await ReceiveMessage();
    }

    public async void SendMessageApi(string jsonData, string path, Action<bool, string> requestMsg)
    {
        await SendMessage(jsonData, path, requestMsg);
    }
    async Task SendMessage(string jsonData, string path, Action<bool, string> requestMsg)
    {

        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);//�޽����� UTF-8 ����Ʈ �迭�� ��ȯ
            await ws.SendAsync(new System.ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            //WebSocketMessageType.Text �� �޽��� Ÿ���� �ؽ�Ʈ ������


            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result = await ws.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);
            string receivedMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);

            //Debug.Log(receivedMsg);//������ ���� �޽��� �ޱ�
            requestMsg?.Invoke(true, receivedMsg);
        }
        catch (WebSocketException wsEx)
        {
            Debug.LogError($"WebSocket ���� ����: {wsEx.Message}");
            requestMsg?.Invoke(false, $"WebSocket ����: {wsEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"����ġ ���� ���� �߻�: {ex.Message}");
            requestMsg?.Invoke(false, $"���� �߻�: {ex.Message}");
        }
    }

}