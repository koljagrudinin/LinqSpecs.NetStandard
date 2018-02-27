using System;
using LinqSpecs.NetStandard;
using LinqSpecs.Tests.Core.Helpers;
using Xunit;

namespace LinqSpecs.Tests.Core
{

    public class AdHocSpecificationTests
    {
        [Fact]
        public void Constructor_should_throw_exception_when_argument_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AdHocSpecification<string>(null));
        }

        [Fact]
        public void Simple_adhoc_should_work()
        {
            var specification = new AdHocSpecification<string>(n => n.StartsWith("J"));

            var result = new SampleRepository().Find(specification);

            Assert.Contains("Jose", result);
            Assert.Contains("Julian", result);
            Assert.DoesNotContain("Manuel", result);
        }

        [Fact]
        public void Should_be_serializable()
        {
            var spec = new AdHocSpecification<string>(n => n == "it works");

            var deserializedSpec = Helpers.Helpers.SerializeAndDeserialize(spec);

            Assert.IsType<AdHocSpecification<string>>(deserializedSpec);
            Assert.True(deserializedSpec.ToExpression().Compile().Invoke("it works"));
            Assert.False(deserializedSpec.ToExpression().Compile().Invoke("it fails"));
        }
    }
}