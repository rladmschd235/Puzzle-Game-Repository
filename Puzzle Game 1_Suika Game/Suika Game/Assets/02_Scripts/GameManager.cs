using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

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
        GameObject instant = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instant.GetComponent<Dongle>();
        return instantDongle;
    }

    private void NextDongle()
    {
        Dongle newDongle =  GetDongle();
        lastDongle = newDongle;
        lastDongle.level = Random.Range(0, 8);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine("WaitNext");
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
