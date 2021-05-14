using System.Linq;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public static class ConfigurationMappingRootModelExtensions
    {
        public static ConfigurationMappingChildModel GetLeafChild(this ConfigurationMappingRootModel rootModel)
        {
            return GetLeafRecursive(rootModel.ChildModels);
        }

        private static ConfigurationMappingChildModel GetLeafRecursive(ConfigurationMappingChildModel[] childModels)
        {
            var firstChild = childModels.First();
            return firstChild.IsLeaf ? firstChild : GetLeafRecursive(firstChild.FromModels);
        }
    }
}