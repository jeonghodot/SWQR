using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fault_Localization_SE_Lab
{
    public partial class Form1 : Form
    {
        public Form1(string cmbAlgorithm , DataTable result)
        {
            InitializeComponent();
            Chart(cmbAlgorithm, result);


        }

        public void Chart(string cmbAlgorithm, DataTable result)
        {

            int algorithmCounter = 0;
            string algorithm = cmbAlgorithm;
            string[] split_al = algorithm.Split(',');
            double values = double.Parse(result.Rows[1][3].ToString());

            foreach (string al in split_al)
            {
                algorithmCounter = algorithmCounter + 1;
             
            }

          //  int[algorithmCounter] algorithms;

            Form1 chart = new Form1(cmbAlgorithm, result);
            chart.Show();




            // chart 를 리셋 합니다.
            chart1.Series.Clear();
            // 값 배열을 생성 합니다.



            int[] intValue = { 1, 2, 3, 4, 5 };
            // 타이틀 배열을 생성 합니다.
            string[] stringValue = { "a", "b", "c", "d", "e" };

            for (int i = 0; i < 5; i++)
            {
                // Series 객체 를 생성 합니다.
                System.Windows.Forms.DataVisualization.Charting.Series se = new
                System.Windows.Forms.DataVisualization.Charting.Series();
                // chart type 을 설정 합니다.
                se.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                // title 값을 입력 합니다.
                se.Name = stringValue[i];
                // 항목 데이터 값을 입력 합니다.
                se.Points.Add(intValue[i]);
                // chart 에 series 객체를 추가 합니다.
                chart1.Series.Add(se);
            }

        }







        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
