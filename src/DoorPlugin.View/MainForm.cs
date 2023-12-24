namespace DoorPlugin.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using DoorPlugin.Wrapper;
    using DoorPlugin.Model;

    /// <summary>
    /// Главная форма приложения.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Строка обозначающая textBox.
        /// </summary>
        private readonly string _textBox = "textBox";

        /// <summary>
        /// Строка обозначающая rangeLabel.
        /// </summary>
        private readonly string _rangeLabel = "rangeLabel";

        /// <summary>
        /// Строка обозначающая errorLabel.
        /// </summary>
        private readonly string _errorLabel = "errorLabel";

        /// <summary>
        /// Цвет по умолчанию.
        /// </summary>
        private readonly Color _defaultColor = Color.White;

        /// <summary>
        /// Цвет для обозначения ошибок.
        /// </summary>
        private readonly Color _errorColor =
            Color.FromArgb(255, 192, 192);

        /// <summary>
        /// Экземпляр строителя.
        /// </summary>
        private readonly Builder _builder = new Builder();

        /// <summary>
        /// Экземпляр класса параметров.
        /// </summary>
        private readonly Parameters _parameters = new Parameters();

        /// <summary>
        /// Словарь элементов формы необходимые для отображения параметров.
        /// </summary>
        private readonly Dictionary<ParametersEnum, Dictionary<string, Control>>
            _controls = new Dictionary<ParametersEnum, Dictionary<string, Control>>();

        /// <summary>
        /// Индекс типа цилиндрообразной ручки.
        /// </summary>
        private readonly int _cylinderHandleIndex = 0;

        /// <summary>
        /// Инициализирует новый экземпляр класса MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox_HandleType.SelectedIndex = _cylinderHandleIndex;

            SetControls();
            SetTextFormElements();
        }

        /// <summary>
        /// Задает элементы управления формы для каждого параметра.
        /// </summary>
        private void SetControls()
        {
            _controls.Clear();
            _controls.Add(
                ParametersEnum.DoorHeight,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_DoorHeight },
                    { _rangeLabel, label_DoorHeight },
                    { _errorLabel, label_ErrorDoorHeight },
                });
            _controls.Add(
                ParametersEnum.DoorWidth,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_DoorWidth },
                    { _rangeLabel, label_DoorWidth },
                    { _errorLabel, label_ErrorDoorWidth },
                });
            _controls.Add(
                ParametersEnum.DoorThickness,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_DoorThickness },
                    { _rangeLabel, label_DoorThickness },
                    { _errorLabel, label_ErrorDoorThickness },
                });
            _controls.Add(
                ParametersEnum.PeepholeHeight,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_PeepholeHeight },
                    { _rangeLabel, label_PeepholeHeight },
                    { _errorLabel, label_ErrorPeepholeHeight },
                });
            _controls.Add(
                ParametersEnum.PeepholeDiameter,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_PeepholeDiameter },
                    { _rangeLabel, label_PeepholeDiameter },
                    { _errorLabel, label_ErrorPeepholeDiameter },
                });
            _controls.Add(
                ParametersEnum.HandleBaseDiameter,
                new Dictionary<string, Control>
                {
                    { _textBox, textBox_HandleBaseDiameter },
                    { _rangeLabel, label_HandleBaseDiameter },
                    { _errorLabel, label_ErrorHandleBaseDiameter },
                });

            if (comboBox_HandleType.SelectedIndex == _cylinderHandleIndex)
            {
                _controls.Add(
                    ParametersEnum.HandleDiameter,
                    new Dictionary<string, Control>
                    {
                        { _textBox, textBox_HandleDiameter },
                        { _rangeLabel, label_HandleDiameter },
                        { _errorLabel, label_ErrorHandleDiameter },
                    });
                _parameters.IsHandleCylinder = true;
                ChangeHandleControlsVisibility(_parameters.IsHandleCylinder);
            }
            else
            {
                _controls.Add(
                    ParametersEnum.HandleRecHeight,
                    new Dictionary<string, Control>
                    {
                        { _textBox, textBox_HandleRecHeight },
                        { _rangeLabel, label_HandleRecHeight },
                        { _errorLabel, label_ErrorHandleRecHeight },
                    });
                _controls.Add(
                    ParametersEnum.HandleRecWidth,
                    new Dictionary<string, Control>
                    {
                        { _textBox, textBox_HandleRecWidth },
                        { _rangeLabel, label_HandleRecWidth },
                        { _errorLabel, label_ErrorHandleRecWidth },
                    });

                _parameters.IsHandleCylinder = false;
                ChangeHandleControlsVisibility(_parameters.IsHandleCylinder);
            }
        }

        /// <summary>
        /// Устанавливает текст в контролы на основе текущих параметров.
        /// </summary>
        private void SetTextFormElements()
        {
            foreach (var item in _parameters.ParametersDict)
            {
                // TODO: key и value (+)
                var key = item.Key;
                var value = item.Value;

                try
                {
                    _controls[key][_textBox].Text =
                        value.CurrentValue.ToString();
                    _controls[key][_rangeLabel].Text =
                        $"от {value.Min} до {value.Max}";
                }
                catch (KeyNotFoundException e)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Обработчик изменения текста в textBox.
        /// </summary>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var textBoxName = textBox.Name;
                var parameterType = ParametersEnum.Unexpected;

                var parameterTypeString =
                    textBoxName.Split('_')[1];

                // Ищет нужный textBox в _controls.
                foreach (var controlsItem in _controls.Keys)
                {
                    if (controlsItem.ToString() == parameterTypeString)
                    {
                        parameterType = controlsItem;
                        break;
                    }
                }

                // Установка нового значения для параметра.
                try
                {
                    _parameters.CheckParameter(
                        parameterType,
                        _parameters.ParametersDict[parameterType],
                        Convert.ToDouble(textBox.Text));
                    SetTextFormElements();
                    _controls[parameterType][_errorLabel].
                        Text = "";
                    _controls[parameterType][_errorLabel].
                        BackColor = _defaultColor;
                    buttonBuild.Enabled = true;
                }
                catch (Exception ex)
                {
                    _controls[parameterType][_errorLabel].
                        Text = ex.Message;
                    _controls[parameterType][_errorLabel].
                        BackColor = _errorColor;
                    buttonBuild.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Меняет видимость контролов типа ручки.
        /// </summary>
        /// <param name="isCylinder">Является ли ручка цилиндром.</param>
        private void ChangeHandleControlsVisibility(bool isCylinder)
        {
            if (isCylinder == true)
            {
                textBox_HandleDiameter.Visible = true;
                label_NameHandleDiameter.Visible = true;
                label_HandleDiameter.Visible = true;
                label_ErrorHandleDiameter.Visible = true;

                label_NameHandleRecHeight.Visible = false;
                textBox_HandleRecHeight.Visible = false;
                label_HandleRecHeight.Visible = false;
                label_ErrorHandleRecHeight.Visible = false;

                label_NameHandleRecWidth.Visible = false;
                textBox_HandleRecWidth.Visible = false;
                label_HandleRecWidth.Visible = false;
                label_ErrorHandleRecWidth.Visible = false;
            }
            else
            {
                textBox_HandleDiameter.Visible = false;
                label_NameHandleDiameter.Visible = false;
                label_HandleDiameter.Visible = false;
                label_ErrorHandleDiameter.Visible = false;

                label_NameHandleRecHeight.Visible = true;
                textBox_HandleRecHeight.Visible = true;
                label_HandleRecHeight.Visible = true;
                label_ErrorHandleRecHeight.Visible = true;

                label_NameHandleRecWidth.Visible = true;
                textBox_HandleRecWidth.Visible = true;
                label_HandleRecWidth.Visible = true;
                label_ErrorHandleRecWidth.Visible = true;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Построить".
        /// </summary>
        private void ButtonBuild_Click(object sender, EventArgs e)
        {
            _builder.BuildDetail(
                _parameters.GetParametersCurrentValues(),
                _parameters.IsHandleCylinder);
        }

        /// <summary>
        /// Обработчик комбобокса выбора типа ручки.
        /// </summary>
        private void ComboBox_HandleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetControls();
            SetTextFormElements();
        }
    }
}