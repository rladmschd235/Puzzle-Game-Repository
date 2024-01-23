using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;         // ���� Ÿ�� ������
    [SerializeField] private Transform tileParent;          // Ÿ���� ��ġ�Ǵ� "Board" ������Ʈ�� Transform

    private List<Tile> tileList;                            // ���� Ÿ�� ���� ����
    
    private Vector2Int puzzleSize = new Vector2Int(4, 4);   // 4X4 ����
    private float neighborTileDistance = 102;               // ������ Ÿ�� ������ �Ÿ�. ������ ����� ���� �ִ�.
    
    public Vector3 EmptyTilePostion { set; get; }           // �� Ÿ���� ��ġ
    public int Playtime { private set; get; } = 0;          // ���� �÷��� �ð�
    public int MoveCount { private set; get; } = 0;         // �̵� Ƚ��

    private IEnumerator Start()
    {
        tileList = new List<Tile>();
        SpawnTiles();

        // ���� ��ġ�� �ٲ��� �ʱ� ������ ���̾ƿ��� ��ġ�� ���� ��ġ�� �籸��
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(tileParent.GetComponent<RectTransform>());

        // ���� �������� ����� ������ ���
        yield return new WaitForEndOfFrame();

        // tileList�� �ִ� ��� SetCorrectPosition() �޼ҵ� ȣ��
        tileList.ForEach(x => x.SetCorrectPosition());

        StartCoroutine("OnSuffle");
        // ���� ���۰� ���ÿ� �÷��� Ÿ�� �� ���� ����
        StartCoroutine("CalculatePlayTime");
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
            tileList[index].transform.SetAsLastSibling(); // ������ Ÿ���� ������ �ڽ����� ���� -> ��ġ ����

            yield return null;
        }

        // ���� ���� ����� �ٸ� ����̾��µ� UI, GridLayoutGroup�� ����ϴٺ��� �ڽ��� ��ġ�� �ٲٴ� ������ ����
        // �׷��� ���� Ÿ�ϸ���Ʈ�� �������� �ִ� ��Ұ� ������ �� Ÿ��
        EmptyTilePostion = tileList[tileList.Count - 1].GetComponent<RectTransform>().localPosition;
    }

    public void IsMoveTile(Tile tile)
    {
        if(Vector3.Distance(EmptyTilePostion, tile.GetComponent<RectTransform>().localPosition) == neighborTileDistance)
        {
            Vector3 goalPosition = EmptyTilePostion; // Ÿ���� �̵��� ��ġ ����

            EmptyTilePostion = tile.GetComponent<RectTransform>().localPosition; // Ÿ���� �� ��ġ ����

            tile.OnMoveTo(goalPosition);

            // Ÿ���� �̵��� ������ �̵� Ƚ�� ����
            MoveCount++;
        }
    }

    public void IsGameOver()
    {
        List<Tile> tiles = tileList.FindAll(x => x.IsCorrected == true); // ���ǰ� ��ġ�ϴ� ��� ��� ��ȯ

        Debug.Log("Correct Count : " + tiles.Count);

        if (tiles.Count == puzzleSize.x * puzzleSize.y - 1)     
        {
            Debug.Log("Game Clear");
            // ���� Ŭ�������� �� �ð� ��� ����
            StopCoroutine("CalculatePlayTime");
            // Board ������Ʈ�� ������Ʈ�� �����ϱ� ������
            // �׸��� �� ���� ȣ���ϱ� ������ ������ ������ �ʴ´�
            GetComponent<UIController>().OnResultPanel();
        }
    }

    private IEnumerator CalculatePlayTime()
    {
        while(true)
        {
            Playtime++;
            yield return new WaitForSeconds(1f);
        }
    }
}
