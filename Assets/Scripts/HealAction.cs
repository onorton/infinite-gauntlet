using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class HealAction : MonoBehaviour, IAction
    {
        private Character _c;
        void Start()
        {
            _c = GetComponent<Character>();
        }

        public void Execute(Character character)
        {
            character.Heal(1, _c);
        }
    }
}
