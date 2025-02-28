using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject loadingCanvas;

    [SerializeField] private Image loadBar;

    public void BeginLoad(string levelName)
    {
        mainMenuCanvas.SetActive(false);
        loadingCanvas.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelName);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / .9f);
            loadBar.fillAmount = progressValue;
            yield return null;
        }
    }
}
