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
    
    public int maxLevel;

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
}
