using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab; // 숫자 타일 프리팹
    [SerializeField] private Transform tileParent; // 타일이 배치되는 "Board" 오브젝트의 Transform

    private List<Tile> tileList; // 생성 타일 정보 저장
    
    private Vector2Int puzzleSize = new Vector2Int(4, 4); // 4X4 퍼즐
    private float neighborTileDistance = 102; // 인접한 타일 사이의 거리. 별도로 계산할 수도 있다.
    
    private Vector3 EmptyTilePostion { set; get; } //빈 타일의 위치

    private IEnumerator Start()
    {
        tileList = new List<Tile>();
        SpawnTiles();

        // 실제 위치는 바뀌지 않기 때문에 레이아웃의 위치를 본래 위치로 재구축
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(tileParent.GetComponent<RectTransform>());

        // 현재 프레임이 종료될 때까지 대기
        yield return new WaitForEndOfFrame();

        // tileList에 있는 모든 SetCorrectPosition() 메소드 호출
        tileList.ForEach(x => x.SetCorrectPosition());

        StartCoroutine("OnSuffle");
    }

    private void SpawnTiles()
    {
        for (int y = 0; y < puzzleSize.y; ++y)
        {
            for (int x = 0; x < puzzleSize.x; ++x)
            {
                GameObject clone = Instantiate(tilePrefab, tileParent);
                Tile tile = clone.GetComponent<Tile>();

                tile.Setup(this, puzzleSize.x * puzzleSize.y, y * puzzleSize.x + x + 1);

                tileList.Add(tile);
            }
        }
    }

    private IEnumerator OnSuffle()
    {
        float current = 0;
        float percent = 0;
        float time = 1.5f;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            int index = Random.Range(0, puzzleSize.x * puzzleSize.y);
            tileList[index].transform.SetAsLastSibling(); // 임의의 타일을 마지막 자식으로 설정 -> 배치 섞임

            yield return null;
        }

        // 원래 셔플 방식은 다른 방식이었는데 UI, GridLayoutGroup을 사용하다보니 자식의 위치를 바꾸는 것으로 설정
        // 그래서 현재 타일리스트의 마지막에 있는 요소가 무조건 빈 타일
        EmptyTilePostion = tileList[tileList.Count - 1].GetComponent<RectTransform>().localPosition;
    }

    public void IsMoveTile(Tile tile)
    {
        if(Vector3.Distance(EmptyTilePostion, tile.GetComponent<RectTransform>().localPosition) == neighborTileDistance)
        {
            Vector3 goalPosition = EmptyTilePostion; // 타일이 이동할 위치 저장

            EmptyTilePostion = tile.GetComponent<RectTransform>().localPosition; // 타일이 빌 위치 저장

            tile.OnMoveTo(goalPosition);
        }
    }
}
