using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toast
{

    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance { get; } = new T();
    }

    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        [SerializeField] private bool _dontDestroyOnLoad = true;
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    System.Type t = typeof(T);

                    _instance = (T)FindObjectOfType(t);
                    if (_instance == null)
                    {
                        //Debug.LogError($" {t} が見つからない");
                        Debug.Log($"Make Singleton {t}");
                        var go = new GameObject();
                        go.AddComponent<T>();
                        _instance = go.GetComponent<T>();
                    }
                }

                return _instance;
            }
        }
        public static bool Exists => _instance != null;
        virtual protected void Awake()
        {
            // 他のGameObjectにアタッチされているか調べる.
            // アタッチされている場合は破棄する.
            if (this != Instance)
            {
                Destroy(this);
                //Destroy(this.gameObject);
                Debug.Log(
                    typeof(T) + "はすでににアタッチされています。");
                return;
            }
            Debug.Log(typeof(T) + "が" + this.gameObject.name + "にアタッチされました");
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

}
