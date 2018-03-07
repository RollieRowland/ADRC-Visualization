namespace ADRCVisualization
{
    partial class Visualizer
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series25 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series26 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series27 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series28 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series29 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series30 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series31 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series32 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series33 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series34 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series35 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series36 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.xPositionTB = new System.Windows.Forms.TextBox();
            this.yPositionTB = new System.Windows.Forms.TextBox();
            this.zPositionTB = new System.Windows.Forms.TextBox();
            this.xRotationTB = new System.Windows.Forms.TextBox();
            this.yRotationTB = new System.Windows.Forms.TextBox();
            this.zRotationTB = new System.Windows.Forms.TextBox();
            this.sendXYZ = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.quadViewer2 = new ADRCVisualization.QuadViewer();
            this.rRotationTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea5.Area3DStyle.IsClustered = true;
            chartArea5.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea5.AxisX.LabelStyle.Enabled = false;
            chartArea5.AxisY.LabelStyle.Enabled = false;
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            this.chart1.Location = new System.Drawing.Point(13, 13);
            this.chart1.Name = "chart1";
            series25.ChartArea = "ChartArea1";
            series25.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series25.IsVisibleInLegend = false;
            series25.MarkerSize = 15;
            series25.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series25.Name = "Series1";
            series26.ChartArea = "ChartArea1";
            series26.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series26.MarkerSize = 10;
            series26.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series26.Name = "Series2";
            series27.ChartArea = "ChartArea1";
            series27.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series27.MarkerSize = 10;
            series27.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series27.Name = "Series3";
            series28.ChartArea = "ChartArea1";
            series28.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series28.MarkerSize = 10;
            series28.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series28.Name = "Series4";
            series29.ChartArea = "ChartArea1";
            series29.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series29.MarkerSize = 10;
            series29.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series29.Name = "Series5";
            series30.ChartArea = "ChartArea1";
            series30.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series30.MarkerSize = 10;
            series30.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series30.Name = "Series6";
            series31.ChartArea = "ChartArea1";
            series31.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series31.MarkerSize = 10;
            series31.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series31.Name = "Series7";
            series32.ChartArea = "ChartArea1";
            series32.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series32.MarkerSize = 10;
            series32.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series32.Name = "Series8";
            series33.ChartArea = "ChartArea1";
            series33.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series33.MarkerSize = 10;
            series33.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series33.Name = "Series9";
            series34.ChartArea = "ChartArea1";
            series34.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series34.MarkerSize = 2;
            series34.Name = "Series10";
            series35.ChartArea = "ChartArea1";
            series35.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series35.MarkerSize = 2;
            series35.Name = "Series11";
            this.chart1.Series.Add(series25);
            this.chart1.Series.Add(series26);
            this.chart1.Series.Add(series27);
            this.chart1.Series.Add(series28);
            this.chart1.Series.Add(series29);
            this.chart1.Series.Add(series30);
            this.chart1.Series.Add(series31);
            this.chart1.Series.Add(series32);
            this.chart1.Series.Add(series33);
            this.chart1.Series.Add(series34);
            this.chart1.Series.Add(series35);
            this.chart1.Size = new System.Drawing.Size(500, 500);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            chartArea6.AxisX.LabelStyle.Enabled = false;
            chartArea6.AxisY.LabelStyle.Enabled = false;
            chartArea6.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea6);
            this.chart2.Location = new System.Drawing.Point(520, 13);
            this.chart2.Name = "chart2";
            series36.ChartArea = "ChartArea1";
            series36.Name = "Series1";
            this.chart2.Series.Add(series36);
            this.chart2.Size = new System.Drawing.Size(190, 486);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "X/Z Coordinate Positions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(530, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Y Coordinate Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(723, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "3D View";
            // 
            // xPositionTB
            // 
            this.xPositionTB.Location = new System.Drawing.Point(12, 519);
            this.xPositionTB.Name = "xPositionTB";
            this.xPositionTB.Size = new System.Drawing.Size(100, 20);
            this.xPositionTB.TabIndex = 6;
            // 
            // yPositionTB
            // 
            this.yPositionTB.Location = new System.Drawing.Point(118, 519);
            this.yPositionTB.Name = "yPositionTB";
            this.yPositionTB.Size = new System.Drawing.Size(100, 20);
            this.yPositionTB.TabIndex = 7;
            // 
            // zPositionTB
            // 
            this.zPositionTB.Location = new System.Drawing.Point(224, 519);
            this.zPositionTB.Name = "zPositionTB";
            this.zPositionTB.Size = new System.Drawing.Size(100, 20);
            this.zPositionTB.TabIndex = 8;
            // 
            // xRotationTB
            // 
            this.xRotationTB.Location = new System.Drawing.Point(520, 519);
            this.xRotationTB.Name = "xRotationTB";
            this.xRotationTB.Size = new System.Drawing.Size(100, 20);
            this.xRotationTB.TabIndex = 9;
            // 
            // yRotationTB
            // 
            this.yRotationTB.Location = new System.Drawing.Point(626, 519);
            this.yRotationTB.Name = "yRotationTB";
            this.yRotationTB.Size = new System.Drawing.Size(100, 20);
            this.yRotationTB.TabIndex = 10;
            // 
            // zRotationTB
            // 
            this.zRotationTB.Location = new System.Drawing.Point(732, 519);
            this.zRotationTB.Name = "zRotationTB";
            this.zRotationTB.Size = new System.Drawing.Size(100, 20);
            this.zRotationTB.TabIndex = 11;
            // 
            // sendXYZ
            // 
            this.sendXYZ.Location = new System.Drawing.Point(330, 519);
            this.sendXYZ.Name = "sendXYZ";
            this.sendXYZ.Size = new System.Drawing.Size(183, 20);
            this.sendXYZ.TabIndex = 12;
            this.sendXYZ.Text = "Send XYZ Coordinates";
            this.sendXYZ.UseVisualStyleBackColor = true;
            this.sendXYZ.Click += new System.EventHandler(this.SendXYZ_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(726, 26);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(502, 473);
            this.elementHost1.TabIndex = 14;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.quadViewer2;
            // 
            // rRotationTB
            // 
            this.rRotationTB.Location = new System.Drawing.Point(838, 519);
            this.rRotationTB.Name = "rRotationTB";
            this.rRotationTB.Size = new System.Drawing.Size(100, 20);
            this.rRotationTB.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(944, 519);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 20);
            this.button1.TabIndex = 13;
            this.button1.Text = "Send XYZ-R Rotation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SendHPB_Click);
            // 
            // Visualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1240, 549);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sendXYZ);
            this.Controls.Add(this.rRotationTB);
            this.Controls.Add(this.zRotationTB);
            this.Controls.Add(this.yRotationTB);
            this.Controls.Add(this.xRotationTB);
            this.Controls.Add(this.zPositionTB);
            this.Controls.Add(this.yPositionTB);
            this.Controls.Add(this.xPositionTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Visualizer";
            this.ShowIcon = false;
            this.Text = "Quadcopter Analyser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Visualizer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox xPositionTB;
        private System.Windows.Forms.TextBox yPositionTB;
        private System.Windows.Forms.TextBox zPositionTB;
        private System.Windows.Forms.TextBox xRotationTB;
        private System.Windows.Forms.TextBox yRotationTB;
        private System.Windows.Forms.TextBox zRotationTB;
        private System.Windows.Forms.Button sendXYZ;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private QuadViewer quadViewer2;
        private System.Windows.Forms.TextBox rRotationTB;
        private System.Windows.Forms.Button button1;
    }
}

