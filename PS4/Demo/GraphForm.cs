using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace SS
{
    public partial class GraphForm : Form
    {
        Spreadsheet spreadsheetData;

        public GraphForm(Spreadsheet s)
        {
            spreadsheetData = s;
            InitializeComponent();
        }

        private void buttonGraph_Click(object sender, EventArgs e)
        {
            //Handle error checking and messages to alert users what is wrong
            int startRowRange, endRowRange;
            if (int.TryParse(textBoxRowStart.Text, out startRowRange) &&
                int.TryParse(textBoxRowEnd.Text, out endRowRange))
            {
                if (startRowRange < endRowRange && startRowRange > 0 && endRowRange > 0)
                {
                    if (Regex.IsMatch(textBoxXCol.Text, "^[A-Z]$") &&
                        Regex.IsMatch(textBoxYCol.Text, "^[A-Z]$"))
                    {
                        //creates and intis a new chart
                        Chart c = new Chart();
                        initChart(c, textBoxXLabel.Text, textBoxYLabel.Text);
                        //creates a new series of data points from the spread sheet data
                        Series s = createDataSeries(startRowRange, endRowRange,
                            textBoxXCol.Text[0], textBoxYCol.Text[0]);
                        c.Series.Add(s);

                        //saves the graph to the debug folder
                        c.SaveImage(textBoxFileName.Text + ".png", ChartImageFormat.Png);
                        MessageBox.Show("Your graph was successfully saved to the debug file of this project", "Saved", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                    else { MessageBox.Show("Enter valid columns (A-Z)"); }
                }
                else{   MessageBox.Show("Enter a valid range"); }
            }
            else
            {
                MessageBox.Show("Enter integers only in range text boxes");
            }
        }

        /// <summary>
        /// enters the data points from the spreadsheet into the graph
        /// </summary>
        /// <param name="rangeStart">start of range</param>
        /// <param name="rangeEnd">end of range</param>
        /// <param name="XCol">X column</param>
        /// <param name="YCol">Y column</param>
        /// <returns>A series containing data points</returns>
        private Series createDataSeries(int rangeStart, int rangeEnd, char XCol, char YCol)
        {
            //setting up basic series information
            Series s = new Series();
            s.Font = new Font("Lucida Sans Unicode", 6f);
            s.Color = Color.FromArgb(215, 47, 6);
            s.BorderColor = Color.FromArgb(159, 27, 13);
            s.ChartType = SeriesChartType.Line;
            s.BackSecondaryColor = Color.FromArgb(173, 32, 11);
            s.BackGradientStyle = GradientStyle.LeftRight;

            //iterates through the rows in the range
            //gets the values of each pair of cells and creates a data point
            for (int i = rangeStart; i <= rangeEnd; i++)
            {
                string Xcell = XCol + i.ToString();
                string Ycell = YCol + i.ToString();
                object xCellVal = spreadsheetData.GetCellValue(XCol + i.ToString());
                object yCellVal = spreadsheetData.GetCellValue(YCol + i.ToString());
                if (xCellVal is double && yCellVal is double)
                {
                    DataPoint p = new DataPoint();
                    p.XValue = (double)xCellVal;
                    p.YValues = new Double[] { (double)yCellVal };
                    s.Points.Add(p);
                }
            }
            //the series of data points is returned
            return s;
        }

        /// <summary>
        /// This code was mostly found from examples online and on stack overflow
        /// I don't claim credit for the majority of this, but I have spent a lot of time working
        /// to make sure I understand how it all works before tossing it in my project
        /// I made minor edits to account for allowing users to enter their own axis labels
        /// and to change the chart type that is displayed and the visible information for it
        /// </summary>
        /// <param name="c">Chart to Init</param>
        private void initChart(Chart c, string xlabel, string ylabel)
        {
            c.AntiAliasing = AntiAliasingStyles.All;
            c.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            c.Width = 640; //SET HEIGHT
            c.Height = 480; //SET WIDTH

            ChartArea ca = new ChartArea();
            ca.BackColor = Color.FromArgb(248, 248, 248);
            ca.BackSecondaryColor = Color.FromArgb(255, 255, 255);
            ca.BackGradientStyle = GradientStyle.TopBottom;

            ca.AxisY.IsMarksNextToAxis = true;
            ca.AxisY.Title = ylabel;
            ca.AxisY.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisY.MajorTickMark.Enabled = true;
            ca.AxisY.MinorTickMark.Enabled = true;
            ca.AxisY.MajorTickMark.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisY.MinorTickMark.LineColor = Color.FromArgb(200, 200, 200);
            ca.AxisY.LabelStyle.ForeColor = Color.FromArgb(89, 89, 89);
            ca.AxisY.LabelStyle.Format = "{0:0.0}";
            ca.AxisY.LabelStyle.IsEndLabelVisible = false;
            ca.AxisY.LabelStyle.Font = new Font("Calibri", 4, FontStyle.Regular);
            ca.AxisY.MajorGrid.LineColor = Color.FromArgb(234, 234, 234);

            ca.AxisX.IsMarksNextToAxis = true;
            ca.AxisX.Title = xlabel;
            ca.AxisX.LabelStyle.Enabled = true;
            ca.AxisX.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisX.MajorGrid.LineWidth = 0;
            ca.AxisX.MajorTickMark.Enabled = true;
            ca.AxisX.MinorTickMark.Enabled = true;
            ca.AxisX.MajorTickMark.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisX.MinorTickMark.LineColor = Color.FromArgb(200, 200, 200);

            c.ChartAreas.Add(ca);
        }
    }
}
