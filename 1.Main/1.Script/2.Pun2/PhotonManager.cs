using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager instance = null;
    private void Awake()
    {
        //���� Ƚ��
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
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
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        //������ �̰��� ȣ�� �ȵ�
        PhotonNetwork.LocalPlayer.NickName = "�г���";
        PhotonNetwork.JoinOrCreateRoom(SceneManager.GetActiveScene().name, new RoomOptions { MaxPlayers = 20 }, null);
        //���� �� (����� < ���� �� �̸����� ����)
    }

    public override void OnJoinedRoom()
    {

        //�� ���� ��
        Spawn();
    }

    public void Spawn()
    {
        //ĳ���� ��ȯ
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //���� ���� ��
    }
}
