using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button button;
    private List<Button> menuButtons = new List<Button>();
    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        button = uiDocument.rootVisualElement.Q("StartButton") as Button;
        button.RegisterCallback<ClickEvent>(OnStartButtonClicked);

        menuButtons = uiDocument.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].RegisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    private void OnDisable()
    {
        button.UnregisterCallback<ClickEvent>(OnStartButtonClicked);
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnButtonClicked(ClickEvent evt)
    {
        Debug.Log("Button Clicked!");
    }
}
