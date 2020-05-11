using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public interface ISequence<T>
{
    void ResetSequence();
    T UpdateSequence();

}

public abstract class SequenceBehaviour<T> : MonoBehaviour, ISequence<T>
{
    public abstract void ResetSequence();

    public abstract T UpdateSequence();
}
public abstract class Sequence<T> : ISequence<T>
{
    public abstract void ResetSequence();

    public abstract T UpdateSequence();
}