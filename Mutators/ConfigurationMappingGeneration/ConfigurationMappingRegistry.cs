using System;
using System.Collections.Generic;
using System.Linq;

namespace GrobExp.Mutators.ConfigurationMappingGeneration
{
    public static class ConfigurationMappingRegistry
    {
        public static void Register<TFrom, TTo>(ConverterCollection<TFrom, TTo> collection)
            where TTo : new()
        {
            var generationResult = ConfigurationMappingGenerator.GenerateMapping(collection.GetConverterTree(MutatorsContext.Empty));
            var mappingModel = generationResult.FromSource;
            mappingSettings.Add(new ConfigurationMappingSettings
                {
                    From = typeof(TFrom),
                    To = typeof(TTo),
                    MappingModel = mappingModel
                });
        }

        public static void Register<TFrom, TToFrom, TTo>(ConverterCollection<TFrom, TToFrom> fromCollection,
                                                         ConverterCollection<TToFrom, TTo> toCollection)
            where TToFrom : new()
            where TTo : new()
        {
            var fromGenerationResult = ConfigurationMappingGenerator.GenerateMapping(fromCollection.GetConverterTree(MutatorsContext.Empty));
            var toGenerationResult = ConfigurationMappingGenerator.GenerateMapping(toCollection.GetConverterTree(MutatorsContext.Empty));
            var mappingModel = ConfigurationMappingTracer.Trace(fromGenerationResult.FromSource, toGenerationResult.FromSource);
            mappingSettings.Add(new ConfigurationMappingSettings
                {
                    From = typeof(TFrom),
                    To = typeof(TTo),
                    MappingModel = mappingModel,
                });
        }

        public static ConfigurationMappingRootModel TryGetMappingModel(Type from, Type to)
        {
            return mappingSettings.FirstOrDefault(x => x.From == from && x.To == to)?.MappingModel;
        }

        private static List<ConfigurationMappingSettings> mappingSettings = new List<ConfigurationMappingSettings>();

        private class ConfigurationMappingSettings
        {
            public Type From { get; set; }
            public Type To { get; set; }
            public ConfigurationMappingRootModel MappingModel { get; set; }
        }
    }
}