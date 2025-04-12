using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

public class DataBase : MonoBehaviour
{
    public int stu_local_code { get; set; } = 1;

    private string nodeURL = "ws://127.0.0.1:8080";

    private static DataBase instance = null;

    private ClientWebSocket ws = new ClientWebSocket();
    StateRe stateRe = StateRe.ready;

    private UserID_Pw data;

    private bool loginOn = false;// �α��� ���ΰ�?
    private bool isReceiving = false; // ���� ���� ������ Ȯ��
    [System.Serializable]
    public class UserID_Pw
    {
        public string stu_number;
        public string stu_password;
        public int stu_local_code;
        public string time;
    }
    enum StateRe
    {
        trying,
        ready
    }
    //�̱���
    private async void Awake()
    {
        Singleton();
        await ConnectWebSocketAsync();
    }

    // Connect WebSocket
    private async Task ConnectWebSocketAsync()
    {
        if(stateRe == StateRe.trying)
        {
            return;
        }
        stateRe = StateRe.trying;//�õ���
        while (true)
        {
            try
            {
                /*
                if (ws.State == WebSocketState.Open)
                {
                    return; // �̹� ������ �Ǿ������� �߰� ������ �õ����� ����
                }
                */
                ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(nodeURL), CancellationToken.None);
                Debug.Log("WebSocket ���� ����");
                stateRe = StateRe.ready;//�ٽ� �õ� ����

                return;
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket ���� ����: {ex.Message}");

                await Task.Delay(1000); // 1�� �� �ٽ� �õ�
            }
        }
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

    public async void SendMessageApi(string message, string path, Action<bool, string> requestMsg)
    {
        //await ws.ConnectAsync(new System.Uri("ws://127.0.0.1:8080"), CancellationToken.None);
        await SendMessage(message, path, requestMsg);
    }
    async Task SendMessage(string message, string path, Action<bool, string> requestMsg)
    {   while (true)
        {
            if (isReceiving)
            {
                Debug.Log("�̹� ���� ��");
                await Task.Delay(100);
            }
            else
            {
                Debug.Log("���");
                break;
            }
        }
        // �̹� ���� ���̸� �������� ����
        isReceiving = true;

        message = path + "{****}" + message;
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);//�޽����� UTF-8 ����Ʈ �迭�� ��ȯ
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
            await ConnectWebSocketAsync();
            // WebSocket �� �õ�



            Debug.LogError($"WebSocket ���� ����: {wsEx.Message}");
            requestMsg?.Invoke(false, $"WebSocket ����: {wsEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"����ġ ���� ���� �߻�: {ex.Message}");
            requestMsg?.Invoke(false, $"���� �߻�: {ex.Message}");
        }
        finally
        {
            Debug.Log("finally");
            isReceiving = false; // ���� �Ϸ� �� �ٽ� ���� �� �ֵ��� ����
        }
    }
    private async void OnApplicationQuit()
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application is quitting", CancellationToken.None);
            Debug.Log("WebSocket ���� ����");
        }
    }
    public void Login(string _id, string _password)
    {
        if (loginOn) return;

        DataBase.Instance.stu_local_code = UnityEngine.Random.Range(1, 2147483646);
        // ������ ������ (JSON ��������)
        data = new UserID_Pw
        {
            stu_number = _id,
            stu_password = _password,
            stu_local_code = DataBase.Instance.stu_local_code, //���� �� ( �α��� �� ���� �ٸ� ���̵�� ���Ƶ� �� )
            time = DateTime.Now.ToString(("mm")) //���� �� ����
        };

        // JSON �������� ��ȯ
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);

        DataBase.Instance.SendMessageApi(jsonData, "Login", (Success, request) => {
            Debug.Log(request);
            if(request == "1")
            {
                //�α��� ���� ��
                loginOn = true;
                //StartCoroutine(DataUpdate());
            }
        });


    }
    float time = 0;
    private void Update()
    {
        if (loginOn==false) return;

        time += Time.deltaTime;

        //Debug.Log(time);
        if (time > 40)
        {
            Debug.Log(time);
            DataBase.Instance.SendMessageApi(data.stu_number.ToString(), "UidCheck", (Success, request) => {

                Debug.Log(stu_local_code.ToString());

                if (request != stu_local_code.ToString())
                {
                    loginOn = false;
                    Debug.Log("������ �ٸ�");
                }
            });

            if(loginOn)
            {
                Debug.Log("������ �ð� ������Ʈ");
                data.time = DateTime.Now.ToString(("mm"));
                string jsonData = JsonUtility.ToJson(data);
                DataBase.Instance.SendMessageApi(jsonData, "Upate", (Success, request) => { });
            }

            //���࿡ ���� uid �ڵ尡 ������ �ִ°Ͱ� �ٸ��� �α׾ƿ� �Ұ� ( �α��� â���� �ٽ� ����)
            time = 0;
        }
    }
}