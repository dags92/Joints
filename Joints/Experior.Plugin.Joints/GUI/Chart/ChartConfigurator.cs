using System.Windows.Forms.DataVisualization.Charting;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Catalog.Joints.Assemblies.Mechanisms;

namespace Experior.Plugin.Joints.GUI.Chart
{
    public class ChartConfigurator
    {
        #region Fields

        private readonly ChartWindow _window;
        private readonly System.Windows.Forms.DataVisualization.Charting.Chart _chart;
        private Series _dataPoints;

        private readonly IMechanism _mechanism;

        #endregion

        #region Constructor

        public ChartConfigurator(ChartWindow window, IMechanism mechanism)
        {
            _mechanism = mechanism;

            _window = window;
            _chart = window.Chart;

            _window.ChartDataTypeModified += (sender, dataType) => UpdateTitle(dataType);
            Init();
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            _chart.AntiAliasing = AntiAliasingStyles.Graphics;

            // X-axis
            _chart.ChartAreas[0].AxisX.RoundAxisValues();
            _chart.ChartAreas[0].AxisX.Title = "Time (seconds)";
            _chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            _chart.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            // Y-Axes:
            _chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            _chart.ChartAreas[0].AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dash;

            UpdateTitle(ChartWindow.ChartDataTypes.Angular_Velocity);
        }

        public Series CreateSeries()
        {
            _dataPoints = new Series(_mechanism.Name);

            if (!_chart.Series.Contains(_dataPoints))
            {
                _chart.Series.Add(_dataPoints);
            }

            _dataPoints.ChartType = SeriesChartType.Line;

            return _dataPoints;
        }

        public void UpdateTitle(ChartWindow.ChartDataTypes value)
        {
            string title;
            switch (value)
            {
                case ChartWindow.ChartDataTypes.Linear_Velocity:
                    title = "Linear Velocity (m/s)";
                    break;

                case ChartWindow.ChartDataTypes.Linear_Force:
                    title = "Linear Force (N)";
                    break;

                case ChartWindow.ChartDataTypes.Angular_Force:
                    title = "Angular Force (N*m)";
                    break;

                default:
                    title = "Angular Velocity (r/s)";
                    break;
            }

            _chart.ChartAreas[0].AxisY.Title = title;
        }

        #endregion
    }
}
