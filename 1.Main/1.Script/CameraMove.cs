using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    private float Yaxis;
    private float Xaxis;

    [SerializeField]
    private Transform target;//Player
    public Vector3 direction;

    private float rotSensitive = 3f;
    private float dis = 4f;
    private float RotationMin = -10f;
    private float RotationMax = 80f;
    private float smoothTime = 0.12f;
    private Vector3 targetRotation;
    private Vector3 currentVel;

    [SerializeField]
    private Joystick joystick;
    private bool first = true;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
    }

    void LateUpdate()
    {
        Vector3 vec = joystick.direction.normalized;

        // Ȯ�� �غ� ��
#if UNITY_ANDROID || UNITY_IOS //|| UNITY_EDITOR
        Yaxis = Yaxis + vec.x * rotSensitive;
        Xaxis = Xaxis - vec.y * rotSensitive;

#else
        if (Cursor.visible == false)
            Cursor.visible = true;

        if (!Input.GetMouseButton(1)&& first == false) return; // ��Ŭ�� �ȴ����� ������ ����

        if (first) first = false;

        if (Cursor.visible == true)
            Cursor.visible = false;

        Yaxis = Yaxis + Input.GetAxis("Mouse X") * rotSensitive;
        Xaxis = Xaxis - Input.GetAxis("Mouse Y") * rotSensitive;
#endif

        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        Vector3 startPosition = target.position - transform.forward * dis;
        //direction = target.position - startPosition; �ݴ��
        direction = startPosition - target.position;

        // �����ɽ�Ʈ�� �߻�
        RaycastHit hit;

        Debug.DrawLine(target.position, target.position + direction, Color.green);


        if (Physics.Raycast(target.position, direction, out hit, direction.magnitude, (-1) -(1<< LayerMask.NameToLayer("Player"))))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = startPosition;
        }



        dis = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel") + dis,3, 13);
    }
}
