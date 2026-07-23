using System;
using Game;
using UnityEngine;

public enum RuleConcatinator
{
    None,
    And,
    ButNot,
}

[Serializable]
public class Rule
{
    [SerializeReference] public IProperty property;

    public Rule(IProperty property)
    {
        this.property = property;
    }
}