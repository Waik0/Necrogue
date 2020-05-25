using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPresenter
{
    private BattleDataUseCase _battleDataUseCase;
    private PlayerDataUseCase _playerDataUseCase;
    public void Inject(PlayerDataUseCase playerDataUseCase, BattleDataUseCase battleDataUseCase)
    {
        _battleDataUseCase = battleDataUseCase;
        _playerDataUseCase = playerDataUseCase;
    }
    public void ResetPresenter()
    {
        
    }
#if DEBUG
    public void DebugUI()
    {
        
    }
#endif
}
