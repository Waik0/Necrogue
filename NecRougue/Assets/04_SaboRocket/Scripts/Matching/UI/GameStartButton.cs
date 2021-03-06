using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameStartButton : MonoBehaviour
{
   private MatchingHostSequence _matchingHostSequence;

   [Inject]
   void Inject(
      MatchingHostSequence matchingHostSequence
   )
   {
      _matchingHostSequence = matchingHostSequence;
   }

   public void SendGameStart()
   {
      _matchingHostSequence.ToGame();
   }

}
