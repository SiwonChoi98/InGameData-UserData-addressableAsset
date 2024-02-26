using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class LoadingManager : MonoBehaviour
{
    public static string NextScene;
    
    public Slider LoadingSlider;
    
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(StartLoadingScene());
    }

    private IEnumerator StartLoadingScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(NextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                LoadingSlider.value = Mathf.Lerp(LoadingSlider.value, op.progress, timer);

                if (LoadingSlider.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                LoadingSlider.value = Mathf.Lerp(LoadingSlider.value, 1f, timer);

                if (LoadingSlider.value == 1f)
                {
                    yield return new WaitForSeconds(2f);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
    
    //static
    public static void LoadScene(string sceneName)
    {
        NextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
}
