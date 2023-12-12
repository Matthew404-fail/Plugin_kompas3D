namespace DoorPlugin.Model
{
    using System;

    /// <summary>
    /// Статический класс, проверяющий значения параметров.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Проверяет параметр на правильность.
        /// </summary>
        /// <param name="current">Текущее значение.</param>
        /// <param name="min">Максимальное значение.</param>
        /// <param name="max">Минимальное значение.</param>
        /// <exception cref="ArgumentException">Выбрасывает исключение, если
        /// значения параметра некорректные</exception>
        public static void ValidateParameter(double current, double min, double max)
        {
            if (min < 0)
            {
                var message = "Минимальное значение не может быть меньше нуля.";
                throw new ArgumentException(message);
            }

            if (min > max)
            {
                var message = "Минимальное значение не может быть больше максимального значения.";
                throw new ArgumentException(message);
            }

            if (current < min || current > max)
            {
                var message = "Текущее значение вне диапазона возможных значений.";
                throw new ArgumentException(message);
            }
        }
    }
}
