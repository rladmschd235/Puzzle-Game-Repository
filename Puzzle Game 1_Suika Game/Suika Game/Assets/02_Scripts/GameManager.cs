using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Core")]
    public bool isOver;
    public int score;
    public int maxLevel;

    [Header("Pooling")]
    public List<Dongle> donglePool;
    public List<ParticleSystem> effectPool;
    
    [Range(1, 30)]
    public int poolSize;
    public int poolCusor;

    [Header("Object")]
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

    [Header("Effect")]
    public GameObject effectPrefab;
    public Transform effectGroup;

    [Header("Sound")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum SFX { LevelUp, Next, Attach, Button, Over };
    private int sfxCursor;

    [Header("GUI")]
    public GameObject startGroup;
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;

    [Header("ETC")]
    public GameObject line;
    public GameObject bottom;

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

        if(!PlayerPrefs.HasKey("maxScore"))
        {
            PlayerPrefs.SetInt("maxScore", 0);
        }
        
        maxScoreText.text = PlayerPrefs.GetInt("maxScore").ToString();
    }

    public void GameStart()
    {
        // 오브젝트 활성화
        line.SetActive(true);
        bottom.SetActive(true);

        // 스코어 활성화
        scoreText.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);

        // 시작화면 비활성화
        startGroup.SetActive(false);

        bgmPlayer.Play();
        SFXPlay(SFX.Button);

        // 게임 시작: 머지 오브젝트 생성
        Invoke("NextDongle", 1.5f);
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

        // 최고 점수 갱신
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("maxScore"));
        PlayerPrefs.SetInt("maxScore", maxScore);

        // 게임 오버 UI 표시
        subScoreText.text = "점수 : "+ scoreText.text;
        endGroup.SetActive(true);

        bgmPlayer.Stop();
        SFXPlay(SFX.Over);
    }

    public void Reset()
    {
        SFXPlay(SFX.Button);
        StartCoroutine("ResetCoroutine");
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("PlayScene");
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

    private void Update()
    {
        if(Input.GetButtonDown("Cancle"))
        {
            Application.Quit();
        }
    }

    private void LateUpdate()
    {
        scoreText.text = score.ToString();
    }
}