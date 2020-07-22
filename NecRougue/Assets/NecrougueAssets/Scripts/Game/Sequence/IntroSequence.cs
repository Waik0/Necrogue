using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class IntroSequence : Sequence<bool>
{
    enum State
    {
        Init,
        Animation,
        End
    }

    private Statemachine<State> _statemachine;
    private IntroPresenter _introPresenter = new IntroPresenter();
    private IntroDataUseCase _introDataUseCase = new IntroDataUseCase();
    private bool _end = false;
    private int _current = 0;
    private bool _skip = false;
    // Start is called before the first frame update
    public IntroSequence()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public void Inject()
    {
        _introPresenter.Inject(_introDataUseCase);
    }

    // Update is called once per frame
    void Update()
    {
        _statemachine.Update();
    }

    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
    }

    public override bool UpdateSequence()
    {
        _statemachine.Update();
        return _statemachine.Current != State.End;
    }

    IEnumerator Init()
    {
        
        _statemachine.Next(State.Animation);
        yield return null;
    }

    IEnumerator Animation()
    {
        _introDataUseCase.ResetData();
        
        while (true)
        {
            if (!_introDataUseCase.SetData(101, _current))
            {
                break;
            }
            _introPresenter.ResetPresenter();
            while (!_skip)
            {
                _skip = !_introPresenter.MessageNext();
                for (int i = 0; i < 4; i++)
                {
                    yield return null;
                }
            }
            _skip = false;
            _current++;
            _introPresenter.MessageAll();
            yield return null;
        }
        _statemachine.Next(State.End);
        
    }
    #if DEBUG
    public void DebugUI()
    {
        _introPresenter.DebugUI();
    }
#endif 
}
