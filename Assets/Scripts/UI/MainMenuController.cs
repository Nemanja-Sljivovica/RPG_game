using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button StartButton;
    public Button ExitButton;
    public TMP_Text StatusText;

    void Start()
    {
        StartButton.onClick.AddListener(OnStartClicked);
        ExitButton.onClick.AddListener(OnExitClicked);
        if (StatusText != null) StatusText.text = "";
    }

    void OnStartClicked()
    {
        StartButton.interactable = false;
        ExitButton.interactable = false;
        if (StatusText != null) StatusText.text = "Connecting to server...";

        StartCoroutine(ApiClient.Instance.GetRunConfig(
            onSuccess: (config) =>
            {
                GameState.Instance.StartNewRun(config);
                SceneManager.LoadScene("Map");
            },
            onError: (err) =>
            {
                if (StatusText != null) StatusText.text = $"Server error: {err}\nIs Django running on localhost:8000?";
                StartButton.interactable = true;
                ExitButton.interactable = true;
            }
        ));
    }

    void OnExitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}