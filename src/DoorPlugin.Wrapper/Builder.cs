namespace DoorPlugin.Wrapper
{
    using System.Collections.Generic;
    using Kompas6API5;
    using Kompas6Constants3D;
    using DoorPlugin.Model;

    /// <summary>
    /// Описывает строитель.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Экземпляр коннектора.
        /// </summary>
        private readonly Connector _connector = new Connector();

        /// <summary>
        /// Множитель заступа длины ручки на основание.
        /// </summary>
        private readonly double _handleRecHeightMultiplier = 0.15;

        /// <summary>
        /// Множитель заступа ширины ручки на основание.
        /// </summary>
        private readonly double _handleRecWidthMultiplier = 2;

        /// <summary>
        /// Словарь текущих значений всех параметров.
        /// </summary>
        public Dictionary<ParametersEnum, double> ParametersDict { get; set; }

        /// <summary>
        /// Строит деталь в Компасе.
        /// </summary>
        /// <param name="parametersDict">Словарь текущих значений всех параметров.</param>
        public void BuildDetail(
            Dictionary<ParametersEnum, double> parametersDict,
            bool isHandleCylinder)
        {
            _connector.ConnectToKompas();
            _connector.CreateDocument3D();

            ParametersDict = parametersDict;

            BuildFoundation();
            BuildBaseHandle();

            if (isHandleCylinder == true)
            {
                BuildCylinderHandle();
            }
            else
            {
                BuildRectangleHandle();
            }

            BuildPeephole();
        }

        /// <summary>
        /// Строит основу двери.
        /// </summary>
        private void BuildFoundation()
        {
            var sketch = _connector.CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            var recParams = _connector.RectangleParams(
                0,
                0,
                -ParametersDict[ParametersEnum.DoorHeight],
                ParametersDict[ParametersEnum.DoorWidth]);

            document2d.ksRectangle(recParams);
            sketch.EndEdit();

            _connector.CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.DoorThickness],
                true);
        }

        /// <summary>
        /// Строит основание рукоятки.
        /// </summary>
        private void BuildBaseHandle()
        {
            var sketch = _connector.CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.HandleWidth],
                -ParametersDict[ParametersEnum.HandleHeight],
                ParametersDict[ParametersEnum.HandleBaseDiameter] / 2,
                1);
            sketch.EndEdit();

            _connector.CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.HandleBaseThickness],
                false);
        }

        /// <summary>
        /// Строит рукоятку.
        /// </summary>
        private void BuildCylinderHandle()
        {
            var offsetThicknessEntity = _connector.CreateOffsetPlane(
                Obj3dType.o3d_planeXOZ,
                ParametersDict[ParametersEnum.HandleBaseThickness]);
            var sketch = _connector.CreateSketch(Obj3dType.o3d_planeXOZ, offsetThicknessEntity);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.HandleWidth],
                -ParametersDict[ParametersEnum.HandleHeight],
                ParametersDict[ParametersEnum.HandleDiameter] / 2,
                1);
            sketch.EndEdit();

            _connector.CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.HandleThickness],
                false);
        }

        /// <summary>
        /// Строит прямоугольную рукоятку.
        /// </summary>
        private void BuildRectangleHandle()
        {
            var offsetThicknessEntity = _connector.CreateOffsetPlane(
                Obj3dType.o3d_planeXOZ,
                ParametersDict[ParametersEnum.HandleBaseThickness]);
            var sketch = _connector.CreateSketch(Obj3dType.o3d_planeXOZ, offsetThicknessEntity);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            var x = ParametersDict[ParametersEnum.HandleWidth] +
                    (ParametersDict[ParametersEnum.HandleRecHeight] *
                     _handleRecHeightMultiplier);
            var y = -ParametersDict[ParametersEnum.HandleHeight] +
                    (ParametersDict[ParametersEnum.HandleRecWidth] /
                     _handleRecWidthMultiplier);

            var recParams = _connector.RectangleParams(
                x,
                y,
                -ParametersDict[ParametersEnum.HandleRecWidth],
                -ParametersDict[ParametersEnum.HandleRecHeight]);

            document2d.ksRectangle(recParams);
            sketch.EndEdit();

            _connector.CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.HandleThickness],
                false);
        }

        /// <summary>
        /// Строит глазок.
        /// </summary>
        private void BuildPeephole()
        {
            var sketch = _connector.CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.PeepholeWidth],
                -ParametersDict[ParametersEnum.PeepholeHeight],
                ParametersDict[ParametersEnum.PeepholeDiameter] / 2,
                1);
            sketch.EndEdit();

            _connector.СreateCutExtrusionThroughAll(sketch, false);
        }
    }
}