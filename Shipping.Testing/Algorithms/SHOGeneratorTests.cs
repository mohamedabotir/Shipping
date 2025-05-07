using NUnit.Framework;
using Domain.Utils;
using System;

namespace UnitTests
{
    [TestFixture]
    public class SHOGeneratorTests
    {
        [Test]
        public void GenerateNumber_ShouldStartWithPrefixAndContainDate()
        {
            var generator = new SHOGenerator("123");
            var number = generator.GenerateNumber();

            var today = DateTime.Now.ToString("yyyyMMdd");

            Assert.IsTrue(number.StartsWith("SHO-"));
            Assert.IsTrue(number.Contains(today));
            Assert.IsTrue(number.EndsWith("123"));
        }

        [Test]
        public void IsValidNumber_ShouldReturnTrue_WhenFormatMatches()
        {
            var purchasePart = "789";
            var generator = new SHOGenerator(purchasePart);
            var generatedNumber = generator.GenerateNumber();

            var isValid = generator.IsValidNumber(generatedNumber);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidNumber_ShouldReturnFalse_WhenDatePartIsNotPresent()
        {
            var generator = new SHOGenerator("456");
            var invalidNumber = "SHO-INVALIDDATE-456";

            var isValid = generator.IsValidNumber(invalidNumber);

            Assert.IsFalse(isValid);
        }
        [Test]
        public void IsValidNumber_ShouldReturnFalse_InCorrectLength()
        {
            var generator = new SHOGenerator("999");
            var generatedNumber = generator.GenerateNumber();

            // Remove last digit to create an incorrect length
            var tampered = generatedNumber.Substring(0, generatedNumber.Length - 1);

            var isValid = generator.IsValidNumber(tampered);

            Assert.IsFalse(isValid);
        }
    }
}
