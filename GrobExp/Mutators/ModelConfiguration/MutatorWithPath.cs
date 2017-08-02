using System.Linq.Expressions;

namespace GrobExp.Mutators.ModelConfiguration
{
    public class MutatorWithPath
    {
        public Expression PathToNode { get; set; }
        public Expression PathToMutator { get; set; }
        public MutatorConfiguration Mutator { get; set; }
    }
}