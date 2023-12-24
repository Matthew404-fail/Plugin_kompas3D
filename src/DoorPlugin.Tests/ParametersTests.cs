namespace DoorPlugin.Tests
{
    using NUnit.Framework;
    using DoorPlugin.Model;
    using System;

    [TestFixture]
    public class ParametersTests
    {
        // TODO: Добавить описания для тестов (+)
        [Description("Положительный тест присвоения нового значения параметру.")]
        [TestCase(
            ParametersEnum.HandleThickness,
            123,
            700,
            3,
            30)]
        [TestCase(
            ParametersEnum.PeepholeDiameter,
            56,
            98,
            12,
            78)]
        public void AssertParameter_Parameter_UpdateParameter(
            ParametersEnum parameterType,
            double currentValue,
            double maxValue,
            double minValue,
            double value)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(
                currentValue,
                minValue,
                maxValue);

            // Act
            parameters.CheckParameter(parameterType, parameter, value);

            // Assert
            Assert.AreEqual(
                value,
                parameters.ParametersDict[parameterType].CurrentValue);
        }

        [Description("Отрицательный тест присвоения нового значения параметру.")]
        [TestCase(
            ParametersEnum.DoorWidth,
            78,
            100,
            25,
            0)]
        [TestCase(
            ParametersEnum.PeepholeWidth,
            50,
            56,
            45,
            0)]
        public void AssertParameter_WrongParameterValue_ThrowException(
            ParametersEnum parameterType,
            double currentValue,
            double maxValue,
            double minValue,
            double value)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(
                currentValue,
                minValue,
                maxValue);

            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
                parameters.CheckParameter(parameterType, parameter, value));
        }

        [Description("Положительный тест IsHandleCylinder.")]
        [TestCase(true)]
        [TestCase(false)]
        public void IsHandleCylinder_SetGetCorrectValue(bool isHandleCylinder)
        {
            // Arrange
            var parameters = new Parameters();

            // ActAssert
            parameters.IsHandleCylinder = isHandleCylinder;

            // Assert
            Assert.AreEqual(isHandleCylinder, parameters.IsHandleCylinder);
        }

        [Description("Положительный тест проверки текущих значений параметров.")]
        [TestCase(new[]
        {
            ParametersEnum.DoorHeight,
            ParametersEnum.DoorWidth,
            ParametersEnum.DoorThickness,
            ParametersEnum.PeepholeHeight,
            ParametersEnum.PeepholeWidth,
            ParametersEnum.PeepholeDiameter,
            ParametersEnum.HandleHeight,
            ParametersEnum.HandleWidth,
            ParametersEnum.HandleBaseDiameter,
            ParametersEnum.HandleDiameter,
            ParametersEnum.HandleBaseThickness,
            ParametersEnum.HandleThickness,
            ParametersEnum.HandleRecWidth,
            ParametersEnum.HandleRecHeight,
        }, new[]
        {
            2000.0,
            750.0,
            50.0,
            1800.0,
            375.0,
            40.0,
            1000.0,
            656.25,
            30.0,
            60.0,
            55.0,
            15.0,
            45.0,
            200.0,
        })]
        public void GetParametersCurrentValues_CurrentValuesAreEqual(
            ParametersEnum[] parametersTypes,
            double[] currentValues)
        {
            // Arrange
            var parameters = new Parameters();
            var parametersDict =
                parameters.GetParametersCurrentValues();

            // Act
            for (var i = 0; i < parametersTypes.Length; i++)
            {
                var type = parametersTypes[i];
                var currentValue = currentValues[i];

                // Assert
                Assert.AreEqual(currentValue, parametersDict[type]);
            }
        }

        [Description("Положительный тест метода расчетов с параметрами,"
                     + " которые влияют на другие параметры.")]
        [TestCase(
            ParametersEnum.DoorHeight,
            2000,
            2100,
            1900,
            new[] { ParametersEnum.PeepholeHeight, ParametersEnum.HandleHeight },
            new[] { 1800.0, 1930.0, 1500.0, 1000.0, 1000.0, 1000.0 })]
        [TestCase(
            ParametersEnum.DoorWidth,
            750,
            800,
            700,
            new[] { ParametersEnum.PeepholeWidth, ParametersEnum.HandleWidth },
            new[] { 375.0, 375.0, 375.0, 656.25, 656.25, 656.25 })]
        [TestCase(
            ParametersEnum.DoorThickness,
            50,
            80,
            40,
            new[] { ParametersEnum.HandleBaseThickness },
            new[] { 55.0, 55.0, 55.0 })]
        [TestCase(
            ParametersEnum.HandleDiameter,
            60,
            80,
            50,
            new[] { ParametersEnum.HandleBaseThickness },
            new[] { 60.0, 60.0, 60.0 })]
        public void ChangeParametersRangeValues_Parameter_UpdateRangeValues(
            ParametersEnum parameterType,
            double currentValue,
            double maxValue,
            double minValue,
            ParametersEnum[] parametersTypes,
            double[] expectedParameters)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(
                currentValue,
                minValue,
                maxValue);
            var step = 0;

            // Act
            parameters.ChangeParametersRangeValues(parameterType, parameter);

            // Assert
            foreach (var item in parametersTypes)
            {
                var currentParameter = parameters.ParametersDict[item];

                Assert.AreEqual(
                    expectedParameters[step],
                    currentParameter.CurrentValue);
                Assert.AreEqual(
                    expectedParameters[step + 1],
                    currentParameter.Max);
                Assert.AreEqual(
                    expectedParameters[step + 2],
                    currentParameter.Min);

                step += 3;
            }
        }

        [Description("Положительный тест метода расчетов с параметрами,"
                     + " которые не влияют на другие параметры.")]
        [TestCase(
            ParametersEnum.PeepholeHeight,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.PeepholeWidth,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.PeepholeDiameter,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.HandleHeight,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.HandleWidth,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.HandleBaseDiameter,
            409,
            541,
            387)]
        [TestCase(
            ParametersEnum.HandleBaseThickness,
            409,
            541,
            387)]
        [TestCase(ParametersEnum.HandleThickness,
            409,
            541,
            387)]
        [TestCase(ParametersEnum.HandleRecHeight,
            409,
            541,
            387)]
        [TestCase(ParametersEnum.HandleRecWidth,
            409,
            541,
            387)]
        public void ChangeParametersRangeValues_Parameter_NothingHappens(
            ParametersEnum parameterType,
            double currentValue,
            double maxValue,
            double minValue)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(
                currentValue,
                minValue,
                maxValue);

            // Act, Assert
            Assert.DoesNotThrow(() =>
                parameters.ChangeParametersRangeValues(parameterType, parameter));
        }

        [Description("Отрицательный тест метода расчетов с некорректными параметрами")]
        [TestCase(ParametersEnum.Unexpected, 12, 55, 3)]
        public void ChangeParametersRangeValues_Parameter_ThrowException(
            ParametersEnum parameterType,
            double currentValue,
            double maxValue,
            double minValue)
        {
            // Arrange
            var parameters = new Parameters();
            var parameter = new Parameter(
                currentValue,
                minValue,
                maxValue);

            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
                parameters.ChangeParametersRangeValues(parameterType, parameter));
        }
    }
}