using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }
    public string BaseUrl = "http://localhost:8000/api";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator GetRunConfig(Action<RunConfig> onSuccess, Action<string> onError)
    {
        string url = $"{BaseUrl}/run/config/";
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                yield break;
            }
            try
            {
                var config = JsonConvert.DeserializeObject<RunConfig>(req.downloadHandler.text);
                onSuccess?.Invoke(config);
            }
            catch (Exception e) { onError?.Invoke(e.Message); }
        }
    }

    public IEnumerator GetMonsterMove(string monsterId, float monsterHpPct, float heroHpPct,
                                       Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{BaseUrl}/battle/monster-move/?monster_id={monsterId}" +
                     $"&monster_hp_pct={monsterHpPct}&hero_hp_pct={heroHpPct}";
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                yield break;
            }
            try
            {
                var resp = JsonConvert.DeserializeObject<MonsterMoveResponse>(req.downloadHandler.text);
                onSuccess?.Invoke(resp.move_id);
            }
            catch (Exception e) { onError?.Invoke(e.Message); }
        }
    }
}