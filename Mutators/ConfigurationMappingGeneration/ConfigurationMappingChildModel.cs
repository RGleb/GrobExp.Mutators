using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public class ConfigurationMappingChildModel
    {
        public Type RootType { get; set; }

        public Expression From { get; set; }
        public Expression To { get; set; }

        public ConfigurationMappingChildModel[] FromModels { get; set; }
        public ConfigurationMappingRootModel ToModel { get; set; }

        public MemberInfo CurrentMember { get; set; }
        public int? CurrentArrayIndex { get; set; }

        public bool IsLeaf => (FromModels?.Length ?? 0) == 0;

        public override int GetHashCode()
        {
            return CurrentMember.GetHashCode() + CurrentArrayIndex?.GetHashCode() ?? -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConfigurationMappingChildModel model)
                return CurrentMember == model.CurrentMember && CurrentArrayIndex == model.CurrentArrayIndex;
            return false;
        }
    }
}