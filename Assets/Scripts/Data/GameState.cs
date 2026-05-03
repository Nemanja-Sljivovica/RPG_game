using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public RunConfig Config;
    public HeroData Hero;
    public List<string> LearnedMoves = new List<string>();
    public List<string> EquippedMoves = new List<string>();
    public int CurrentEncounterIndex = 0;
    public int MaxEquipped = 4;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun(RunConfig config)
    {
        Config = config;
        Hero = config.hero;
        LearnedMoves = new List<string>(config.hero.moves);
        EquippedMoves = new List<string>(config.hero.moves);
        CurrentEncounterIndex = 0;
    }

    public MonsterData GetCurrentMonster()
    {
        if (Config == null || CurrentEncounterIndex >= Config.monsters.Count) return null;
        return Config.monsters[CurrentEncounterIndex];
    }

    public bool IsRunComplete()
    {
        return CurrentEncounterIndex >= Config.monsters.Count;
    }

    public void LearnRandomMoveFromMonster(MonsterData monster)
    {
        var unlearned = monster.moves.FindAll(m => !LearnedMoves.Contains(m));
        if (unlearned.Count == 0) return;
        string newMove = unlearned[Random.Range(0, unlearned.Count)];
        LearnedMoves.Add(newMove);
    }

    public void AwardXp(int amount)
    {
        Hero.xp += amount;
        while (Hero.level < Config.xp_curve.Count - 1 && Hero.xp >= Config.xp_curve[Hero.level])
        {
            Hero.level++;
            Hero.stats.health += Config.level_up_gains.health;
            Hero.stats.attack += Config.level_up_gains.attack;
            Hero.stats.defense += Config.level_up_gains.defense;
            Hero.stats.magic  += Config.level_up_gains.magic;
        }
    }
}