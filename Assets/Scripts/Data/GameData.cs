using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SpriteEntry
{
    public string id;      
    public Sprite sprite;
}
[Serializable]
public class Stats
{
    public int health;
    public int attack;
    public int defense;
    public int magic;
}

[Serializable]
public class HeroData
{
    public string name;
    public int level;
    public int xp;
    public Stats stats;
    public List<string> moves;
}

[Serializable]
public class MoveData
{
    public string name;
    public string type;
    public string effect;
    public int power;
    public int duration;
    public int debuff;
    public int hp_cost;
    public string input;
}

[Serializable]
public class MonsterData
{
    public string id;
    public string name;
    public Stats stats;
    public List<string> moves;
    public int xp_reward;
}

[Serializable]
public class RunConfig
{
    public HeroData hero;
    public Stats level_up_gains;
    public List<int> xp_curve;
    public Dictionary<string, MoveData> moves;
    public List<MonsterData> monsters;
}

[Serializable]
public class MonsterMoveResponse
{
    public string move_id;
}