using System;
using Experior.Catalog.Joints.Assemblies;
using Experior.Core.Assemblies;
using Experior.Plugin.Joints.GUI.Chart;
using Environment = Experior.Core.Environment;

namespace Experior.Plugin.Joints.Ribbon
{
    public class ChartPanel
    {
        #region Fields

        private Environment.UI.Toolbar.Button _chartButton;
        private ChartWindowHandler _chartWindowHandler;

        #endregion

        #region Constructor

        public ChartPanel()
        {
            Init();
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            _chartButton = new Environment.UI.Toolbar.Button("Chart", Joints.Common.Icon.Get("Graph.png"))
            {
                Enabled = true,
                OnClick = ChartButtonOnClick
            };

            Environment.UI.Toolbar.Add(_chartButton, "Joints", "Visualization");
        }

        private void ChartButtonOnClick(object sender, EventArgs e)
        {
            Environment.InvokeUiAction(() =>
            {
                var assembly = FetchSelectedAssembly();
                if (assembly == null)
                {
                    Log.Write("Selected assembly is not of type Joint"); //TODO: CHANGE THIS !!
                    return;
                }

                if (_chartWindowHandler != null)
                {
                    return;
                }

                _chartWindowHandler = new ChartWindowHandler(assembly);
                _chartWindowHandler.OnClosed += WindowOnClosed;
                _chartWindowHandler.Show();
            });
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            _chartWindowHandler.OnClosed -= WindowOnClosed;
            _chartWindowHandler = null;
        }

        private IMechanism FetchSelectedAssembly()
        {
            foreach (var item in Assembly.Items)
            {
                if (item.Selected && item is IMechanism mechanism)
                {
                    return mechanism;
                }
            }

            return null;
        }

        #endregion
    }
}