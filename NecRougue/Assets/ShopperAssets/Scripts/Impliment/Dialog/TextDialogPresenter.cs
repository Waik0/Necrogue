using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDialogPresenter : ITextDialogPresenter
{
    private DialogView _dialogView;
    public TextDialogPresenter(DialogView view)
    {
        _dialogView = view;
    }

    public void Open(Vector2 pos,Vector2 size, string text)
    {
        _dialogView.SetPosition(pos,size);
        _dialogView.SetText(text);
        _dialogView.Open();
    }

    public void Close()
    {
        _dialogView.Close();   
    }
}
