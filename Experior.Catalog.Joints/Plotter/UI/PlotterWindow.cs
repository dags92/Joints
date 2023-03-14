using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Experior.Catalog.Joints.Plotter.UI
{
    public partial class PlotterWindow : Form, IChart
    {
        #region Fields


        #endregion

        #region Constructor

        public PlotterWindow()
        {
            InitializeComponent();
            Chart = DataChart;
        }

        #endregion

        #region Public Properties

        public Chart Chart { get; }

        #endregion

        #region Private Methods



        #endregion
    }
}
