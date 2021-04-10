using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneProgress : MonoBehaviour
{
    private Coroutine _loadScene;
    [Inject] private ZenjectSceneLoader _zenjectSceneLoader;
    public void LoadScene(string sceneName,BridgeProperty bridgeProperty,Action onLoad = null)
    {
        if (_loadScene != null)
        {
            StopCoroutine(_loadScene);
        }
        _loadScene = StartCoroutine(LoadSceneAsync(sceneName,LoadSceneMode.Single,bridgeProperty,onLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName,LoadSceneMode mode,BridgeProperty bridgeProperty,Action onLoad = null)
    {
        Debug.Log("シーン読み込み開始");
        yield return _zenjectSceneLoader.LoadSceneAsync(sceneName,
            mode,
            extraBindings: container =>
            {
                Debug.Log("Bind : SceneProgress");
                container.Bind<BridgeProperty>().FromInstance(bridgeProperty).AsCached();
            });
        Debug.Log("シーン読み込み完了");
        onLoad?.Invoke();
    }

    public IEnumerator LoadSceneAdditiveAsync(string sceneName, BridgeProperty bridgeProperty, Action onLoad = null)
    {
        yield return LoadSceneAsync(sceneName,LoadSceneMode.Additive, bridgeProperty,onLoad);
    }
    public void LoadSceneAdditive(string sceneName,BridgeProperty bridgeProperty,Action onLoad = null)
    {
        if (_loadScene != null)
        {
            StopCoroutine(_loadScene);
        }
        _loadScene = StartCoroutine(LoadSceneAsync(sceneName,LoadSceneMode.Additive, bridgeProperty,onLoad));
    }
}
