import random
from django.http import JsonResponse
from .config import HERO_BASE, LEVEL_UP_GAINS, XP_CURVE, MOVES, MONSTERS


def run_config(request):
    """GET /api/run/config/ — called once at run start."""
    return JsonResponse({
        "hero": HERO_BASE,
        "level_up_gains": LEVEL_UP_GAINS,
        "xp_curve": XP_CURVE,
        "moves": MOVES,
        "monsters": MONSTERS,
    })


def monster_move(request):
    """GET /api/battle/monster-move/?monster_id=X&monster_hp_pct=Y&hero_hp_pct=Z"""
    monster_id = request.GET.get("monster_id")
    monster = next((m for m in MONSTERS if m["id"] == monster_id), None)
    if not monster:
        return JsonResponse({"error": "unknown monster"}, status=400)

    try:
        monster_hp_pct = float(request.GET.get("monster_hp_pct", 1.0))
        hero_hp_pct = float(request.GET.get("hero_hp_pct", 1.0))
    except ValueError:
        monster_hp_pct = 1.0
        hero_hp_pct = 1.0

    available = monster["moves"]
    weights = []
    for move_id in available:
        move = MOVES[move_id]
        weight = 1.0
        if move["effect"] == "damage" and hero_hp_pct < 0.3:
            weight = 2.5
        if "buff" in move["effect"] and monster_hp_pct < 0.4:
            weight = 2.0
        weights.append(weight)

    chosen = random.choices(available, weights=weights, k=1)[0]
    return JsonResponse({"move_id": chosen})