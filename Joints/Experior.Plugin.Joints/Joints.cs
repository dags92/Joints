using System.Windows.Media;
using System.Reflection;
using Experior.Core.Resources;
using Experior.Plugin.Joints.Ribbon;

namespace Experior.Plugin.Joints
{
    public class Joints : Experior.Core.Plugin
    {
        #region Fields

        private ExcelPanel _excel;
        private ChartPanel _chart;

        #endregion

        #region Constructor

        public Joints() : base("Joints")
        {
            Common.Icon = new EmbeddedImageLoader(Assembly.GetExecutingAssembly());

            if (!Experior.Core.Environment.Engine.Physics)
            {
                return;
            }

            Init();
        }

        #endregion

        #region Properties

        public override ImageSource Logo => Common.Icon.Get("Logo");

        #endregion

        #region Private Methods

        private void Init()
        {
            _chart = new ChartPanel();
            _excel = new ExcelPanel();
        }

        #endregion

        #region Nested Types

        public class Common
        {
            public static EmbeddedImageLoader Icon;
        }

        #endregion
    }
}
