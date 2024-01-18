using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Dongle> donglePool;
    public List<ParticleSystem> effectPool;
    
    [Range(1, 30)]
    public int poolSize;
    public int poolCusor;

    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

    public GameObject effectPrefab;
    public Transform effectGroup;

    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum SFX { LevelUp, Next, Attach, Button, Over };
    private int sfxCursor;

    public int score;
    public int maxLevel;
    public bool isOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        // 풀 리스트 초기화
        donglePool = new List<Dongle>();
        effectPool = new List<ParticleSystem>();

        for(int i = 0; i < poolSize; i++)
        {
            MakeDongle();
        }
    }

    private void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }

    private Dongle MakeDongle()
    {
        // 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        // 머지 오브젝트 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        instantDongleObj.name = "Dongle " + effectPool.Count;
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        
        instantDongle.manager = this;
        instantDongle.effect = instantEffect;

        donglePool.Add(instantDongle);

        return instantDongle;
    }

    private Dongle GetDongle()
    {
        for (int i = 0; i < donglePool.Count; i++)
        {
            poolCusor = (poolCusor + 1) % donglePool.Count;
            if(!donglePool[poolCusor].gameObject.activeSelf) // 현재 가리키는 풀 오브젝트가 비활성화되어있는가
            {
                return donglePool[poolCusor];
            }
        }

        return MakeDongle(); // 모든 풀 오브젝트가 활성화 상태라면 풀 오브젝트 생성 함수 반환
    }

    private void NextDongle()
    {
        if(isOver)
        {
            return;
        }

        lastDongle = GetDongle();
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);

        SFXPlay(SFX.Next);
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

        yield return new WaitForSeconds(1f);
        SFXPlay(SFX.Over);
    }

    public void SFXPlay(SFX type)
    {
        switch (type)
        {
            case SFX.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case SFX.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case SFX.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case SFX.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case SFX.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }
}