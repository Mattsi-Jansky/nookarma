using Jansk.Karma.Plugins;
using Xunit;

namespace Jansk.Karma.Tests.Plugins
{
    public class ParseKarmaChangeTests
    {
        [Fact]
        public void WhenGeneratingChangeRequest_ShouldNotIncludeOperator()
        {
            var expected = "test";
            var plugin = new KarmaPlugin();
            
            var result = plugin.ParseKarmaChange("test++");
            
            Assert.Equal(expected, result.Name);
        }

        [Fact]
        public void WhenUsingPositiveKarmaOperator_ShouldHaveChangeAmoutOne()
        {
            var plugin = new KarmaPlugin();
            
            var result = plugin.ParseKarmaChange("test++");
            
            Assert.Equal(1, result.Amount);
        }
        
        [Fact]
        public void WhenUsingNegativeKarmaOperator_ShouldHaveChangeAmoutNegativeOne()
        {
            var plugin = new KarmaPlugin();
            
            var result = plugin.ParseKarmaChange("test--");
            
            Assert.Equal(-1, result.Amount);
        }
    }
}