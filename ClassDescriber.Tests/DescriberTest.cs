using System;
using ClassDescriber.Library;
using Xunit;

namespace ClassDescriber.Tests
{
    public class DescriberTest
    {
        public DescriberTest()
        {
        }

        [Theory]
        [InlineData("")]
        [InlineData("asd")]
        [InlineData("EveryStringAvailable")]
        public void DescriberShouldDescribePrimitiveString(string input)
        {
            var result = Describer.Describe(input);
            Assert.Equal(result, '"' + input + '"' + '\r' + '\n');
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DescriberShouldDescribePrimitiveInteger(int input)
        {
            var result = Describer.Describe(input);
            Assert.Equal(result, input.ToString() + '\r' + '\n');
        }
        [Theory]
        [InlineData(0.5)]
        [InlineData(-1.5)]
        [InlineData(1.5)]
        public void DescriberShouldDescribePrimitiveDouble(int input)
        {
            var result = Describer.Describe(input);
            Assert.Equal(input.ToString() + '\r' + '\n', result);
        }

        [Fact]
        public void DescriberShouldReturnNullStringForNull()
        {
            var result = Describer.Describe(null);
            Assert.Equal("null" + '\r' + '\n', result);
        }
        [Fact]
        public void DescriberShouldReturnForObject()
        {
            var result = Describer.Describe(new object());
            Assert.Equal("Object of Class Object" + '\r' + '\n', result);
        }
    }
}
