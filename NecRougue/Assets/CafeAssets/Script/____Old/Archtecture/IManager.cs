using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// イベント駆動の代わり
/// T（インターフェース）を実装したクラスを集めて関数を呼び出すManagerを実装する。
/// ※自動追加はBind時のみなので注意！！！！（動的追加はAddが必須）
/// ※しかもTを実装するclassのbindはNonLazyのみ対応だと思う。
/// todo いつInjectされても機能できるようにしたい
/// </summary>
public interface IManager<T> : IDisposable
{
    void Add(T element);
    void RemoveNull();
}
