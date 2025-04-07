using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using LiveCharts.Wpf;
using LiveCharts;
using MetroFramework.Forms;
using System.Windows.Media;

namespace Книжный
{
    public partial class ГлавнаяНЕадмин: MetroForm
    {
        private ПродажаСоздание создание;
        private Поставка_создание создание1;
        private Продажа Продажа;
        private Поставки поставки;
        private bool isAdmin;

        public ГлавнаяНЕадмин(bool isAdmin)
        {
            InitializeComponent();
            Resizable = false;
            MaximizeBox = false;
            this.isAdmin = isAdmin;
            

            cartesianChart1.ForeColor = System.Drawing.Color.FromArgb(255, 0, 174, 219);
            cartesianChart1.Background = new SolidColorBrush(Colors.White);
            ПостроитьГрафики();
        }

        private void metroTile5_Click(object sender, EventArgs e)
        {
            Продажа = new Продажа(isAdmin);
            Продажа.ShowDialog();
        }

        private void metroTile6_Click(object sender, EventArgs e)
        {
            поставки = new Поставки(isAdmin);
            поставки.ShowDialog();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            создание = new ПродажаСоздание();
            создание.ShowDialog();
        }

        private void metroTile2_Click(object sender, EventArgs e)
        {
            создание1 = new Поставка_создание();
            создание1.ShowDialog();
        }

        private void ГлавнаяНЕадмин_Activated(object sender, EventArgs e)
        {
            ПостроитьГрафики();
        }

        private void ПостроитьГрафики()
        {
            DataTable продажиПоДням = ConnectDB.select(
                "SELECT DATE(Дата_продажи) AS Дата, COUNT(*) AS Количество " +
                "FROM Продажи " +
                "WHERE DATE(Дата_продажи) BETWEEN CURDATE() - INTERVAL 6 DAY AND CURDATE() " +
                "GROUP BY DATE(Дата_продажи) " +
                "ORDER BY Дата;"

            );

            var значенияГистограммы = new ChartValues<int>();
            var меткиОсиX = new List<string>();

            foreach (DataRow row in продажиПоДням.Rows)
            {
                значенияГистограммы.Add(Convert.ToInt32(row["Количество"]));
                меткиОсиX.Add(Convert.ToDateTime(row["Дата"]).ToString("dd.MM.yyyy"));
            }

            cartesianChart1.Series = new SeriesCollection
            {

                new ColumnSeries
                {
                    Title = "Продажи",
                    Values = значенияГистограммы
                }
            };

            cartesianChart1.AxisX.Clear();
            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Дата",
                Labels = меткиОсиX
            });

            cartesianChart1.AxisY.Clear();
            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Количество продаж"
            });

            DataTable книги = ConnectDB.select(
                "SELECT Книги.Жанр AS Книги, COUNT(*) AS Количество " +
                "FROM Продажи " +
                "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                "WHERE Дата_продажи >= CURDATE() - INTERVAL 1 MONTH " +
                "GROUP BY Книги.Жанр " +
                "ORDER BY Количество DESC;"
            );

            pieChart1.Series.Clear();
            int общееКоличество = книги.AsEnumerable().Sum(r => Convert.ToInt32(r["Количество"]));

            foreach (DataRow row in книги.Rows)
            {
                string книга = row["Книги"].ToString();
                int количество = Convert.ToInt32(row["Количество"]);

                pieChart1.Series.Add(new PieSeries
                {
                    Title = книга,
                    Values = new ChartValues<int> { количество },
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString()
                });
            }
        }

        private void metroTile8_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void metroTile7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
