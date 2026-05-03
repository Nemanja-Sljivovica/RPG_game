using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public Transform EncounterListParent;
    public Button EncounterSlotTemplate;
    public Image HeroPortrait;
    public TMP_Text HeroStatsText;
    public Button EnterBattleButton;
    public Button ManageMovesButton;
    public Button BackToMenuButton;
    public TMP_Text MapStatusText;

    private List<Button> encounterSlots = new List<Button>();

    void Start()
    {
        if (GameState.Instance == null || GameState.Instance.Config == null)
        {
            if (MapStatusText != null) MapStatusText.text = "No active run. Returning to menu...";
            SceneManager.LoadScene("MainMenu");
            return;
        }

        BuildEncounterList();
        UpdateUI();

        EnterBattleButton.onClick.AddListener(OnEnterBattle);
        ManageMovesButton.onClick.AddListener(OnManageMoves);
        BackToMenuButton.onClick.AddListener(OnBackToMenu);
    }

    void BuildEncounterList()
    {
        EncounterSlotTemplate.gameObject.SetActive(false);

        var monsters = GameState.Instance.Config.monsters;
        for (int i = 0; i < monsters.Count; i++)
        {
            var slot = Instantiate(EncounterSlotTemplate, EncounterListParent);
            slot.gameObject.SetActive(true);
            slot.name = $"Encounter_{i}_{monsters[i].name}";

            // Set label
            var label = slot.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = $"{i + 1}\n{monsters[i].name}";

            // Set sprite
           var icon = slot.transform.Find("MonsterIcon")?.GetComponent<Image>();
            Debug.Log($"Slot {i}: monster id='{monsters[i].id}', icon found={icon != null}, registry exists={SpriteRegistry.Instance != null}");

            if (icon != null && SpriteRegistry.Instance != null)
            {
                var sprite = SpriteRegistry.Instance.GetMonsterSprite(monsters[i].id);
                Debug.Log($"Slot {i}: sprite lookup result={sprite != null}, sprite name='{sprite?.name}'");
                if (sprite != null)
                {
                    icon.sprite = sprite;
                    icon.color = Color.white;
                }
            }

            int captured = i;
            slot.onClick.AddListener(() => OnSlotClicked(captured));
            encounterSlots.Add(slot);
        }
    }

    void UpdateUI()
    {
        var state = GameState.Instance;
        var hero = state.Hero;
        int idx = state.CurrentEncounterIndex;

        // Hero portrait
        if (HeroPortrait != null && SpriteRegistry.Instance != null && SpriteRegistry.Instance.HeroSprite != null)
        {
            HeroPortrait.sprite = SpriteRegistry.Instance.HeroSprite;
            HeroPortrait.preserveAspect = true;
        }

        // Hero stats text
        if (HeroStatsText != null)
        {
            HeroStatsText.text =
                $"<b>{hero.name}</b>  Lv {hero.level}\n" +
                $"HP {hero.stats.health}  ATK {hero.stats.attack}\n" +
                $"DEF {hero.stats.defense}  MAG {hero.stats.magic}\n" +
                $"XP {hero.xp}\n" +
                $"Equipped: {string.Join(", ", state.EquippedMoves)}";
        }

        // Encounter slot tinting
        for (int i = 0; i < encounterSlots.Count; i++)
        {
             var img = encounterSlots[i].GetComponent<Image>();
            if (i < idx) img.color = new Color(0.6f, 1f, 0.6f);          // beaten = light green
            else if (i == idx) img.color = new Color(1f, 0.95f, 0.6f);   // current = light yellow
            else img.color = new Color(0.7f, 0.7f, 0.7f);                // future = gray
        }

        if (state.IsRunComplete())
        {
            MapStatusText.text = $"Run complete! All 5 monsters defeated.";
            EnterBattleButton.interactable = false;
        }
        else
        {
            var monster = state.GetCurrentMonster();
            MapStatusText.text = $"Next encounter: {monster.name}\n<size=20>(Click any beaten monster to replay for XP and try to learn another move)</size>";
            EnterBattleButton.interactable = state.EquippedMoves.Count > 0;
        }
    }

    void OnSlotClicked(int idx)
    {
        var state = GameState.Instance;
        if (idx == state.CurrentEncounterIndex)
        {
             OnEnterBattle();
        }
        else if (idx < state.CurrentEncounterIndex)
         {
        // Replay an already-beaten fight (grind XP, try to learn another move)
            StartCoroutine(EnterReplayBattle(idx));
        }
    // future encounters (idx > current) do nothing
    }

    System.Collections.IEnumerator EnterReplayBattle(int idx)
    {
        GameState.Instance.ReplayMonsterIndex = idx;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        yield break;
    }

    void OnEnterBattle()
    {
        if (GameState.Instance.IsRunComplete()) return;
        SceneManager.LoadScene("Battle");
    }

    void OnManageMoves()
    {
        SceneManager.LoadScene("MoveManagement");
    }

    void OnBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}