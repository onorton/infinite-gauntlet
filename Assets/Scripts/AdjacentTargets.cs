using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Targets
{
    public class AdjacentTargets : MonoBehaviour, ITargetSelector
    {
        public int MaximumTargets;
        private Character _c;
        void Start()
        {
            _c = GetComponent<Character>();
        }
        public IEnumerable<Character> SelectTargets(List<Character> characters) =>
            // Select up to MaximumTargets adjacent targets
            characters.Where(c => c != _c && _c.PositionHandler.GridPosition.IsAdjacent(c.PositionHandler.GridPosition)).Take(MaximumTargets);

        public int Range()
        {
            return 1;
        }
    }

}