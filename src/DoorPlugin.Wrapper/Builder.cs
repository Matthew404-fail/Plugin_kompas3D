namespace DoorPlugin.Wrapper
{
    using System;
    using System.Collections.Generic;
    using Kompas6API5;
    using Kompas6Constants3D;
    using DoorPlugin.Model;
    using Kompas6Constants;

    /// <summary>
    /// Описывает строитель.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Словарь текущих значений всех параметров.
        /// </summary>
        public Dictionary<ParametersEnum, double> ParametersDict;

        /// <summary>
        /// Документ в Компасе.
        /// </summary>
        public ksDocument3D Doc3D;

        /// <summary>
        /// Экземпляр коннектора.
        /// </summary>
        private readonly Connector _connector = new Connector();

        /// <summary>
        /// Создает новое подключению к компасу.
        /// </summary>
        /// <exception cref="ArgumentException">Выбрасывает исключение, если
        /// невозможно подключиться к Компасу.</exception>
        public void CheckOrCreateKompasConnection()
        {
            if (!_connector.ConnectToKompas())
            {
                throw new ArgumentException("Не удалось подключиться к Компасу.");
            }
        }

        /// <summary>
        /// Создает новый документ.
        /// </summary>
        public void CreateNewDocument()
        {
            Doc3D = _connector.CreateDocument3D();
            Doc3D.Create();
        }

        /// <summary>
        /// Строит деталь в Компасе.
        /// </summary>
        /// <param name="parametersDict">Словарь текущих значений всех параметров.</param>
        public void BuildDetail(Dictionary<ParametersEnum, double> parametersDict)
        {
            ParametersDict = parametersDict;

            BuildFoundation();
            BuildBaseHandle();
            BuildHandle();
            BuildPeephole();
        }

        /// <summary>
        /// Строит основу двери.
        /// </summary>
        private void BuildFoundation()
        {
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            var recParams = RectangleParams(
                0,
                0,
                -ParametersDict[ParametersEnum.DoorHeight],
                ParametersDict[ParametersEnum.DoorWidth]);

            document2d.ksRectangle(recParams);
            sketch.EndEdit();

            CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.DoorThickness],
                true);
        }

        /// <summary>
        /// Строит основание рукоятки.
        /// </summary>
        private void BuildBaseHandle()
        {
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.HandleWidth],
                -ParametersDict[ParametersEnum.HandleHeight],
                ParametersDict[ParametersEnum.HandleBaseDiameter] / 2,
                1);
            sketch.EndEdit();

            CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.HandleBaseThickness],
                false);
        }

        /// <summary>
        /// Строит рукоятку.
        /// </summary>
        private void BuildHandle()
        {
            var offsetThicknessEntity = CreateOffsetPlane(
                Obj3dType.o3d_planeXOZ,
                ParametersDict[ParametersEnum.HandleBaseThickness]);
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ, offsetThicknessEntity);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.HandleWidth],
                -ParametersDict[ParametersEnum.HandleHeight],
                ParametersDict[ParametersEnum.HandleDiameter] / 2,
                1);
            sketch.EndEdit();

            CreateExtrusion(
                sketch,
                ParametersDict[ParametersEnum.HandleThickness],
                false);
        }

        /// <summary>
        /// Строит глазок.
        /// </summary>
        private void BuildPeephole()
        {
            var sketch = CreateSketch(Obj3dType.o3d_planeXOZ, null);
            var document2d = (ksDocument2D)sketch.BeginEdit();

            document2d.ksCircle(
                ParametersDict[ParametersEnum.PeepholeWidth],
                -ParametersDict[ParametersEnum.PeepholeHeight],
                ParametersDict[ParametersEnum.PeepholeDiameter] / 2,
                1);
            sketch.EndEdit();

            СreateCutExtrusionThroughAll(sketch, false);
        }

        // TODO: вынести в Connector методы, которые связаны с API Компаса
        /// <summary>
        /// Создает смещенную плоскость относительно другой плоскости.
        /// </summary>
        /// <param name="plane">Тип базовой плоскости.</param>
        /// <param name="offset">Величина смещения.</param>
        /// <returns>Экземпляр смещенной плоскости.</returns>
        public ksEntity CreateOffsetPlane(Obj3dType plane, double offset)
        {
            var offsetEntity = (ksEntity)_connector.Part.
                NewEntity((short)Obj3dType.o3d_planeOffset);
            var offsetDef = (ksPlaneOffsetDefinition)offsetEntity.
                GetDefinition();

            offsetDef.SetPlane((ksEntity)_connector.Part.NewEntity((short)plane));
            offsetDef.offset = offset;
            offsetDef.direction = false;
            offsetEntity.Create();

            return offsetEntity;
        }

        /// <summary>
        /// Создает эскиз на заданной плоскости.
        /// </summary>
        /// <param name="planeType">Тип плоскости, на которой создается
        /// эскиз.</param>
        /// <param name="offsetPlane">Смещенная плоскость
        /// (может быть null).</param>
        /// <returns>Определение созданного эскиза.</returns>
        public ksSketchDefinition CreateSketch(
            Obj3dType planeType,
            ksEntity offsetPlane)
        {
            var plane = (ksEntity)_connector.Part.
                GetDefaultEntity((short)planeType);
            var sketch = (ksEntity)_connector.Part.
                NewEntity((short)Obj3dType.o3d_sketch);
            var ksSketch = (ksSketchDefinition)sketch.GetDefinition();

            if (offsetPlane != null)
            {
                ksSketch.SetPlane(offsetPlane);
                sketch.Create();
                return ksSketch;
            }

            ksSketch.SetPlane(plane);
            sketch.Create();
            return ksSketch;
        }

        /// <summary>
        /// Создает выдавливание на основе эскиза.
        /// </summary>
        /// <param name="sketch">Эскиз, на основе которого создается
        /// выдавливание.</param>
        /// <param name="depth">Глубина выдавливания.</param>
        /// <param name="side">Направление выдавливания
        /// (true - в одну сторону, false - в обратную).</param>
        /// <returns>Определение созданного выдавливания.</returns>
        public ksBossExtrusionDefinition CreateExtrusion(
            ksSketchDefinition sketch,
            double depth,
            bool side = true)
        {
            var extrusionEntity = (ksEntity)_connector.Part.
                NewEntity((short)ksObj3dTypeEnum.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusionEntity.
                GetDefinition();

            extrusionDef.SetSideParam(side, (short)End_Type.etBlind, depth);
            extrusionDef.directionType =
                side ? (short)Direction_Type.dtNormal :
                    (short)Direction_Type.dtReverse;
            extrusionDef.SetSketch(sketch);
            extrusionEntity.Create();

            return extrusionDef;
        }

        /// <summary>
        /// Метод осуществляющий вырезание через все поверхности.
        /// </summary>
        /// <param name="sketch">Эскиз</param>
        public void СreateCutExtrusionThroughAll(
            ksSketchDefinition sketch,
            bool side = true)
        {
            var cutExtrusionEntity = (ksEntity)_connector.Part.NewEntity(
                (short)ksObj3dTypeEnum.o3d_cutExtrusion);
            var cutExtrusionDef =
                (ksCutExtrusionDefinition)cutExtrusionEntity
                    .GetDefinition();

            cutExtrusionDef.SetSideParam(side,
                (short)End_Type.etThroughAll);
            cutExtrusionDef.directionType = side ?
                (short)Direction_Type.dtNormal :
                (short)Direction_Type.dtReverse;
            cutExtrusionDef.cut = true;
            cutExtrusionDef.SetSketch(sketch);

            cutExtrusionEntity.Create();
        }

        /// <summary>
        /// Метод рисования прямоугольника
        /// </summary>
        /// <param name="x">X базовой точки</param>
        /// <param name="y">Y базовой точки</param>
        /// <param name="height">Высота</param>
        /// <param name="width">Ширина</param>
        /// <returns>Переменная с параметрами прямоугольника</returns>
        public ksRectangleParam RectangleParams(
            double x,
            double y,
            double height,
            double width)
        {
            var rectangleParam =
                (ksRectangleParam)_connector.Kompas.GetParamStruct
                    ((short)StructType2DEnum.ko_RectangleParam);
            rectangleParam.x = x;
            rectangleParam.y = y;
            rectangleParam.height = height;
            rectangleParam.width = width;
            rectangleParam.style = 1;
            return rectangleParam;
        }
    }
}
