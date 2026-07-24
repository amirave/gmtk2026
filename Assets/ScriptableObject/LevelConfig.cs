using UnityEngine;
using System.Collections.Generic;
using Game;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class LevelConfig : ScriptableObject
{
    [SerializeReference] private List<Rule> _addedRules;
    [SerializeField] private List<Item> _itemPrefabs;
    [SerializeField] private int _roundsPerLevel;
    
    public List<Item> ItemPrefabs => _itemPrefabs;
    public List<Rule> AddedRules => _addedRules;
    public int RoundsPerLevel => _roundsPerLevel;
}
