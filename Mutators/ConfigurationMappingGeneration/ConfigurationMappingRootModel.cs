using System;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public class ConfigurationMappingRootModel
    {
        public Type RootType { get; set; }
        public ConfigurationMappingChildModel[] ChildModels { get; set; }
    }
}