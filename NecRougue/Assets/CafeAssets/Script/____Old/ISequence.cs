using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ステートマシンでゲームの流れを管理する
/// </summary>
public interface ISequence
{
    bool UpdateState();
}

/// <summary>
/// シーケンス終了時に結果を返す
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISequenceResult<T> where T : struct
{
    T Result();
}

public abstract class SequenceRoot : MonoBehaviour,ISequence
{ 
    public abstract bool UpdateState();
}