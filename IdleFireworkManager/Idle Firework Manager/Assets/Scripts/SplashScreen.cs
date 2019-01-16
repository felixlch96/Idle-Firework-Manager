using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(loadNextScene());
	}

    IEnumerator loadNextScene()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("main gameplay scene");
    }
}
