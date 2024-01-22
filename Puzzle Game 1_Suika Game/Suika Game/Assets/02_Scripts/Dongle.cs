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
        // �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        // Ʈ������ �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;
    }

    void Update()
    {
        if (isDrag) // �巡�� ������ ��
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // ���콺 Ŀ�� ��ġ ����

            float leftBorder = -4.2f + transform.localScale.x / 2; // ���� ���
            float rightBorder = 4.2f - transform.localScale.x / 2; // ���� ���

            mousePos.x = Mathf.Clamp(mousePos.x, leftBorder, rightBorder); // x�� ��� ����
            mousePos.y = 8;
            mousePos.z = 0;

            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // �ε巴�� ���ִ� ���� 0���� 1 ������ ���� �־����
        }
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag= false;
        rigid.simulated = true; // �����ۿ� Ȱ��ȭ
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
        if (collision.gameObject.tag == "Dongle") // �浹�� ���� ������Ʈ�� �±װ� "Dongle"�ΰ�
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            // ��ġ�� ���� ���� ����: �浹�� ������Ʈ ���� == �� ����, �� �� ��ġ�� ���� ���� �� X, ���� 7���� �۾ƾ��Ѵ�.
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {

                // ���� ����� ��ġ ��������
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = collision.transform.position.x;
                float otherY = collision.transform.position.y;

                // ���� �Ʒ��� ���� �� �Ǵ� ������ ������ ��, ���� �����ʿ� ���� �� 
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    // �浹 ������Ʈ ��Ȱ��ȭ
                    other.Hide(transform.position);

                    // ���� ������Ʈ ���� ��
                    LevelUp();
                }
            }
        }
    }

    // �������� ������Ʈ ����� ��Ȱ��ȭ��Ű�� �Լ�
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

    // ������ ������Ʈ�� ���� �� ������Ʈ�� �̵���Ű�� �Լ�
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

        manager.score += (int)MathF.Pow(2, level); // Pow : ���� ������ �ŵ� ����

        isMerge = false;
        gameObject.SetActive(false); // �̵��� �� �����ٸ� ������Ʈ ��Ȱ��ȭ
    }

    private void LevelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero; // �����ӵ� 0���� ����
        rigid.angularVelocity = 0; // ����� �ð� ���� ȸ��, ������ �ݽð� ���� ȸ��

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1); // ���� �� �� �ִϸ��̼� ���
        EffectPlay(); // ����Ʈ ���
        manager.SFXPlay(GameManager.SFX.LevelUp);   

        yield return new WaitForSeconds(0.3f);

        level++; // ���� ���� 1 ����

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
