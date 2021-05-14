using System.Collections.Generic;
using System.Linq;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public static class ConfigurationMappingTracer
    {
        public static ConfigurationMappingRootModel Trace(ConfigurationMappingRootModel from, ConfigurationMappingRootModel to)
        {
            foreach (var leaf in GetAllLeaf(from.ChildModels))
            {
                var toLeaf = TryFindLeaf(leaf.ToModel, to);
                if (toLeaf == null)
                {
                    leaf.To = null;
                    leaf.ToModel = null;
                }
                else
                {
                    leaf.To = toLeaf.GetLeafChild().From;
                    leaf.ToModel = toLeaf;
                }
            }
            return from;
        }

        private static ConfigurationMappingRootModel TryFindLeaf(ConfigurationMappingRootModel leaf, ConfigurationMappingRootModel to)
        {
            if (leaf.RootType != to.RootType)
                return null;

            var currentLeaf = leaf.ChildModels.First();
            var currentTo = to.ChildModels;
            while (true)
            {
                var current = currentTo.FirstOrDefault(x => x.Equals(currentLeaf));
                if (current == null)
                    return null;
                if (currentLeaf.IsLeaf && current.IsLeaf)
                    return current.ToModel;
                if (!currentLeaf.IsLeaf && !current.IsLeaf)
                {
                    currentLeaf = currentLeaf.FromModels.First();
                    currentTo = current.FromModels;
                    continue;
                }
                return null;
            }
        }

        private static List<ConfigurationMappingChildModel> GetAllLeaf(ConfigurationMappingChildModel[] child)
        {
            var result = new List<ConfigurationMappingChildModel>();
            foreach (var c in child)
                result.AddRange(c.IsLeaf ? new List<ConfigurationMappingChildModel> {c} : GetAllLeaf(c.FromModels));
            return result;
        }
    }
}