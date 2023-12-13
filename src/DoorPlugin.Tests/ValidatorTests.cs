namespace DoorPlugin.Tests
{
    using NUnit.Framework;
    using DoorPlugin.Model;
    using System;

    [TestFixture]
    public class ValidatorTests
    {
        [Description("Положительный тест валидатора.")]
        [TestCase(5.0, 1.0, 10.0)]
        [TestCase(7.5, 5.0, 10.0)]
        [TestCase(300, 10, 5000)]
        [TestCase(666, 666, 666)]
        public void ValidateRange_ShouldNotThrowException(
            double value,
            double minValue,
            double maxValue)
        {
            // Act, Assert
            Assert.DoesNotThrow(() =>
                Validator.ValidateParameter(value, minValue, maxValue));
        }

        [Description("Отрицательный тест валидатора.")]
        [TestCase(5.0, 35.0, 10.0)]
        [TestCase(7.5, -5.0, 10.0)]
        [TestCase(300, 10, 30)]
        [TestCase(700, 325, 666)]
        public void ValidateRange_ShouldThrowArgumentException(
            double value,
            double minValue,
            double maxValue)
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
                Validator.ValidateParameter(value, minValue, maxValue));
        }
    }
}
