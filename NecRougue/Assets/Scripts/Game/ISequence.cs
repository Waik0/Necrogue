using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public interface ISequence
{
    void ResetSequence();
    string UpdateSequence();

}

public abstract class SequenceBehaviour : MonoBehaviour, ISequence
{
    public abstract void ResetSequence();

    public abstract string UpdateSequence();
}