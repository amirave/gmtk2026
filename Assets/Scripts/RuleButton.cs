using System;
using Game;
using TMPro;
using UnityEngine;

[Serializable]
public class RuleButton : CustomButton
{
    [SerializeReference] private Rule _rule;

    public Rule Rule => _rule;

    void Start()
    {
        GetComponentInChildren<TMP_Text>().text = Rule.property.ToString();
    }
}