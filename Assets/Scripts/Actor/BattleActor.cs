using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BattleActor
{
    #region 静态属性
    public string name => "Actor";
    public int uuid { get; private set; }
    public int ActorId { get; private set; }
    public int ActorType { get; private set; }
    public int ActorDimension { get; private set; }
    public int SpecType { get; private set; }
    public float HeighOffset { get; private set; }
    public int[] AreaSize { get; private set; }
    public int Upgrade { get; private set; }
    public int Input { get; private set; }
    public int Feature { get; private set; }
    public int AIId { get; private set; }
    public int layer { get; private set; }
    public bool enabled { get; set; }
    public int tag { get; private set; }
    public int faction { get; private set; }
    public int attackType { get; private set; }
    public int localComponentType { get; private set; }
    public float ActorHeight { get; private set; }
    public int WayFindModel { set; get; }
    public int CommonSkillId { get; private set; }
    public int targetType { get; private set; } // 可攻击的单位类型标识（单位，建筑）
    public int lockType { get; private set; } // 锁定势力标识
    public int lockSort { get; private set; }// 锁定排序
    public int attackDimension { get; private set; }// 可攻击的单位空地标识（地面，空中）
    public float rawMoveSpeed { get; private set; }

    #endregion


    #region 动态属性
    public bool IsDead
    {
        get
        {
            return false;
        }
    }
    public float ActorRadius
    {
        get
        {
            return 1;
        }
    }

    public float DetectRadius
    {
        get
        {
            return 1;
        }
    }

    public float MoveSpeed
    {
        get
        {
            return 1;
        }
    }

    public float AlertRadius
    {
        get
        {
            return 1;
        }
    }

    public bool isEntity
    {
        get
        {
            return false;
        }
    }
    public int MaxHp;
    public int Hp;

    public int MaxMp;
    public int Mp // 魔法量
    {
        get { return 100; }
    }
    public int MaxSp;
    public int Shield;
    public int Sp //技能能量
    {
        get
        {
            return 1;
        }
    }

    #endregion
}
