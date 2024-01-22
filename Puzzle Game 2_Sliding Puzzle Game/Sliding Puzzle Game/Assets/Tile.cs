using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textNumeric;
    private Board board;
    private Vector3 correctPosition;

    private int numeric;
    public int Numeric
    {
        set
        {
            numeric = value;
            textNumeric.text = numeric.ToString();
        }
        get => numeric;
    }

    public void Setup(Board board, int hideNumeric, int numeric)
    {
        this.board = board;
        textNumeric = GetComponentInChildren<TextMeshProUGUI>();

        Numeric = numeric;
        if(Numeric == hideNumeric) // 한 칸은 비어있어야 타일 이동 가능 -> 숨길 한 칸이라면 Image, Text UI 비활성화
        {
            GetComponent<UnityEngine.UI.Image>().enabled = false; // Image 클래스는 한번만 사용 -> using 처리하지 않음
            textNumeric.enabled = false;
        }
    }

    public void SetCorrectPosition()
    {
        correctPosition = GetComponent<RectTransform>().localPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭했을 때 행동
        Debug.Log("click" + Numeric);
        board.IsMoveTile(this);
    }

    public void OnMoveTo(Vector3 end)
    {
        StartCoroutine("MoveTo", end);
    }

    private IEnumerator MoveTo(Vector3 end)
    {
        float current = 0;
        float percent = 0;
        float moveTime = 0.1f;
        Vector3 start = GetComponent<RectTransform>().localPosition;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            GetComponent<RectTransform>().localPosition = Vector3.Lerp(start, end, percent);

            yield return null;  
        }
    }
}