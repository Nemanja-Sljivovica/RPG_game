using System.Collections.Generic;
using UnityEngine;

public class SpriteRegistry : MonoBehaviour
{
    public static SpriteRegistry Instance { get; private set; }

    public List<SpriteEntry> MonsterSprites;
    public Sprite HeroSprite;

    private Dictionary<string, Sprite> lookup;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        lookup = new Dictionary<string, Sprite>();
        foreach (var entry in MonsterSprites)
        {
            if (!string.IsNullOrEmpty(entry.id) && entry.sprite != null)
                lookup[entry.id] = entry.sprite;
        }
    }

    public Sprite GetMonsterSprite(string id)
    {
        if (lookup != null && lookup.TryGetValue(id, out var s)) return s;
        return null;
    }
}