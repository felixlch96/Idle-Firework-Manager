using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartUpLoader : MonoBehaviour
{
    //loading variables
    public Slider loadingBar;
    public GameObject loadingScreen;
    public TextMeshProUGUI progressTxt;

    // Use this for initialization
    void Start ()
    {        
        StartCoroutine(loadScene("main gameplay scene"));
	}
    
    IEnumerator loadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        while (!operation.isDone)
        {            
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.value = progress;
            progressTxt.SetText(((int)(progress * 100)).ToString() + "%");

            yield return null;
        }        
    }
}
