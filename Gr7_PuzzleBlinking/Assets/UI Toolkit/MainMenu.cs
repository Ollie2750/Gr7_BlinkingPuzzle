using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button startButton;
    private Button exitButton;
    private List<Button> menuButtons = new List<Button>();
    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        startButton = uiDocument.rootVisualElement.Q("StartButton") as Button;
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        exitButton = uiDocument.rootVisualElement.Q("ExitButton") as Button;
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);

        menuButtons = uiDocument.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].RegisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(OnStartButtonClicked);
        exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClicked);
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Exit Play Mode in Editor!");
            return;
        }
        else {
            Application.Quit();
        }
    }

    private void OnButtonClicked(ClickEvent evt)
    {
        Debug.Log("Button Clicked!");
    }
}
