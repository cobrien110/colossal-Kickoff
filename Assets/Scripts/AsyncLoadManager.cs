using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private bool hasLoadingBar;
    [SerializeField] private bool autoSwitchOnLoad = true;

    [SerializeField] private Image loadBar;

    public void BeginLoad(string levelName)
    {
        if (hasLoadingBar)
        {
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
            if (loadingCanvas != null) loadingCanvas.SetActive(true);
        }

        StartCoroutine(LoadLevelAsync(levelName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelName);
        if (!autoSwitchOnLoad) loadOperation.allowSceneActivation = false; // Prevent auto-switching

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / .9f);
            if (loadBar != null) loadBar.fillAmount = progressValue;
            yield return null;
        }
    }

    public void LoadScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
