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
        // ����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        // ���� ������Ʈ ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        // ����Ʈ �Ҵ�
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
            yield return null; // �� ������ ���
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
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ������Ʈ ��������
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        // 2. ����� ���� ��� ���� ������Ʈ ���� ȿ�� ��Ȱ��ȭ
        foreach (Dongle dongle in dongles)
        {
            dongle.rigid.simulated = false;
        }

        // 3. 1���� ����� �ϳ��� �����ؼ� �����ϱ�
        foreach (Dongle dongle in dongles)
        {
            dongle.Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
