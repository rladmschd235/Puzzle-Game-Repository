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
        if (isDrag) // 드래그 상태일 때
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 커서 위치 저장

            float leftBorder = -4.2f + transform.localScale.x / 2; // 좌측 경계
            float rightBorder = 4.2f - transform.localScale.x / 2; // 우측 경계

            mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder); // x축 경계 제한
            mousePos.y = 8;
            mousePos.z = 0;

            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // 부드럽게 해주는 값은 0부터 1 사이의 값을 넣어야함
        }
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag= false;
        rigid.simulated = true; // 물리작용 활성화
    }
}
