using System;
using System.Collections.Generic;
using UnityEngine;

public class ErrorLog : MonoBehaviour {

    public void Add(string text) {
        _textList.Insert(0, text);
        _textChanged = true;
    }

    private void Update() {
        if (_textChanged) {
            _text = string.Join(Environment.NewLine, _textList.ToArray());
            _textChanged = false;
        }
    }

    private void OnGUI() {
        if (_text != null) {
            GUI.TextArea(new Rect(400, 10, 600, 800), _text);
        }
    }

    private readonly List<string> _textList = new List<string>();
    private bool _textChanged;
    private string _text;

}
