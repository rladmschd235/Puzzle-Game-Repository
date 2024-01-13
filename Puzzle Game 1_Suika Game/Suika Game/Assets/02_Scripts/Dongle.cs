using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    private Rigidbody2D rigid;

    public bool isDrag;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDrag) // �巡�� ������ ��
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // ���콺 Ŀ�� ��ġ ����

            float leftBorder = -4.2f + transform.localScale.x / 2; // ���� ���
            float rightBorder = 4.2f - transform.localScale.x / 2; // ���� ���

            mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder); // x�� ��� ����
            mousePos.y = 8;
            mousePos.z = 0;

            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // �ε巴�� ���ִ� ���� 0���� 1 ������ ���� �־����
        }
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag= false;
        rigid.simulated = true; // �����ۿ� Ȱ��ȭ
    }
}
