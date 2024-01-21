using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab; // ���� Ÿ�� ������
    [SerializeField] private Transform tileParent; // Ÿ���� ��ġ�Ǵ� "Board" ������Ʈ�� Transform

    private Vector2Int puzzleSize = new Vector2Int(4, 4); // 4X4 ����

    private void Start()
    {
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        for (int y = 0; y < puzzleSize.y; ++y)
        {
            for (int x = 0; x < puzzleSize.x; ++x)
            {
                Instantiate(tilePrefab, tileParent);
            }
        }
    }
}
