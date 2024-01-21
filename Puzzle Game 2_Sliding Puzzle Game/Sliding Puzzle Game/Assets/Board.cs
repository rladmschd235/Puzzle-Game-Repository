using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab; // 숫자 타일 프리팹
    [SerializeField] private Transform tileParent; // 타일이 배치되는 "Board" 오브젝트의 Transform

    private Vector2Int puzzleSize = new Vector2Int(4, 4); // 4X4 퍼즐

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
