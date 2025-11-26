using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayController : MonoBehaviour
{
   
   [SerializeField] private string nextSceneName = "MainScene";
   [SerializeField] private GameObject blackScreen;
   [SerializeField] private float delayBeforeLoad = 0.15f;

   private bool hasClicked = false;
   
   void Update()
    {
        if (hasClicked) return;

        if(Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            hasClicked = true;
            StartCoroutine(FadeAndLoad());
        }

        
    }

    private IEnumerator FadeAndLoad()
    {
        if (blackScreen != null)
        {
            blackScreen.SetActive(true);
        }

        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
