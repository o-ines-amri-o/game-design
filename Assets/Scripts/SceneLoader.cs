using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    // Update is called once per frame
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
