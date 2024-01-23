using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI textPlayTime;
    [SerializeField] private TextMeshProUGUI textMoveCount;
    [SerializeField] private Board board;

    public void OnResultPanel()
    {
        resultPanel.SetActive(true);

        textPlayTime.text = $"PLAY TIME : {board.Playtime / 60:D2}:{board.Playtime % 60:D2}";
        textMoveCount.text = $"MOVE COUNT : {board.MoveCount}";
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 활성화 된 씬의 이름을 가져온다
    }
}
