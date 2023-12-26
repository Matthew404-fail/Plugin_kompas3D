namespace DoorPlugin.Wrapper
{
    using System;
    using System.Runtime.InteropServices;
    using Kompas6API5;
    using Kompas6Constants;
    using Kompas6Constants3D;

    /// <summary>
    /// Класс для подключения к Компасу.
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// Компонент исполнения.
        /// </summary>
        public ksPart Part { get; set; }

        /// <summary>
        /// Получает объект Компаса.
        /// </summary>
        public KompasObject Kompas { get; set; }

        /// <summary>
        /// Подключается к активной сессии Компаса.
        /// </summary>
        /// <returns>True, если подключение успешно.
        /// В противном случае - false.</returns>
        public void ConnectToKompas()
        {
            try
            {
                Kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
                Console.WriteLine("Уже подключено к активной сессии Компаса");
            }
            catch (COMException)
            {
                try
                {
                    Kompas = (KompasObject)Activator.CreateInstance(
                        Type.GetTypeFromProgID("KOMPAS.Application.5"));
                    Kompas.Visible = true;
                    Kompas.ActivateControllerAPI();

                    Console.WriteLine("Успешно подключено к активной сессии Компаса");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при подключении к активной сессии Компаса: "
                                      + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Неожиданная ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Создает 3D-документ Компаса.
        /// </summary>
        /// <returns>Объект 3D-документа Компаса.</returns>
        public ksDocument3D CreateDocument3D()
        {
            ksDocument3D document3D = Kompas.Document3D();

            document3D.Create();
            Part = document3D.GetPart((int)Part_Type.pTop_Part);

            return document3D;
        }

        /// <summary>
        /// Создает смещенную плоскость относительно другой плоскости.
        /// </summary>
        /// <param name="plane">Тип базовой плоскости.</param>
        /// <param name="offset">Величина смещения.</param>
        /// <returns>Экземпляр смещенной плоскости.</returns>
        public ksEntity CreateOffsetPlane(Obj3dType plane, double offset)
        {
            var offsetEntity = (ksEntity)Part.
                NewEntity((short)Obj3dType.o3d_planeOffset);
            var offsetDef = (ksPlaneOffsetDefinition)offsetEntity.
                GetDefinition();

            offsetDef.SetPlane((ksEntity)Part.NewEntity((short)plane));
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
            var plane = (ksEntity)Part.
                GetDefaultEntity((short)planeType);
            var sketch = (ksEntity)Part.
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
            var extrusionEntity = (ksEntity)Part.
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
            var cutExtrusionEntity = (ksEntity)Part.NewEntity(
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
        /// Метод рисования прямоугольника.
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
                (ksRectangleParam)Kompas.GetParamStruct
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