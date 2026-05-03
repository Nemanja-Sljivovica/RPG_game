using System.Collections.Generic;

public class Combatant
{
    public string Name;
    public int MaxHp;
    public int CurrentHp;

    // Effective stats = base stats + active modifiers
    public int BaseAttack;
    public int BaseDefense;
    public int BaseMagic;

    // Active stat changes (positive = buff, negative = debuff)
    public int AttackMod;
    public int DefenseMod;
    public int MagicMod;

    // Turns remaining for each modifier
    public int AttackModTurns;
    public int DefenseModTurns;
    public int MagicModTurns;

    public int EffectiveAttack => System.Math.Max(0, BaseAttack + AttackMod);
    public int EffectiveDefense => System.Math.Max(0, BaseDefense + DefenseMod);
    public int EffectiveMagic => System.Math.Max(0, BaseMagic + MagicMod);

    public bool IsDead => CurrentHp <= 0;
    public float HpPct => MaxHp > 0 ? (float)CurrentHp / MaxHp : 0f;

    public void TickModifiers()
    {
        if (AttackModTurns > 0) { AttackModTurns--; if (AttackModTurns == 0) AttackMod = 0; }
        if (DefenseModTurns > 0) { DefenseModTurns--; if (DefenseModTurns == 0) DefenseMod = 0; }
        if (MagicModTurns > 0) { MagicModTurns--; if (MagicModTurns == 0) MagicMod = 0; }
    }
}