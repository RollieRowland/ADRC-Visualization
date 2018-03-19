namespace ADRCVisualization
{
    partial class UserInterface
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
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.XZCoordinates = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.YCoordinate = new System.Windows.Forms.DataVisualization.Charting.Chart();
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
            this.rRotationTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.quadViewer1 = new ADRCVisualization.QuadViewer();
            ((System.ComponentModel.ISupportInitialize)(this.XZCoordinates)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YCoordinate)).BeginInit();
            this.SuspendLayout();
            // 
            // XZCoordinates
            // 
            chartArea1.Area3DStyle.IsClustered = true;
            chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.XZCoordinates.ChartAreas.Add(chartArea1);
            this.XZCoordinates.Location = new System.Drawing.Point(13, 13);
            this.XZCoordinates.Name = "XZCoordinates";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series1.IsVisibleInLegend = false;
            series1.MarkerSize = 15;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series2.MarkerSize = 10;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "Series2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series3.MarkerSize = 10;
            series3.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series3.Name = "Series3";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series4.MarkerSize = 10;
            series4.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series4.Name = "Series4";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series5.MarkerSize = 10;
            series5.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series5.Name = "Series5";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series6.MarkerSize = 10;
            series6.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series6.Name = "Series6";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series7.MarkerSize = 10;
            series7.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series7.Name = "Series7";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series8.MarkerSize = 10;
            series8.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series8.Name = "Series8";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series9.MarkerSize = 10;
            series9.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star4;
            series9.Name = "Series9";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series10.MarkerSize = 2;
            series10.Name = "Series10";
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series11.MarkerSize = 2;
            series11.Name = "Series11";
            this.XZCoordinates.Series.Add(series1);
            this.XZCoordinates.Series.Add(series2);
            this.XZCoordinates.Series.Add(series3);
            this.XZCoordinates.Series.Add(series4);
            this.XZCoordinates.Series.Add(series5);
            this.XZCoordinates.Series.Add(series6);
            this.XZCoordinates.Series.Add(series7);
            this.XZCoordinates.Series.Add(series8);
            this.XZCoordinates.Series.Add(series9);
            this.XZCoordinates.Series.Add(series10);
            this.XZCoordinates.Series.Add(series11);
            this.XZCoordinates.Size = new System.Drawing.Size(500, 500);
            this.XZCoordinates.TabIndex = 0;
            this.XZCoordinates.Text = "XZCoordinates";
            // 
            // YCoordinate
            // 
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.Name = "ChartArea1";
            this.YCoordinate.ChartAreas.Add(chartArea2);
            this.YCoordinate.Location = new System.Drawing.Point(520, 13);
            this.YCoordinate.Name = "YCoordinate";
            series12.ChartArea = "ChartArea1";
            series12.Name = "Series1";
            this.YCoordinate.Series.Add(series12);
            this.YCoordinate.Size = new System.Drawing.Size(190, 486);
            this.YCoordinate.TabIndex = 1;
            this.YCoordinate.Text = "YCoordinate";
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
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(726, 26);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(502, 473);
            this.elementHost1.TabIndex = 14;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.quadViewer1;
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
            this.Controls.Add(this.YCoordinate);
            this.Controls.Add(this.XZCoordinates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Visualizer";
            this.ShowIcon = false;
            this.Text = "Quadcopter Analyser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Visualizer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.XZCoordinates)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YCoordinate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart XZCoordinates;
        private System.Windows.Forms.DataVisualization.Charting.Chart YCoordinate;
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
        private System.Windows.Forms.TextBox rRotationTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private QuadViewer quadViewer1;
    }
}

