using Experior.Catalog.Joints.Assemblies.Mechanisms;
using System;

namespace Experior.Plugin.Joints.GUI.Chart
{
    public class ChartWindowHandler
    {
        #region Fields

        private ChartWindow _chartWindow; // GUI
        private ChartConfigurator _chartConfigurator;
        private DataHandler _dataHandler;

        private readonly IMechanism _mechanism;

        #endregion

        #region Constructor

        public ChartWindowHandler(IMechanism assembly)
        {
            _mechanism = assembly;
        }

        #endregion

        #region Events

        public EventHandler OnClosed;

        #endregion

        #region Public Methods

        public void Show()
        {
            _chartWindow = new ChartWindow(_mechanism);
            _chartWindow.Closed += WindowOnClosed;

            _chartWindow.Show();

            _chartConfigurator = new ChartConfigurator(_chartWindow, _mechanism);
            _dataHandler = new DataHandler(_mechanism, _chartWindow, _chartConfigurator);
        }

        #endregion

        #region Private Methods

        private void WindowOnClosed(object sender, EventArgs e)
        {
            _chartWindow.Closed -= WindowOnClosed;

            _dataHandler.DisposeData();

            _chartConfigurator = null;
            _dataHandler = null;

            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
