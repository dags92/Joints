namespace Experior.Plugin.Joints.GUI.Chart
{
    partial class ChartWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.JointChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.SamplingTimeBox = new System.Windows.Forms.TextBox();
            this.DataType = new System.Windows.Forms.ComboBox();
            this.DataTypeLabel = new System.Windows.Forms.Label();
            this.SamplingTimeLabel = new System.Windows.Forms.Label();
            this.AxisType = new System.Windows.Forms.ComboBox();
            this.AxisTypeLabel = new System.Windows.Forms.Label();
            this.JointsLabel = new System.Windows.Forms.Label();
            this.JointName = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.JointChart)).BeginInit();
            this.SuspendLayout();
            // 
            // JointChart
            // 
            chartArea1.Name = "ChartArea1";
            this.JointChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.JointChart.Legends.Add(legend1);
            this.JointChart.Location = new System.Drawing.Point(12, 12);
            this.JointChart.Name = "JointChart";
            this.JointChart.Size = new System.Drawing.Size(611, 470);
            this.JointChart.TabIndex = 0;
            this.JointChart.Text = "chart1";
            // 
            // SamplingTimeBox
            // 
            this.SamplingTimeBox.Location = new System.Drawing.Point(660, 293);
            this.SamplingTimeBox.Name = "SamplingTimeBox";
            this.SamplingTimeBox.Size = new System.Drawing.Size(100, 20);
            this.SamplingTimeBox.TabIndex = 1;
            this.SamplingTimeBox.TextChanged += new System.EventHandler(this.SamplingTime_TextChanged);
            // 
            // DataType
            // 
            this.DataType.FormattingEnabled = true;
            this.DataType.Location = new System.Drawing.Point(641, 159);
            this.DataType.Name = "DataType";
            this.DataType.Size = new System.Drawing.Size(141, 21);
            this.DataType.TabIndex = 2;
            this.DataType.TextChanged += new System.EventHandler(this.DataType_TextChanged);
            // 
            // DataTypeLabel
            // 
            this.DataTypeLabel.AutoSize = true;
            this.DataTypeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DataTypeLabel.Location = new System.Drawing.Point(669, 132);
            this.DataTypeLabel.Name = "DataTypeLabel";
            this.DataTypeLabel.Size = new System.Drawing.Size(85, 19);
            this.DataTypeLabel.TabIndex = 3;
            this.DataTypeLabel.Text = "Data Type";
            // 
            // SamplingTimeLabel
            // 
            this.SamplingTimeLabel.AutoSize = true;
            this.SamplingTimeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SamplingTimeLabel.Location = new System.Drawing.Point(637, 261);
            this.SamplingTimeLabel.Name = "SamplingTimeLabel";
            this.SamplingTimeLabel.Size = new System.Drawing.Size(145, 19);
            this.SamplingTimeLabel.TabIndex = 4;
            this.SamplingTimeLabel.Text = "Sampling Time (s)";
            // 
            // AxisType
            // 
            this.AxisType.FormattingEnabled = true;
            this.AxisType.Location = new System.Drawing.Point(641, 219);
            this.AxisType.Name = "AxisType";
            this.AxisType.Size = new System.Drawing.Size(141, 21);
            this.AxisType.TabIndex = 5;
            this.AxisType.TextChanged += new System.EventHandler(this.AxisType_TextChanged);
            // 
            // AxisTypeLabel
            // 
            this.AxisTypeLabel.AutoSize = true;
            this.AxisTypeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AxisTypeLabel.Location = new System.Drawing.Point(690, 190);
            this.AxisTypeLabel.Name = "AxisTypeLabel";
            this.AxisTypeLabel.Size = new System.Drawing.Size(42, 19);
            this.AxisTypeLabel.TabIndex = 6;
            this.AxisTypeLabel.Text = "Axis";
            // 
            // JointsLabel
            // 
            this.JointsLabel.AutoSize = true;
            this.JointsLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JointsLabel.Location = new System.Drawing.Point(684, 75);
            this.JointsLabel.Name = "JointsLabel";
            this.JointsLabel.Size = new System.Drawing.Size(56, 19);
            this.JointsLabel.TabIndex = 7;
            this.JointsLabel.Text = "Joints";
            // 
            // JointName
            // 
            this.JointName.FormattingEnabled = true;
            this.JointName.Location = new System.Drawing.Point(642, 102);
            this.JointName.Name = "JointName";
            this.JointName.Size = new System.Drawing.Size(140, 21);
            this.JointName.TabIndex = 8;
            this.JointName.TextChanged += new System.EventHandler(this.Joint_TextChanged);
            // 
            // ChartWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 494);
            this.Controls.Add(this.JointName);
            this.Controls.Add(this.JointsLabel);
            this.Controls.Add(this.AxisTypeLabel);
            this.Controls.Add(this.AxisType);
            this.Controls.Add(this.SamplingTimeLabel);
            this.Controls.Add(this.DataTypeLabel);
            this.Controls.Add(this.DataType);
            this.Controls.Add(this.SamplingTimeBox);
            this.Controls.Add(this.JointChart);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(2000, 1222);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 533);
            this.Name = "ChartWindow";
            this.Text = "JointChart";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.JointChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart JointChart;
        private System.Windows.Forms.TextBox SamplingTimeBox;
        private System.Windows.Forms.ComboBox DataType;
        private System.Windows.Forms.Label DataTypeLabel;
        private System.Windows.Forms.Label SamplingTimeLabel;
        private System.Windows.Forms.ComboBox AxisType;
        private System.Windows.Forms.Label AxisTypeLabel;
        private System.Windows.Forms.Label JointsLabel;
        private System.Windows.Forms.ComboBox JointName;
    }
}