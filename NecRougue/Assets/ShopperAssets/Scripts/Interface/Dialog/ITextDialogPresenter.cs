using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITextDialogPresenter
{
    void Open(Vector2 pos,Vector2 size,string text);
    void Close();
}
