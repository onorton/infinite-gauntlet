using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Targets
{
    public class ClosestInRange : MonoBehaviour, ITargetSelector
    {
        public int MaximumTargets;
        public int range;
        private Character _c;
        void Start()
        {
            _c = GetComponent<Character>();
        }
        public IEnumerable<Character> SelectTargets(List<Character> characters) =>
            characters.Where(c => c != _c && _c.PositionHandler.GridPosition.GridDistance(c.PositionHandler.GridPosition) <= range).OrderBy(c => _c.PositionHandler.GridPosition.GridDistance(c.PositionHandler.GridPosition)).Take(MaximumTargets);

        public int Range()
        {
            return range;
        }
    }

}