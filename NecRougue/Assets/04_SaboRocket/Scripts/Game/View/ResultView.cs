using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResultView : MonoBehaviour
{
    [SerializeField] private Text _result;
    private ResultUseCase _resultUseCase;
    [Inject]
    void Inject(
        ResultUseCase resultUseCase
    )
    {
        _resultUseCase = resultUseCase;
        _resultUseCase.OnResult = OnUpdateDeck;
    }
    void OnUpdateDeck(ResultData resultData)
    {
        StartCoroutine(Result(resultData));
    }

    IEnumerator Result(ResultData resultData)
    {
        _result.text = "";
        _result.text += resultData.winRoll.ToString() + "のかち";
        yield return new WaitForSeconds(3);
        _result.text = "";
    }
}
