using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MoveManagementController : MonoBehaviour
{
    public Transform LearnedContent;
    public Transform EquippedContent;
    public Button MoveButtonTemplate;
    public Button DoneButton;
    public TMP_Text CapacityText;
    public TMP_Text StatusText;

    void Start()
    {
        if (GameState.Instance == null || GameState.Instance.Config == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        DoneButton.onClick.AddListener(OnDone);
        if (StatusText != null) StatusText.text = "";
        Refresh();
    }

    void Refresh()
    {
        ClearChildren(LearnedContent);
        ClearChildren(EquippedContent);

        var state = GameState.Instance;
        var moves = state.Config.moves;

        // Learned but NOT equipped
        foreach (var moveId in state.LearnedMoves)
        {
            if (state.EquippedMoves.Contains(moveId)) continue;
            CreateMoveButton(LearnedContent, moveId, moves[moveId], () => Equip(moveId));
        }

        // Equipped
        foreach (var moveId in state.EquippedMoves)
        {
            if (!moves.ContainsKey(moveId)) continue;
            CreateMoveButton(EquippedContent, moveId, moves[moveId], () => Unequip(moveId));
        }

        if (CapacityText != null)
            CapacityText.text = $"Equipped: {state.EquippedMoves.Count} / {state.MaxEquipped}";
    }

    void CreateMoveButton(Transform parent, string moveId, MoveData move, System.Action onClick)
    {
        var btn = Instantiate(MoveButtonTemplate, parent);
        btn.gameObject.SetActive(true);
        btn.name = $"MoveBtn_{moveId}";
        var label = btn.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            string typeTag = move.type == "magic" ? "<color=#a3c2ff>[M]</color>" : "<color=#ffb88a>[P]</color>";
            label.text = $"{typeTag} {move.name}";
        }
        btn.onClick.AddListener(() => onClick());
    }

    void Equip(string moveId)
    {
        var state = GameState.Instance;
        if (state.EquippedMoves.Count >= state.MaxEquipped)
        {
            StatusText.text = $"Max {state.MaxEquipped} equipped. Remove one first.";
            return;
        }
        if (!state.EquippedMoves.Contains(moveId))
            state.EquippedMoves.Add(moveId);
        StatusText.text = "";
        Refresh();
    }

    void Unequip(string moveId)
    {
        var state = GameState.Instance;
        state.EquippedMoves.Remove(moveId);
        StatusText.text = "";
        Refresh();
    }

    void OnDone()
    {
        if (GameState.Instance.EquippedMoves.Count == 0)
        {
            StatusText.text = "Equip at least 1 move before continuing.";
            return;
        }
        SceneManager.LoadScene("Map");
    }

    void ClearChildren(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--) Destroy(t.GetChild(i).gameObject);
    }
}