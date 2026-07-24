using System;
using Game;
using UnityEngine;

[Serializable]
public class LoseButton : CustomButton
{
    [SerializeField] private string _value;

    public string Value => _value;
}