using UnityEngine;

public static class MoveResolver
{
    public class Result
    {
        public string Description;
        public int DamageDealt;
        public int HealAmount;
    }

    public static Result Apply(MoveData move, Combatant attacker, Combatant target)
    {
        var r = new Result();
        if (move == null) { r.Description = $"{attacker.Name} did nothing."; return r; }

        switch (move.effect)
        {
            case "damage":
                r.DamageDealt = ComputeDamage(move, attacker, target);
                target.CurrentHp = Mathf.Max(0, target.CurrentHp - r.DamageDealt);
                r.Description = $"{attacker.Name} used {move.name} for {r.DamageDealt} damage.";
                break;

            case "heal_self":
                r.HealAmount = move.power + attacker.EffectiveMagic / 2;
                attacker.CurrentHp = Mathf.Min(attacker.MaxHp, attacker.CurrentHp + r.HealAmount);
                r.Description = $"{attacker.Name} used {move.name}, healed {r.HealAmount} HP.";
                break;

            case "drain":
                r.DamageDealt = ComputeDamage(move, attacker, target);
                target.CurrentHp = Mathf.Max(0, target.CurrentHp - r.DamageDealt);
                attacker.CurrentHp = Mathf.Min(attacker.MaxHp, attacker.CurrentHp + r.DamageDealt);
                r.Description = $"{attacker.Name} used {move.name}, drained {r.DamageDealt} HP.";
                break;

            case "buff_defense_self":
                attacker.DefenseMod = move.power;
                attacker.DefenseModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name}, defense up.";
                break;

            case "buff_attack_self":
                attacker.AttackMod = move.power;
                attacker.AttackModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name}, attack up.";
                break;

            case "buff_magic_self":
                attacker.MagicMod = move.power;
                attacker.MagicModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name}, magic up.";
                break;

            case "buff_magic_self_costhp":
                attacker.MagicMod = move.power;
                attacker.MagicModTurns = move.duration;
                int cost = move.hp_cost > 0 ? move.hp_cost : 10;
                attacker.CurrentHp = Mathf.Max(1, attacker.CurrentHp - cost);
                r.Description = $"{attacker.Name} used {move.name}, magic up (paid {cost} HP).";
                break;

            case "debuff_attack_target":
                target.AttackMod = -move.power;
                target.AttackModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name}, lowered {target.Name}'s attack.";
                break;

            case "debuff_defense_target":
                target.DefenseMod = -move.power;
                target.DefenseModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name}, lowered {target.Name}'s defense.";
                break;

            case "damage_debuff_defense":
                r.DamageDealt = ComputeDamage(move, attacker, target);
                target.CurrentHp = Mathf.Max(0, target.CurrentHp - r.DamageDealt);
                target.DefenseMod = -(move.debuff > 0 ? move.debuff : 4);
                target.DefenseModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name} for {r.DamageDealt} damage, lowered defense.";
                break;

            case "damage_debuff_magic":
                r.DamageDealt = ComputeDamage(move, attacker, target);
                target.CurrentHp = Mathf.Max(0, target.CurrentHp - r.DamageDealt);
                target.MagicMod = -(move.debuff > 0 ? move.debuff : 4);
                target.MagicModTurns = move.duration;
                r.Description = $"{attacker.Name} used {move.name} for {r.DamageDealt} damage, lowered magic.";
                break;

            default:
                r.Description = $"{attacker.Name} used {move.name}.";
                break;
        }
        return r;
    }

    static int ComputeDamage(MoveData move, Combatant attacker, Combatant target)
    {
        float scale;
        float defense;
        if (move.type == "magic")
        {
            scale = attacker.EffectiveMagic;
            defense = 0; // magic ignores defense per the brief
        }
        else
        {
            scale = attacker.EffectiveAttack;
            defense = target.EffectiveDefense;
        }
        float baseDmg = move.power + scale * 0.5f - defense * 0.5f;
        baseDmg *= Random.Range(0.9f, 1.1f);
        return Mathf.Max(1, Mathf.RoundToInt(baseDmg));
    }
}