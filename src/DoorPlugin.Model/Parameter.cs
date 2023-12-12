namespace DoorPlugin.Model
{
    /// <summary>
    /// Описывает параметр.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Получает или задает текущее значение параметра.
        /// </summary>
        public double Current { get; set; }

        /// <summary>
        /// Получает или задает максимальное значение параметра.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Получает или задает минимальное значение параметра.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public Parameter()
        {
            Current = 60;
            Max = 200;
            Min = 50;
        }

        /// <summary>
        /// Конструктор с вводимыми значениями.
        /// </summary>
        /// <param name="current">Текущее значение.</param>
        /// <param name="min">Минимальное значение.</param>
        /// <param name="max"> Максимальное значение.</param>
        public Parameter(
            double current,
            double min,
            double max)
        {
            Validator.ValidateParameter(current, min, max);

            Current = current;
            Max = max;
            Min = min;
        }
    }
}
