namespace DoorPlugin.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Описывает параметры.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Минимальный отступ глазка от высоты.
        /// </summary>
        private readonly double _peepholeHeightMinOffset = 0.75;

        /// <summary>
        /// Максимальный отступ глазка от высоты.
        /// </summary>
        private readonly int _peepholeHeightMaxOffset = 70;

        /// <summary>
        /// Отступ рукоятки.
        /// </summary>
        private readonly int _handleHeightOffset = 2;

        /// <summary>
        /// Отступ глазка по горизонтали.
        /// </summary>
        private readonly int _peepholeWidthOffset = 2;

        /// <summary>
        /// Множитель ширины рукоятки.
        /// </summary>
        private readonly double _handleWidthMultiplier = 0.875;

        /// <summary>
        /// Выступ рукоятки.
        /// </summary>
        private readonly int _handleBaseThicknessOffset = 2;

        /// <summary>
        /// Словарь, тип параметра - параметр.
        /// </summary>
        public Dictionary<ParametersEnum, Parameter> ParametersDict;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameters со значениями
        /// по умолчанию.
        /// </summary>
        public Parameters()
        {
            ParametersDict = new Dictionary<ParametersEnum, Parameter>
            {
                { ParametersEnum.DoorHeight, new Parameter(2000, 1900, 2100) },
                { ParametersEnum.DoorWidth, new Parameter(750, 700, 800) },
                { ParametersEnum.DoorThickness, new Parameter(50, 40, 80) },
                { ParametersEnum.PeepholeHeight, new Parameter(1800, 1500, 1970) },
                { ParametersEnum.PeepholeWidth, new Parameter(375, 375, 375) },
                { ParametersEnum.PeepholeDiameter, new Parameter(40, 30, 70) },
                { ParametersEnum.HandleHeight, new Parameter(1000, 1000, 1000) },
                { ParametersEnum.HandleWidth, new Parameter(656.25, 656.25, 656.25) },
                { ParametersEnum.HandleBaseDiameter, new Parameter(30, 25, 35) },
                { ParametersEnum.HandleDiameter, new Parameter(60, 50, 80) },
                { ParametersEnum.HandleBaseThickness, new Parameter(55, 55, 55) },
                { ParametersEnum.HandleThickness, new Parameter(15, 15, 15) }
            };
        }

        /// <summary>
        /// Получает словарь текущих значений параметров.
        /// </summary>
        /// <returns>Словарь, тип параметра - текущее значение параметра.</returns>
        public Dictionary<ParametersEnum, double> GetParametersCurrentValues()
        {
            var parametersCurrentValues = new Dictionary<ParametersEnum, double>();

            foreach (var item in ParametersDict)
            {
                var key = item.Key;
                var value = item.Value;

                parametersCurrentValues.Add(key, value.Current);
            }

            return parametersCurrentValues;
        }

        /// <summary>
        /// Проверяет параметр.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <param name="parameter">Экземпляр параметра.</param>
        /// <param name="value">Значение для присвоения параметру.</param>
        public void CheckParameter(
            ParametersEnum parameterType,
            Parameter parameter,
            double value)
        {
            Validator.ValidateParameter(value, parameter.Min, parameter.Max);
            ParametersDict[parameterType].Current = value;
            ChangeParametersRangeValues(parameterType, parameter);
        }

        /// <summary>
        /// Изменяет граничные значения параметров.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <param name="parameter">Экземпляр параметра.</param>
        /// <exception cref="ArgumentException">Вызывает исключение
        /// при неверно введенном типе параметра.</exception>
        public void ChangeParametersRangeValues(
            ParametersEnum parameterType,
            Parameter parameter)
        {
            switch (parameterType)
            {
                case ParametersEnum.DoorHeight:
                    ParametersDict[ParametersEnum.PeepholeHeight].
                        Min = parameter.Current * _peepholeHeightMinOffset;
                    ParametersDict[ParametersEnum.PeepholeHeight].
                        Max = parameter.Current - _peepholeHeightMaxOffset;

                    ParametersDict[ParametersEnum.HandleHeight].
                        Current = parameter.Current / _handleHeightOffset;
                    ParametersDict[ParametersEnum.HandleHeight].
                        Min = parameter.Current / _handleHeightOffset;
                    ParametersDict[ParametersEnum.HandleHeight].
                        Max = parameter.Current / _handleHeightOffset;
                    break;

                case ParametersEnum.DoorWidth:
                    ParametersDict[ParametersEnum.PeepholeWidth].
                        Current = parameter.Current / _peepholeWidthOffset;
                    ParametersDict[ParametersEnum.PeepholeWidth].
                        Min = parameter.Current / _peepholeWidthOffset;
                    ParametersDict[ParametersEnum.PeepholeWidth].
                        Max = parameter.Current / _peepholeWidthOffset;

                    ParametersDict[ParametersEnum.HandleWidth].
                        Current = parameter.Current * _handleWidthMultiplier;
                    ParametersDict[ParametersEnum.HandleWidth].
                        Min = parameter.Current * _handleWidthMultiplier;
                    ParametersDict[ParametersEnum.HandleWidth].
                        Max = parameter.Current * _handleWidthMultiplier;
                    break;

                case ParametersEnum.DoorThickness:
                    ParametersDict[ParametersEnum.HandleBaseThickness].Current =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    ParametersDict[ParametersEnum.HandleBaseThickness].Min =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    ParametersDict[ParametersEnum.HandleBaseThickness].Max =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    break;

                case ParametersEnum.HandleDiameter:
                    ParametersDict[ParametersEnum.HandleBaseThickness].Current =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    ParametersDict[ParametersEnum.HandleBaseThickness].Min =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    ParametersDict[ParametersEnum.HandleBaseThickness].Max =
                        (parameter.Current / _handleBaseThicknessOffset) +
                        (ParametersDict[ParametersEnum.HandleDiameter].
                            Current / 2);
                    break;

                case ParametersEnum.HandleBaseThickness:
                case ParametersEnum.HandleThickness:
                case ParametersEnum.PeepholeHeight:
                case ParametersEnum.PeepholeWidth:
                case ParametersEnum.PeepholeDiameter:
                case ParametersEnum.HandleHeight:
                case ParametersEnum.HandleWidth:
                case ParametersEnum.HandleBaseDiameter:
                    break;

                default:
                    var message = "Введен некорректный тип параметра";
                    throw new ArgumentException(message);
            }
        }
    }
}
