using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadOnPlayMode : MonoBehaviour
{
    public string SceneName;

    private void Awake() {
        SceneManager.UnloadSceneAsync(SceneName);
    }
}
