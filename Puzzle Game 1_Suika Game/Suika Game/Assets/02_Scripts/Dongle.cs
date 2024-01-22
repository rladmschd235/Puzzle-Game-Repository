using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public Rigidbody2D rigid;

    private CircleCollider2D circle;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float deadTime;

    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    private void OnDisable()
    {
        // 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        // 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;
    }

    void Update()
    {
        if (isDrag) // 드래그 상태일 때
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 커서 위치 저장

            float leftBorder = -4.2f + transform.localScale.x / 2; // 좌측 경계
            float rightBorder = 4.2f - transform.localScale.x / 2; // 우측 경계

            mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder); // x축 경계 제한
            mousePos.y = 8;
            mousePos.z = 0;

            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // 부드럽게 해주는 값은 0부터 1 사이의 값을 넣어야함
        }
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag= false;
        rigid.simulated = true; // 물리작용 활성화
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine("AttachRoutine");
    }

    IEnumerator AttachRoutine()
    {
        if(isAttach)
        {
            yield break;
        }

        isAttach = true;
        manager.SFXPlay(GameManager.SFX.Attach);

        yield return new WaitForSeconds(0.2f);

        isAttach = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle") // 충돌한 게임 오브젝트의 태그가 "Dongle"인가
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            // 합치기 로직 실행 조건: 충돌한 오브젝트 레벨 == 내 레벨, 둘 다 합치기 로직 실행 중 X, 레벨 7보다 작아야한다.
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {

                // 나와 상대편 위치 가져오기
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = collision.transform.position.x;
                float otherY = collision.transform.position.y;

                // 내가 아래에 있을 때 또는 동일한 높이일 때, 내가 오른쪽에 있을 때 
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    // 충돌 오브젝트 비활성화
                    other.Hide(transform.position);

                    // 현재 오브젝트 레벨 업
                    LevelUp();
                }
            }
        }
    }

    // 합쳐지는 오브젝트 기능을 비활성화시키는 함수
    public void Hide(Vector3 targetPos) 
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;

        if(targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }

    // 합쳐질 오브젝트를 레벨 업 오브젝트로 이동시키는 함수
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while(frameCount < 20)
        {
            frameCount++;

            if(targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            yield return null;
        }

        manager.score += (int)MathF.Pow(2, level); // Pow : 지정 숫자의 거듭 제곱

        isMerge = false;
        gameObject.SetActive(false); // 이동이 다 끝났다면 오브젝트 비활성화
    }

    private void LevelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero; // 물리속도 0으로 지정
        rigid.angularVelocity = 0; // 양수면 시계 방향 회전, 음수면 반시계 방향 회전

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1); // 레벨 업 된 애니메이션 출력
        EffectPlay(); // 이펙트 출력
        manager.SFXPlay(GameManager.SFX.LevelUp);   

        yield return new WaitForSeconds(0.3f);

        level++; // 현재 레벨 1 증가

        manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        isMerge = false;
    }

    private void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;

            if(deadTime > 2f)
            {
                spriteRenderer.color = Color.red;
            }
            if(deadTime > 5f)
            {
                manager.GameOver();     
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}
