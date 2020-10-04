using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions {

    public class AttackAction : MonoBehaviour, IAction
    {
        public void Execute(Character character)
        {
            character.TakeDamage(1);
        }
    }
}
