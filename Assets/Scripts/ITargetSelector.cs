using System.Collections.Generic;

namespace Targets
{
    public interface ITargetSelector
    {
        IEnumerable<Character> SelectTargets(List<Character> characters);

        int Range();
    }

}