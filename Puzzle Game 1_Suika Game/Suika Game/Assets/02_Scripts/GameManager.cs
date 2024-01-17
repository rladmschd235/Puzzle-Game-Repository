using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int score;
    public int maxLevel;
    public bool isOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        NextDongle();
    }

    private Dongle GetDongle()
    {
        // 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        // 머지 오브젝트 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        // 이펙트 할당
        instantDongle.effect = instantEffect;
        return instantDongle;
    }

    private void NextDongle()
    {
        if(isOver)
        {
            return;
        }

        Dongle newDongle =  GetDongle();
        lastDongle = newDongle;
        lastDongle.manager = this;
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (lastDongle != null)
        {
            yield return null; // 한 프레임 대기
        }

        yield return new WaitForSeconds(2.5f);

        NextDongle();
    }
    
    public void TouchDown()
    {
        if (lastDongle == null) return;

        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null) return;


        lastDongle.Drop();
        lastDongle = null;
    }

    public void GameOver()
    {
        if(isOver)
        {
            return;
        }
        else
        {
            isOver = true;
        }
        
        Debug.Log("Game Over");

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        // 1. 장면 안에 활성화 되어있는 모든 머지 오브젝트 가져오기
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        // 2. 지우기 전에 모든 머지 오브젝트 물리 효과 비활성화
        foreach (Dongle dongle in dongles)
        {
            dongle.rigid.simulated = false;
        }

        // 3. 1번의 목록을 하나씩 접근해서 제거하기
        foreach (Dongle dongle in dongles)
        {
            dongle.Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
