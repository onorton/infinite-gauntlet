using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardState
{
    private List<CharacterState> _characterStates;
    public BoardState(List<Character> characters)
    {
        _characterStates = characters.Select(c => c.CharacterState()).ToList();
    }

    public override bool Equals(object obj)
    {
        return obj is BoardState state &&
              Enumerable.SequenceEqual(_characterStates, state._characterStates);
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        foreach (var c in _characterStates)
        {
            hashCode = unchecked(hashCode + EqualityComparer<CharacterState>.Default.GetHashCode(c));
        }
        return hashCode;
    }
}