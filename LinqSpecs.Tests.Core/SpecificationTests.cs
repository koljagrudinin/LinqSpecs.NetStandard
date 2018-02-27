using System;
using System.Linq.Expressions;
using LinqSpecs.NetStandard;
using LinqSpecs.Tests.Core.Helpers;
using Xunit;

namespace LinqSpecs.Tests.Core
{

    public class SpecificationTests
    {
        [Fact]
        public void ToString_should_return_expression_string()
        {
            Expression<Func<string, bool>> expr = s => s.Length == 2;
            Specification<string> spec = new AdHocSpecification<string>(expr);

            Assert.Equal(expr.ToString(), spec.ToString());
        }

        [Fact]
        public void Can_implicitly_convert_specification_to_expression()
        {
            Specification<string> spec = new AdHocSpecification<string>(s => s.Length == 2);
            Expression<Func<string, bool>> expr = spec;

            Assert.True(expr.Compile().Invoke("ab"));
            Assert.False(expr.Compile().Invoke("abcd"));
        }

        [Fact]
        public void And_operator_should_work()
        {
            var startWithJ = new AdHocSpecification<string>(n => n.StartsWith("J"));
            var endsWithE = new AdHocSpecification<string>(n => n.EndsWith("e"));

            var result = new SampleRepository().Find(startWithJ & endsWithE);

            Assert.Contains("Jose", result);
            Assert.DoesNotContain("Julian", result);
            Assert.DoesNotContain("Manuel", result);
        }

        [Fact]
        public void Or_operator_should_work()
        {
            var startWithJ = new AdHocSpecification<string>(n => n.StartsWith("J"));
            var endsWithN = new AdHocSpecification<string>(n => n.EndsWith("n"));

            var result = new SampleRepository().Find(startWithJ | endsWithN);

            Assert.Contains("Jose", result);
            Assert.Contains("Julian", result);
            Assert.DoesNotContain("Manuel", result);
        }

        [Fact]
        public void Negate_operator_should_work()
        {
            var startWithJ = new AdHocSpecification<string>(n => n.StartsWith("J"));

            var result = new SampleRepository().Find(!startWithJ);

            Assert.DoesNotContain("Jose", result);
            Assert.DoesNotContain("Julian", result);
            Assert.Contains("Manuel", result);
        }

        [Fact]
        public void AndAlso_operator_is_equivalent_to_And_operator()
        {
            var spec1 = new AdHocSpecification<string>(n => n.Length > 2);
            var spec2 = new AdHocSpecification<string>(n => n.Length < 5);

            Assert.Equal(spec1 & spec2, spec1 && spec2);
        }

        [Fact]
        public void OrElse_operator_is_equivalent_to_Or_operator()
        {
            var spec1 = new AdHocSpecification<string>(n => n.Length < 2);
            var spec2 = new AdHocSpecification<string>(n => n.Length > 5);

            Assert.Equal(spec1 | spec2, spec1 || spec2);
        }

        [Fact]
        public void Combination_of_boolean_operators_should_work()
        {
            var startWithM = new AdHocSpecification<string>(n => n.StartsWith("M"));
            var endsWithN = new AdHocSpecification<string>(n => n.EndsWith("n"));
            var containsU = new AdHocSpecification<string>(n => n.Contains("u"));

            var result = new SampleRepository().Find(startWithM | (!endsWithN & !containsU));

            Assert.Contains("Jose", result);
            Assert.DoesNotContain("Julian", result);
            Assert.Contains("Manuel", result);
        }
    }
}