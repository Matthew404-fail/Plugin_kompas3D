namespace DoorPlugin.Wrapper
{
    using System;
    using System.Runtime.InteropServices;
    using Kompas6API5;
    using Kompas6Constants3D;

    /// <summary>
    /// Класс для подключения к Компасу.
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// Компонент исполнения.
        /// </summary>
        public ksPart Part;

        /// <summary>
        /// Получает объект Компаса.
        /// </summary>
        public KompasObject Kompas { get; private set; }

        /// <summary>
        /// Подключается к активной сессии Компаса.
        /// </summary>
        /// <returns>True, если подключение успешно.
        /// В противном случае - false.</returns>
        public bool ConnectToKompas()
        {
            try
            {
                Kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
                Console.WriteLine("Уже подключено к активной сессии Компаса");
                return true;
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
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при подключении к активной сессии Компаса: "
                                      + ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Неожиданная ошибка: " + ex.Message);
                return false;
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
    }
}
