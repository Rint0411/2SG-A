using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.DownLoad("PlayerModel", false);//�÷��̾� �� �̸� �ޱ�

        GameManager.Instance.MoveMap("SignUp"); //�α��� ȭ������ �̵�
    }

}
