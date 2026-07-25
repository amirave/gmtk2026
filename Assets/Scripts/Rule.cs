using System;
using Game;
using UnityEngine;

public enum RuleMode
{
    None, // Requires just the first property
    IsNot, // Requires just the first property
    And,
    ButNot,
}

[Serializable]
public class Rule
{
    [SerializeReference] public IProperty property;
    [SerializeField] public RuleMode mode;
    [SerializeReference] public IProperty secondProperty;

    public Rule() { }

    public Rule(IProperty first, RuleMode mode, IProperty second)
    {
        property = first;
        this.mode = mode;
        secondProperty = second;
    }
}