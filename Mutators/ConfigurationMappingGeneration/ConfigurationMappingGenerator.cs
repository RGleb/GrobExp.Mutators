using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using GrobExp.Mutators.AutoEvaluators;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public static class ConfigurationMappingGenerator
    {
        public static ConfigurationMappingGenerationResult GenerateMapping(ModelConfigurationNode node)
        {
            var toSourceModels = new List<ConfigurationMappingRootModel>();
            var fromSourceModels = new List<ConfigurationMappingRootModel>();

            foreach (var mutator in GetAllMutators(node))
            {
                if (!(mutator.Source is EqualsToConfiguration equalsToConfiguration))
                    continue;

                var fromSourceExpression = equalsToConfiguration.Value.Body;
                var toSourceExpression = mutator.Destination;

                if (!TryGetMappingModel(fromSourceExpression, out var fromSourceModel))
                    continue;
                if (!TryGetMappingModel(toSourceExpression, out var toSourceModel))
                    continue;

                var fromSourceLeafChildModel = fromSourceModel.GetLeafChild();
                fromSourceLeafChildModel.From = fromSourceExpression;
                fromSourceLeafChildModel.To = toSourceExpression;
                fromSourceLeafChildModel.ToModel = GetMappingModel(toSourceExpression);
                var lastLastToSourceModel = fromSourceLeafChildModel.ToModel.GetLeafChild();
                lastLastToSourceModel.From = toSourceExpression;
                lastLastToSourceModel.To = fromSourceExpression;

                var toSource = toSourceModel.GetLeafChild();
                toSource.From = toSourceExpression;
                toSource.To = fromSourceExpression;
                toSource.ToModel = GetMappingModel(fromSourceExpression);
                var lastLastToDestinationModel = toSource.ToModel.GetLeafChild();
                lastLastToDestinationModel.From = fromSourceExpression;
                lastLastToDestinationModel.To = toSourceExpression;

                fromSourceModels.Add(fromSourceModel);
                toSourceModels.Add(toSourceModel);
            }

            return new ConfigurationMappingGenerationResult
                {
                    FromSource = GroupModels(fromSourceModels),
                    ToSource = GroupModels(toSourceModels),
                };
        }

        private static ConfigurationMappingRootModel GroupModels(List<ConfigurationMappingRootModel> models)
        {
            return models.GroupBy(x => x.RootType)
                         .Select(x => new ConfigurationMappingRootModel
                             {
                                 RootType = x.Key,
                                 ChildModels = GroupChild(x.SelectMany(z => z.ChildModels).ToArray())
                             })
                         .Single();
        }

        private static ConfigurationMappingChildModel[] GroupChild(ConfigurationMappingChildModel[] child)
        {
            var result = child.GroupBy(x => x).ToArray();
            foreach (var group in result)
                group.Key.FromModels = GroupChild(group.SelectMany(x => x.FromModels ?? new ConfigurationMappingChildModel[0]).ToArray());
            return result.Select(x => x.Key).ToArray();
        }

        private static ConfigurationMappingRootModel GetMappingModel(Expression expression)
        {
            return TryGetMappingModel(expression, out var model) ? model : throw new InvalidOperationException();
        }

        private static bool TryGetMappingModel(Expression expression, out ConfigurationMappingRootModel model)
        {
            model = new ConfigurationMappingRootModel();
            var childModelsStack = new Stack<ConfigurationMappingChildModel>();
            while (true)
            {
                if (expression is ParameterExpression parameterExpression)
                {
                    model.RootType = parameterExpression.Type;
                    break;
                }
                else if (expression is MemberExpression memberExpression)
                {
                    childModelsStack.Push(new ConfigurationMappingChildModel {CurrentMember = memberExpression.Member});
                    expression = memberExpression.Expression;
                }
                else if (expression is MethodCallExpression methodCallExpression)
                {
                    if (methodCallExpression.Method.IsEachMethod() || methodCallExpression.Method.IsCurrentMethod())
                        expression = methodCallExpression.Arguments.First();
                    else
                        return false;
                }
                else if (expression is BinaryExpression binaryExpression)
                {
                    if (binaryExpression.NodeType == ExpressionType.ArrayIndex &&
                        binaryExpression.Left is MemberExpression left && binaryExpression.Right is ConstantExpression right)
                    {
                        childModelsStack.Push(new ConfigurationMappingChildModel {CurrentMember = left.Member, CurrentArrayIndex = (int)right.Value});
                        expression = left.Expression;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            foreach (var childModel in childModelsStack)
                childModel.RootType = model.RootType;

            var childModels = childModelsStack.ToArray();
            for (var i = 0; i < childModels.Length - 1; i++)
                childModels[i].FromModels = new[] {childModels[i + 1]};
            model.ChildModels = new[] {childModels[0]};

            return true;
        }

        private static (Expression Destination, MutatorConfiguration Source)[] GetAllMutators(ModelConfigurationNode node)
        {
            return node.Mutators
                       .Select(mutator => (mutator.Key, mutator.Value))
                       .Concat(node.Children.SelectMany(GetAllMutators))
                       .ToArray();
        }
    }
}