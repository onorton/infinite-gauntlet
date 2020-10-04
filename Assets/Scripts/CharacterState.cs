using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    public object CharacterType { get; internal set; }
    public int Health { get; internal set; }

    public override bool Equals(object obj)
    {
        return obj is CharacterState state &&
               EqualityComparer<object>.Default.Equals(CharacterType, state.CharacterType) &&
               Health == state.Health;
    }

    public override int GetHashCode()
    {
        int hashCode = 1302919431;
        hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(CharacterType);
        hashCode = hashCode * -1521134295 + Health.GetHashCode();
        return hashCode;
    }
}