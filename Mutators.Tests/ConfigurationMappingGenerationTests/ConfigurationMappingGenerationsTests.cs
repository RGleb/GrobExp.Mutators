using System.Linq;

using FluentAssertions;

using GrobExp.Mutators;
using GrobExp.Mutators.ConfigurationMappingGeneration;

using NUnit.Framework;

namespace Mutators.Tests.ConfigurationMappingGenerationTests
{
    public class ConfigurationMappingGenerationsTests
    {
        [Test]
        public void TestSingleConverter_ShouldGenerateMappingFromTestAToTestB()
        {
            ConfigurationMappingRegistry.Register(new TestAToTestBConverterCollection());
            var model = ConfigurationMappingRegistry.TryGetMappingModel(typeof(TestA), typeof(TestB));

            model.RootType.Should().Be(typeof(TestA));
            model.ChildModels.Should().HaveCount(2);

            model.ChildModels[0].RootType.Should().Be(typeof(TestA));
            model.ChildModels[0].IsLeaf.Should().BeFalse();
            model.ChildModels[0].CurrentMember.Should().Be(typeof(TestA).GetMember(nameof(TestA.AD)).First());

            model.ChildModels[0].FromModels[0].RootType.Should().Be(typeof(TestA));
            model.ChildModels[0].FromModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[0].FromModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[0].FromModels[0].CurrentMember.Should().Be(typeof(TestD).GetMember(nameof(TestD.DA)).First());

            model.ChildModels[0].FromModels[0].ToModel.RootType.Should().Be(typeof(TestB));
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].CurrentMember.Should().Be(typeof(TestB).GetMember(nameof(TestB.BA)).First());

            model.ChildModels[1].RootType.Should().Be(typeof(TestA));
            model.ChildModels[1].IsLeaf.Should().BeTrue();
            model.ChildModels[1].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[1].CurrentMember.Should().Be(typeof(TestA).GetMember(nameof(TestA.AA)).First());

            model.ChildModels[1].ToModel.RootType.Should().Be(typeof(TestB));

            model.ChildModels[1].ToModel.ChildModels[0].RootType.Should().Be(typeof(TestB));
            model.ChildModels[1].ToModel.ChildModels[0].IsLeaf.Should().BeFalse();
            model.ChildModels[1].ToModel.ChildModels[0].CurrentMember.Should().Be(typeof(TestB).GetMember(nameof(TestB.BD)).First());

            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].RootType.Should().Be(typeof(TestB));
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].CurrentMember.Should().Be(typeof(TestD).GetMember(nameof(TestD.DA)).First());
        }

        [Test]
        public void TestTwoConverters_ShouldGenerateMappingFromTestAToTestC()
        {
            ConfigurationMappingRegistry.Register(new TestAToTestBConverterCollection(), new TestBToTestCConverterCollection());
            var model = ConfigurationMappingRegistry.TryGetMappingModel(typeof(TestA), typeof(TestC));

            model.RootType.Should().Be(typeof(TestA));
            model.ChildModels.Should().HaveCount(2);

            model.ChildModels[0].RootType.Should().Be(typeof(TestA));
            model.ChildModels[0].IsLeaf.Should().BeFalse();
            model.ChildModels[0].CurrentMember.Should().Be(typeof(TestA).GetMember(nameof(TestA.AD)).First());

            model.ChildModels[0].FromModels[0].RootType.Should().Be(typeof(TestA));
            model.ChildModels[0].FromModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[0].FromModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[0].FromModels[0].CurrentMember.Should().Be(typeof(TestD).GetMember(nameof(TestD.DA)).First());

            model.ChildModels[0].FromModels[0].ToModel.RootType.Should().Be(typeof(TestC));
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].RootType.Should().Be(typeof(TestC));
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[0].FromModels[0].ToModel.ChildModels[0].CurrentMember.Should().Be(typeof(TestC).GetMember(nameof(TestC.CA)).First());

            model.ChildModels[1].RootType.Should().Be(typeof(TestA));
            model.ChildModels[1].IsLeaf.Should().BeTrue();
            model.ChildModels[1].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[1].CurrentMember.Should().Be(typeof(TestA).GetMember(nameof(TestA.AA)).First());

            model.ChildModels[1].ToModel.RootType.Should().Be(typeof(TestC));

            model.ChildModels[1].ToModel.ChildModels[0].RootType.Should().Be(typeof(TestC));
            model.ChildModels[1].ToModel.ChildModels[0].IsLeaf.Should().BeFalse();
            model.ChildModels[1].ToModel.ChildModels[0].CurrentMember.Should().Be(typeof(TestC).GetMember(nameof(TestC.CD)).First());

            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].RootType.Should().Be(typeof(TestC));
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].IsLeaf.Should().BeTrue();
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].FromModels.Should().BeNullOrEmpty();
            model.ChildModels[1].ToModel.ChildModels[0].FromModels[0].CurrentMember.Should().Be(typeof(TestD).GetMember(nameof(TestD.DA)).First());
        }

        private class TestA
        {
            public string AA { get; set; }
            public TestD AD { get; set; }
        }

        private class TestB
        {
            public string BA { get; set; }
            public TestD BD { get; set; }
        }

        private class TestC
        {
            public string CA { get; set; }
            public TestD CD { get; set; }
        }

        private class TestD
        {
            public string DA { get; set; }
        }

        private class TestAToTestBConverterCollection : ConverterCollection<TestA, TestB>
        {
            public TestAToTestBConverterCollection()
                : base(new PathFormatterCollection(), new TestStringConverter())
            {
            }

            protected override void Configure(MutatorsContext context, ConverterConfigurator<TestA, TestB> configurator)
            {
                configurator.Target(x => x.BA)
                            .Set(x => x.AD.DA);
                configurator.GoTo(x => x.BD)
                            .Target(x => x.DA)
                            .Set(x => x.AA);
            }
        }

        private class TestBToTestCConverterCollection : ConverterCollection<TestB, TestC>
        {
            public TestBToTestCConverterCollection()
                : base(new PathFormatterCollection(), new TestStringConverter())
            {
            }

            protected override void Configure(MutatorsContext context, ConverterConfigurator<TestB, TestC> configurator)
            {
                configurator.Target(x => x.CA)
                            .Set(x => x.BA);
                configurator.GoTo(x => x.CD)
                            .Target(x => x.DA)
                            .Set(x => x.BD.DA);
            }
        }
    }
}