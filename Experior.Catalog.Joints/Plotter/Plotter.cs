using Experior.Catalog.Joints.Plotter.UI;
using Experior.Interfaces;

namespace Experior.Catalog.Joints.Plotter
{
    public class Plotter
    {
        #region Fields

        private PlotterWindow _window;
        private Configurator _configurator;

        #endregion

        #region Constructor

        public Plotter()
        {

        }

        #endregion

        #region Public Properties



        #endregion

        #region Public Methods

        public void Show()
        {
            Experior.Core.Environment.InvokeUiAction(() =>
            {
                _window = new PlotterWindow();
                _window.Show();

                if (_window.Chart == null)
                {
                    Log.Write($"Chart has not been instantiated", System.Windows.Media.Colors.Red, LogFilter.Error);
                    return;
                }

                _configurator = new Configurator(_window.Chart);
            });
        }

        #endregion
    }
}
