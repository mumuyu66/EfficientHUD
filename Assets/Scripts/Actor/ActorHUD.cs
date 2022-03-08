using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorHUD:MonoBehaviour
{
    public Camera Camera;
    public Camera UICamera;

    public MeshImage HPBar;
    public MeshImage MPBar;
    public MeshImage HPBotBar;
    public MeshImage ShieldBar;

    public GameObject HPObj;
    public GameObject MPObj;

    public Sprite RedSpr; 
    public Sprite RedBotSpr;
    public Sprite GreenSpr;
    public Sprite GreenBotSpr;

    private Tweener hpTween;

    private int MaxHp;
    private int Hp;
    private int MaxMp;
    private int Mp;
    private int Shield;
    private int Faction;

    private bool IsRecycle = false;

    public BattleActor Actor { get; private set; }

    public void ActorBuild(BattleActor actor)
    {
        this.Actor = actor;
    }

    public void ActorStart()
    {
        MaxHp = Actor.MaxHp;
        Hp = Actor.Hp;
        MaxMp = Actor.MaxMp;
        Mp = Actor.Mp;
        Shield = Actor.Shield;
        Faction = Actor.faction;
        Init();
    }

    public void Init()
    {
        UpdateSize();
        UpdateFaction();
    }

    // 更新size
    public void UpdateSize()
    {
        HPObj.GetComponent<RectTransform>().sizeDelta = new Vector2(68f, 14f);
        HPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(66f, 10f);
        HPBotBar.GetComponent<RectTransform>().sizeDelta = new Vector2(66f, 10f);
        ShieldBar.GetComponent<RectTransform>().sizeDelta = new Vector2(66f, 10f);
        MPBar.GetComponent<RectTransform>().sizeDelta = new Vector2(68f, 5f);
        MPObj.GetComponent<RectTransform>().sizeDelta = new Vector2(68f, 6f);
    }

    // 更新阵营
    public void UpdateFaction()
    {
        if (Faction > 0)
        {
            HPBar.sprite = GreenSpr;
            HPBotBar.sprite = GreenBotSpr;
        }
        else
        {
            HPBar.sprite = RedSpr;
            HPBotBar.sprite = RedBotSpr;
        }
    }

    // 逻辑帧
    public void ActorLogicUpdate()
    {
        if (Hp != Actor.Hp || MaxHp != Actor.MaxHp || Actor.Shield != Shield)
        {
            UpdateHP();
        }
        if (Mp != Actor.Mp || MaxMp != Actor.MaxMp)
        {
            UpdateMp();
        }
    }
    private Vector2 outPos;
    //渲染帧
    public void ActorDisplayUpdate()
    {
        if (Actor != null)
        {
            Vector3 v = Camera.WorldToScreenPoint(Actor.Position);
            
            transform.localPosition = UICamera.ScreenToWorldPoint(v);
        }
    }

    public void Update()
    {
        ActorLogicUpdate();
        ActorDisplayUpdate();
    }

    public void UpdateHP()
    {
        if (Actor != null && !IsRecycle)
        {
            if (Actor.Shield > 0)
            {
                int hp = Actor.Hp;
                int shield = Actor.Shield;
                int sunHp = hp + shield;
                int maxHp = Math.Max(sunHp, Actor.MaxHp);
                TweenShield(sunHp, hp, maxHp);
            }
            else
            {
                TweenHp(Math.Max(0.02f, Actor.Hp / Actor.MaxHp));
            }
            Hp = Actor.Hp;
            MaxHp = Actor.MaxHp;
        }
    }

    public void UpdateMp()
    {
        TweenMp(Math.Max(0.02f,Actor.Mp / Actor.MaxMp));
        Mp = Actor.Mp;
        MaxMp = Actor.MaxMp;
    }

    private void TweenShield(float shield, float hp, float maxHP)
    {
        HPBar.fillAmount = hp / maxHP;
        ShieldBar.fillAmount = shield / maxHP;

        if (hpTween != null)
        {
            hpTween.Complete(true);
        }

        if (shield <= hp)
        {
            ShieldBar.fillAmount = 0;
            hpTween = HPBotBar.DOFillAmount(shield / maxHP, 0.02f).SetEase(Ease.OutCirc);
        }
        else
        {
            hpTween = HPBotBar.DOFillAmount(shield / maxHP, 0.8f).SetEase(Ease.OutCirc);
        }
    }

    private void TweenHp(float value)
    {
        if (HPObj.activeSelf == false && value > 0)
        {
            HPObj.SetActive(true);
        }

        HPBar.fillAmount = value;

        if (hpTween != null)
        {
            hpTween.Complete(true);
        }

        if (value <= 0)
        {
            HPBotBar.fillAmount = 0;
            //hpTween = HPBotBar.DOFillAmount(value, 0.02f).SetEase(Ease.OutCirc).OnComplete(() => { Recycle(mUid); });
        }
        else
        {
            hpTween = HPBotBar.DOFillAmount(value, 0.8f).SetEase(Ease.OutCirc);
        }
    }

    private void TweenMp(float value)
    {
        if (value <= MPBar.fillAmount)
        {
            MPBar.DOFillAmount(value, 0.2f).SetEase(Ease.OutCirc);
        }
        else
        {
            MPBar.DOFillAmount(value, 0.1f).SetEase(Ease.OutCirc);
        }
    }

    // 单位死亡
    public void ActorEnd()
    {
        
    }
    // 回收
    public void ActorRelease()
    {
        Actor = null;
    }
}
