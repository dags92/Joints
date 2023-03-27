using System;
using System.Globalization;
using System.Timers;
using System.Windows.Forms;
using Experior.Catalog.Joints.Assemblies.Mechanisms;

namespace Experior.Plugin.Joints.GUI.Chart
{
    public partial class ChartWindow : Form
    {
        #region Fields

        private System.Timers.Timer _inputTimer;

        private ChartDataTypes _chartDataType;
        private Axes _axis;
        private float _samplingTime = 0.016f;

        private readonly IMechanism _mechanism;

        #endregion

        #region Constructor

        public ChartWindow(IMechanism mechanism)
        {
            _mechanism = mechanism;

            InitializeComponent();
            Init();
        }

        #endregion

        #region Events

        public EventHandler<string> JointModified;

        public EventHandler<ChartDataTypes> ChartDataTypeModified;

        public EventHandler<Axes> AxisModified;

        public EventHandler<float> SamplingTimeModified;

        #endregion

        #region Enums

        public enum ChartDataTypes
        {
            Angular_Velocity,
            Linear_Velocity,
            Linear_Force,
            Angular_Force
        }

        public enum Axes
        {
            X,
            Y,
            Z
        }

        #endregion

        #region Public Properties

        public System.Windows.Forms.DataVisualization.Charting.Chart Chart => JointChart;

        public string Joint => JointName.SelectedIndex.ToString();

        public ChartDataTypes ChartDataType
        {
            get => _chartDataType;
            set
            {
                if (value == _chartDataType)
                {
                    return;
                }

                _chartDataType = value;
                ChartDataTypeModified?.Invoke(this, value);
            }
        }

        public Axes Axis
        {
            get => _axis;
            set
            {
                _axis = value;
                AxisModified?.Invoke(this, value);
            }
        }

        public float SamplingTime
        {
            get => _samplingTime;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _samplingTime = value;
                SamplingTimeModified?.Invoke(this, value);
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            _inputTimer = new System.Timers.Timer(500);
            _inputTimer.AutoReset = false;
            _inputTimer.Elapsed += InputTimerOnElapsed;

            // Enhance Visualization of Enum values:

            // Joints:
            _mechanism.Joints.ForEach(a => JointName.Items.Add(a));
            JointName.SelectedIndex = 0;

            // Data Types:
            var dataTypes = Enum.GetNames(typeof(ChartDataTypes));
            foreach (var name in dataTypes)
            {
                DataType.Items.Add(name.Replace('_', ' '));
            }

            DataType.SelectedIndex = (int)ChartDataTypes.Angular_Velocity;

            // Axis:
            var temp = Enum.GetNames(typeof(Axes));
            foreach (var axisName in temp)
            {
                AxisType.Items.Add(axisName);
            }
            AxisType.SelectedIndex = (int)Axes.X;

            // Sampling Time:
            SamplingTimeBox.Text = _samplingTime.ToString(CultureInfo.InvariantCulture);
        }

        private void Joint_TextChanged(object sender, EventArgs e)
        {
            JointModified?.Invoke(this, Joint);
        }

        private void DataType_TextChanged(object sender, EventArgs e)
        {
            ChartDataType = (ChartDataTypes)DataType.SelectedIndex;
        }

        private void AxisType_TextChanged(object sender, EventArgs e)
        {
            Axis = (Axes)DataType.SelectedIndex;
        }

        private void SamplingTime_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(SamplingTimeBox.Text, out var newValue))
            {
                if (newValue <= 0)
                {
                    return;
                }

                SamplingTime = newValue;
            }
            else
            {
                _inputTimer.Start();
            }
        }

        private void InputTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            SamplingTimeBox.Text = SamplingTime.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
