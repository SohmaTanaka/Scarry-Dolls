using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
    //　非同期動作で使用するAsyncOperation
    private AsyncOperation async;

    [Tooltip("読み込み率を表示するスライダー")] public Slider slider;

    private void Awake()
    {
        FadeManager.FadeIn();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        async = SceneManager.LoadSceneAsync(FadeManager.nextScene, LoadSceneMode.Additive);

        while (!async.isDone)
        {
            float progressValue = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressValue;

            yield return null;
        }

        SceneManager.UnloadSceneAsync(0, UnloadSceneOptions.None);
    }
}