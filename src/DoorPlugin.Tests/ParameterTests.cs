namespace DoorPlugin.Tests
{
    using NUnit.Framework;
    using DoorPlugin.Model;
    using System;

    [TestFixture]
    public class ParameterTests
    {
        [Description("Положительный тест конструктора Parameter.")]
        [TestCase(4, 14, 1)]
        [TestCase(700, 10000, 699)]
        [TestCase(23, 67, 18)]
        public void Parameter_Initialization_SetCorrectly(
            double currentValue,
            double maxValue,
            double minValue)
        {
            // Arrange, Act
            var parameter = new Parameter(currentValue, minValue, maxValue);

            // Assert
            Assert.AreEqual(currentValue,parameter.CurrentValue);
            Assert.AreEqual(maxValue, parameter.Max);
            Assert.AreEqual(minValue, parameter.Min);
        }

        [Description("Положительный тест задания значений Parameter.")]
        [TestCase(4, 14, 1)]
        [TestCase(700, 10000, 699)]
        [TestCase(23, 67, 18)]
        public void Parameter_SetProperties_UpdateValues(
            double currentValue,
            double maxValue,
            double minValue)
        {
            // Arrange
            var parameter = new Parameter();

            // Act
            parameter.CurrentValue = currentValue;
            parameter.Max = maxValue;
            parameter.Min = minValue;

            // Assert
            Assert.AreEqual(currentValue,parameter.CurrentValue);
            Assert.AreEqual(maxValue, parameter.Max);
            Assert.AreEqual(minValue, parameter.Min);
        }

        [Description("Отрицательный тест задания значений Parameter.")]
        [TestCase(5, 3, 1)]
        [TestCase(700, 10000, 20000)]
        [TestCase(23, 67, -18)]
        public void Parameter_Initialization_IncorrectValues_ThrowArgumentException(
            double currentValue,
            double maxValue,
            double minValue)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() =>
                new Parameter(currentValue, maxValue, minValue));
        }
    }
}
