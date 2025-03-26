using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Save("2401017", "�赵��", "kim1234", "1234");
    }

    [System.Serializable] //����ȭ
    public class PostDataSet
    {
        public string student_num;
        public string name;
        public string id;
        public string password;
    }

    public void Save(string _student_num, string _name, string _id, string _password)
    {
        // ������ ������ ��ü (JSON ��������)
        PostDataSet data = new PostDataSet
        {
            student_num = _student_num,
            name = _name,
            id = _id,
            password = _password
        };

        // ��ü�� JSON �������� ��ȯ
        string jsonData = JsonUtility.ToJson(data);
        DataBase.Instance.SendMessageApi(jsonData, "Login/Login.php", (Success, request) => {

            Debug.LogError(request);

        });
        

    }
}
