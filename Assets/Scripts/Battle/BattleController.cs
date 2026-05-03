using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    [Header("Hero UI")]
    public Image HeroSprite;
    public TMP_Text HeroNameText;
    public Image HeroHpBarFill;
    public TMP_Text HeroHpText;

    [Header("Monster UI")]
    public Image MonsterSprite;
    public TMP_Text MonsterNameText;
    public Image MonsterHpBarFill;
    public TMP_Text MonsterHpText;

    [Header("Move UI")]
    public Transform MoveButtonsRow;
    public Button MoveButtonTemplate;
    public TMP_Text WasdDisplay;

    [Header("Log + Result")]
    public TMP_Text BattleLogText;
    public GameObject ResultPanel;
    public TMP_Text ResultText;
    public Button ResultButton;

    private Combatant hero;
    private Combatant monster;
    private MonsterData monsterData;
    private Dictionary<string, MoveData> moves;
    private List<string> equippedMoveIds;
    private List<Button> moveButtons = new List<Button>();
    private bool playerTurn = true;
    private bool battleOver = false;
    private string typedSequence = "";
    private Dictionary<string, string> wasdToMoveId = new Dictionary<string, string>();

    void Start()
    {
        if (GameState.Instance == null || GameState.Instance.Config == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        moves = GameState.Instance.Config.moves;
        equippedMoveIds = new List<string>(GameState.Instance.EquippedMoves);
        monsterData = GameState.Instance.GetCurrentMonster();
        if (monsterData == null) { SceneManager.LoadScene("Map"); return; }

        SetupCombatants();
        SetupSprites();
        BuildMoveButtons();
        UpdateHpUI();
        BattleLogText.text = $"A wild {monsterData.name} appears!";
        ResultPanel.SetActive(false);
        WasdDisplay.text = "";

        ResultButton.onClick.AddListener(OnResultButton);
    }

    void Update()
    {
        if (battleOver || !playerTurn) return;
        HandleWasdInput();
    }

    void SetupCombatants()
    {
        var h = GameState.Instance.Hero;
        hero = new Combatant
        {
            Name = h.name,
            MaxHp = h.stats.health,
            CurrentHp = h.stats.health,
            BaseAttack = h.stats.attack,
            BaseDefense = h.stats.defense,
            BaseMagic = h.stats.magic
        };

        monster = new Combatant
        {
            Name = monsterData.name,
            MaxHp = monsterData.stats.health,
            CurrentHp = monsterData.stats.health,
            BaseAttack = monsterData.stats.attack,
            BaseDefense = monsterData.stats.defense,
            BaseMagic = monsterData.stats.magic
        };
    }

    void SetupSprites()
    {
        if (SpriteRegistry.Instance != null)
        {
            if (SpriteRegistry.Instance.HeroSprite != null) HeroSprite.sprite = SpriteRegistry.Instance.HeroSprite;
            var ms = SpriteRegistry.Instance.GetMonsterSprite(monsterData.id);
            if (ms != null) MonsterSprite.sprite = ms;
        }
        HeroSprite.preserveAspect = true;
        MonsterSprite.preserveAspect = true;

        var hData = GameState.Instance.Hero;
        HeroNameText.text = $"{hero.Name}  Lv {hData.level}";
        MonsterNameText.text = monster.Name;
    }

    void BuildMoveButtons()
    {
        MoveButtonTemplate.gameObject.SetActive(false);
        wasdToMoveId.Clear();
        foreach (var moveId in equippedMoveIds)
        {
            if (!moves.ContainsKey(moveId)) continue;
            var moveData = moves[moveId];
            var btn = Instantiate(MoveButtonTemplate, MoveButtonsRow);
            btn.gameObject.SetActive(true);
            btn.name = $"MoveBtn_{moveId}";

            var label = btn.GetComponentInChildren<TMP_Text>();
            string typeTag = moveData.type == "magic" ? "<color=#a3c2ff>[M]</color>" : "<color=#ffb88a>[P]</color>";
            string inputTag = !string.IsNullOrEmpty(moveData.input) ? $"[{moveData.input.ToUpper()}]" : "";
            label.text = $"{inputTag}\n{typeTag} {moveData.name}";

            string capturedId = moveId;
            btn.onClick.AddListener(() => OnPlayerMove(capturedId));
            moveButtons.Add(btn);

            if (!string.IsNullOrEmpty(moveData.input))
                wasdToMoveId[moveData.input.ToLower()] = moveId;
        }
    }

    void HandleWasdInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) AppendKey("w");
        else if (Input.GetKeyDown(KeyCode.A)) AppendKey("a");
        else if (Input.GetKeyDown(KeyCode.S)) AppendKey("s");
        else if (Input.GetKeyDown(KeyCode.D)) AppendKey("d");
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            typedSequence = "";
            UpdateWasdDisplay();
        }
    }

    void AppendKey(string k)
    {
        typedSequence += k;
        if (typedSequence.Length > 4) typedSequence = typedSequence.Substring(typedSequence.Length - 4);
        UpdateWasdDisplay();

        if (typedSequence.Length == 4)
        {
            if (wasdToMoveId.TryGetValue(typedSequence, out var moveId))
            {
                typedSequence = "";
                UpdateWasdDisplay();
                OnPlayerMove(moveId);
            }
            else
            {
                BattleLogText.text = $"Unknown sequence: {typedSequence.ToUpper()}";
                typedSequence = "";
                StartCoroutine(ClearWasdDisplaySoon());
            }
        }
    }

    IEnumerator ClearWasdDisplaySoon()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateWasdDisplay();
    }

    void UpdateWasdDisplay()
    {
        var padded = typedSequence.PadRight(4, '_').ToUpper();
        WasdDisplay.text = $"> {string.Join(" ", padded.ToCharArray())}";
    }

    void OnPlayerMove(string moveId)
    {
        if (battleOver || !playerTurn) return;
        if (!moves.ContainsKey(moveId)) return;
        playerTurn = false;
        SetMoveButtonsInteractable(false);
        StartCoroutine(ResolvePlayerTurn(moveId));
    }

    IEnumerator ResolvePlayerTurn(string moveId)
    {
        var move = moves[moveId];
        var result = MoveResolver.Apply(move, hero, monster);
        BattleLogText.text = result.Description;
        UpdateHpUI();
        yield return new WaitForSeconds(1.0f);

        if (monster.IsDead) { EndBattle(true); yield break; }

        // Ask server for monster move
        bool gotMove = false;
        string monsterMoveId = null;
        yield return StartCoroutine(ApiClient.Instance.GetMonsterMove(
            monsterData.id, monster.HpPct, hero.HpPct,
            (id) => { monsterMoveId = id; gotMove = true; },
            (err) => { BattleLogText.text = $"Server error: {err}"; gotMove = true; }
        ));

        if (!string.IsNullOrEmpty(monsterMoveId) && moves.ContainsKey(monsterMoveId))
        {
            var monsterMove = moves[monsterMoveId];
            var monsterResult = MoveResolver.Apply(monsterMove, monster, hero);
            BattleLogText.text = monsterResult.Description;
            UpdateHpUI();
            yield return new WaitForSeconds(1.0f);
        }

        if (hero.IsDead) { EndBattle(false); yield break; }

        hero.TickModifiers();
        monster.TickModifiers();
        playerTurn = true;
        SetMoveButtonsInteractable(true);
    }

    void SetMoveButtonsInteractable(bool on)
    {
        foreach (var b in moveButtons) b.interactable = on;
    }

    void UpdateHpUI()
    {
        HeroHpBarFill.fillAmount = hero.HpPct;
        HeroHpText.text = $"{hero.CurrentHp} / {hero.MaxHp}";
        MonsterHpBarFill.fillAmount = monster.HpPct;
        MonsterHpText.text = $"{monster.CurrentHp} / {monster.MaxHp}";
    }

    void EndBattle(bool won)
    {
        battleOver = true;
        SetMoveButtonsInteractable(false);
        ResultPanel.SetActive(true);

        if (won)
        {
            int xp = monsterData.xp_reward;
            int oldLevel = GameState.Instance.Hero.level;
            GameState.Instance.AwardXp(xp);
            int newLevel = GameState.Instance.Hero.level;

            string learnedLine = "";
            var unlearned = monsterData.moves.FindAll(m => !GameState.Instance.LearnedMoves.Contains(m));
            if (unlearned.Count > 0)
            {
                string newMove = unlearned[Random.Range(0, unlearned.Count)];
                GameState.Instance.LearnedMoves.Add(newMove);
                learnedLine = $"\nLearned: {moves[newMove].name}";
            }
            else
            {
                learnedLine = "\n(All moves already learned)";
            }

            GameState.Instance.CurrentEncounterIndex++;

            string lvLine = newLevel > oldLevel ? $"\n<color=#ffd966>Level up! Now level {newLevel}</color>" : "";
            ResultText.text = $"<color=#a8e6a3>Victory!</color>\n+{xp} XP{lvLine}{learnedLine}";
        }
        else
        {
            ResultText.text = "<color=#ff8080>Defeat...</color>\nReturning to map.";
        }
    }

    void OnResultButton()
    {
        SceneManager.LoadScene("Map");
    }
}