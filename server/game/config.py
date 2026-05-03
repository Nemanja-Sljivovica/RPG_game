HERO_BASE = {
    "name": "Knight",
    "level": 1,
    "xp": 0,
    "stats": {"health": 100, "attack": 15, "defense": 10, "magic": 10},
    "moves": ["slash", "shield_up", "battle_cry", "second_wind"],
}

LEVEL_UP_GAINS = {"health": 15, "attack": 3, "defense": 2, "magic": 3}

XP_CURVE = [0, 100, 250, 500, 1000, 2000, 4000]

MOVES = {
    "slash":        {"name": "Slash",        "type": "physical", "effect": "damage", "power": 20, "input": "wwww"},
    "shield_up":    {"name": "Shield Up",    "type": "physical", "effect": "buff_defense_self", "power": 5, "duration": 2, "input": "ssss"},
    "battle_cry":   {"name": "Battle Cry",   "type": "physical", "effect": "buff_attack_self", "power": 5, "duration": 2, "input": "dddd"},
    "second_wind":  {"name": "Second Wind",  "type": "magic",    "effect": "heal_self", "power": 25, "input": "aaaa"},

    "shadow_bolt":  {"name": "Shadow Bolt",  "type": "magic",    "effect": "damage", "power": 30},
    "drain_life":   {"name": "Drain Life",   "type": "magic",    "effect": "drain", "power": 12},
    "curse":        {"name": "Curse",        "type": "magic",    "effect": "debuff_attack_target", "power": 4, "duration": 2},
    "dark_pact":    {"name": "Dark Pact",    "type": "magic",    "effect": "buff_magic_self_costhp", "power": 6, "hp_cost": 10, "duration": 2},

    "bite":         {"name": "Bite",         "type": "physical", "effect": "damage", "power": 18},
    "web_throw":    {"name": "Web Throw",    "type": "physical", "effect": "damage_debuff_defense", "power": 8, "debuff": 4, "duration": 2},
    "pounce":       {"name": "Pounce",       "type": "physical", "effect": "damage", "power": 28},
    "skitter":      {"name": "Skitter",      "type": "physical", "effect": "buff_defense_self", "power": 5, "duration": 2},

    "flame_breath": {"name": "Flame Breath", "type": "magic",    "effect": "damage", "power": 32},
    "claw_swipe":   {"name": "Claw Swipe",   "type": "physical", "effect": "damage", "power": 20},
    "intimidate":   {"name": "Intimidate",   "type": "physical", "effect": "debuff_attack_target", "power": 5, "duration": 2},
    "dragon_scales":{"name": "Dragon Scales","type": "physical", "effect": "buff_defense_self", "power": 6, "duration": 2},

    "rusty_blade":  {"name": "Rusty Blade",  "type": "physical", "effect": "damage", "power": 18},
    "dirty_kick":   {"name": "Dirty Kick",   "type": "physical", "effect": "damage_debuff_defense", "power": 8, "debuff": 4, "duration": 2},
    "frenzy":       {"name": "Frenzy",       "type": "physical", "effect": "buff_attack_self", "power": 5, "duration": 2},
    "headbutt":     {"name": "Headbutt",     "type": "physical", "effect": "damage", "power": 26},

    "firebolt":     {"name": "Firebolt",     "type": "magic",    "effect": "damage", "power": 22},
    "arcane_surge": {"name": "Arcane Surge", "type": "magic",    "effect": "buff_magic_self", "power": 5, "duration": 2},
    "mana_drain":   {"name": "Mana Drain",   "type": "magic",    "effect": "damage_debuff_magic", "power": 8, "debuff": 4, "duration": 2},
    "hex_shield":   {"name": "Hex Shield",   "type": "magic",    "effect": "buff_defense_self", "power": 5, "duration": 2},
}

MONSTERS = [
    {"id": "goblin_warrior", "name": "Goblin Warrior",
     "stats": {"health": 60, "attack": 12, "defense": 8, "magic": 5},
     "moves": ["rusty_blade", "dirty_kick", "frenzy", "headbutt"],
     "xp_reward": 50},

    {"id": "giant_spider", "name": "Giant Spider",
     "stats": {"health": 80, "attack": 14, "defense": 10, "magic": 5},
     "moves": ["bite", "web_throw", "pounce", "skitter"],
     "xp_reward": 100},

    {"id": "goblin_mage", "name": "Goblin Mage",
     "stats": {"health": 90, "attack": 8, "defense": 8, "magic": 18},
     "moves": ["firebolt", "arcane_surge", "mana_drain", "hex_shield"],
     "xp_reward": 175},

    {"id": "witch", "name": "Witch",
     "stats": {"health": 110, "attack": 8, "defense": 10, "magic": 22},
     "moves": ["shadow_bolt", "drain_life", "curse", "dark_pact"],
     "xp_reward": 275},

    {"id": "dragon", "name": "Dragon",
     "stats": {"health": 160, "attack": 18, "defense": 14, "magic": 18},
     "moves": ["flame_breath", "claw_swipe", "intimidate", "dragon_scales"],
     "xp_reward": 400},
]