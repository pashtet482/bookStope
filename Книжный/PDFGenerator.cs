using MetroFramework;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Книжный
{
    class PDFGenerator
    {
        public static void GenerateBookReport(DataTable books)
        {
            string filePath = "Отчет_по_наличию_книг.pdf";

        PdfDocument document = new PdfDocument();
        document.Info.Title = "Отчет по наличию книг";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
        XFont headerFont = new XFont("Arial", 12, XFontStyleEx.Bold);
        XFont textFont = new XFont("Arial", 10, XFontStyleEx.Regular);

        XUnit y = XUnit.FromPoint(40);
        gfx.DrawString("Отчет по наличию книг", titleFont, XBrushes.Black, new XRect(XUnit.FromPoint(200).Point, y.Point, 
                       XUnit.FromPoint(400).Point, XUnit.FromPoint(20).Point), XStringFormats.TopLeft);

        y += XUnit.FromPoint(50);

        gfx.DrawString("Название", headerFont, XBrushes.Black, new XPoint(XUnit.FromPoint(50).Point, y.Point));
        gfx.DrawString("Автор", headerFont, XBrushes.Black, new XPoint(XUnit.FromPoint(250).Point, y.Point));
        gfx.DrawString("Количество", headerFont, XBrushes.Black, new XPoint(XUnit.FromPoint(450).Point, y.Point));
        y += XUnit.FromPoint(20);

        foreach (DataRow row in books.Rows)
        {
            gfx.DrawString(row["Название"].ToString(), textFont, XBrushes.Black, new XPoint(XUnit.FromPoint(50).Point, y.Point));
            gfx.DrawString(row["Автор"].ToString(), textFont, XBrushes.Black, new XPoint(XUnit.FromPoint(250).Point, y.Point));
            gfx.DrawString(row["Количество"].ToString(), textFont, XBrushes.Black, new XPoint(XUnit.FromPoint(450).Point, y.Point));
            y += XUnit.FromPoint(20);

            if (y.Point > page.Height.Point - 50)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = XUnit.FromPoint(40);
            }
        }

        document.Save(filePath);
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static void GenerateCheck(DataTable checkData)
        {
            if (checkData.Rows.Count == 0) return;

            string filePath = $"Чек_продажа_{checkData.Rows[0]["Номер продажи"]}.pdf";

            PdfDocument doc = new PdfDocument();
            doc.Info.Title = "Чек о покупке";
            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont headerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            XFont textFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            double y = 40;

            gfx.DrawString("Чек о покупке", headerFont, XBrushes.Black, new XPoint(200, y));
            y += 30;

            var first = checkData.Rows[0];
            gfx.DrawString($"Номер продажи: {first["Номер продажи"]}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Дата: {Convert.ToDateTime(first["Дата"]).ToShortDateString()}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Покупатель: {first["Покупатель"]}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Продавец: {first["Продавец"]}", textFont, XBrushes.Black, new XPoint(40, y)); y += 30;

            gfx.DrawString("Книги:", headerFont, XBrushes.Black, new XPoint(40, y)); y += 20;

            decimal total = 0;
            foreach (DataRow row in checkData.Rows)
            {
                gfx.DrawString($"{row["Книга"]} — {row["Кол-во"]} шт — {row["Стоимость"]} руб.", textFont, XBrushes.Black, new XPoint(50, y));
                total += Convert.ToDecimal(row["Стоимость"]);
                y += 20;

                if (y > page.Height.Point - 50)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            y += 10;
            gfx.DrawString($"Итого: {total} руб. включая НДС 20%", headerFont, XBrushes.Black, new XPoint(50, y));

            doc.Save(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static void GenerateSupplyReport(DataTable supplies)
        {
            string filePath = "Отчет_по_поставкам.pdf";

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Отчет по поставкам";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
            XFont headerFont = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont textFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            XUnit y = XUnit.FromPoint(40);
            gfx.DrawString("Отчет по поставкам", titleFont, XBrushes.Black,
                new XRect(XUnit.FromPoint(200).Point, y.Point, XUnit.FromPoint(400).Point, XUnit.FromPoint(20).Point),
                XStringFormats.TopLeft);

            y += XUnit.FromPoint(50);

            gfx.DrawString("Дата", headerFont, XBrushes.Black, new XPoint(40, y.Point));
            gfx.DrawString("Поставщик", headerFont, XBrushes.Black, new XPoint(120, y.Point));
            gfx.DrawString("Книга", headerFont, XBrushes.Black, new XPoint(280, y.Point));
            gfx.DrawString("Количество", headerFont, XBrushes.Black, new XPoint(450, y.Point));
            y += XUnit.FromPoint(20);

            foreach (DataRow row in supplies.Rows)
            {
                DateTime date = Convert.ToDateTime(row["Дата Поставки"]);
                gfx.DrawString(date.ToString("dd-MM-yyyy"), textFont, XBrushes.Black, new XPoint(40, y.Point));
                gfx.DrawString(row["Поставщик"].ToString(), textFont, XBrushes.Black, new XPoint(120, y.Point));
                gfx.DrawString(row["Книга"].ToString(), textFont, XBrushes.Black, new XPoint(280, y.Point));
                gfx.DrawString(row["Количество"].ToString(), textFont, XBrushes.Black, new XPoint(450, y.Point));
                y += XUnit.FromPoint(20);

                if (y.Point > page.Height.Point - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = XUnit.FromPoint(40);
                }
            }

            document.Save(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static void СформироватьОтчетПоСотрудникам()
        {
            string filePath = "Отчет_по_работе.pdf";
            try
            {
                var таблица = ConnectDB.select(@"
                SELECT 
                    CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS Сотрудник,
                    COUNT(Продажи.ID_Продажи) AS Количество_покупок
                FROM 
                    Продажи
                JOIN 
                    Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника
                GROUP BY 
                    Сотрудник
                ORDER BY 
                    Количество_покупок DESC;
                ");

                PdfDocument document = new PdfDocument();
                document.Info.Title = "Отчет по работе сотрудников";

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitle = new XFont("Arial", 16, XFontStyleEx.Bold);
                XFont font = new XFont("Arial", 12, XFontStyleEx.Regular);

                int startY = 40;
                gfx.DrawString("Отчет по работе сотрудников", fontTitle, XBrushes.Black, new XRect(0, startY, page.Width.Point, 30), XStringFormats.TopCenter);
                startY += 40;

                gfx.DrawString("Сотрудник", font, XBrushes.Black, 40, startY);
                gfx.DrawString("Оформлено покупок", font, XBrushes.Black, 300, startY);
                startY += 20;

                foreach (DataRow row in таблица.Rows)
                {
                    gfx.DrawString(row["Сотрудник"].ToString(), font, XBrushes.Black, 40, startY);
                    gfx.DrawString(row["Количество_покупок"].ToString(), font, XBrushes.Black, 300, startY);
                    startY += 20;

                    if (startY > page.Height.Point - 40)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        startY = 40;
                    }
                }

                document.Save(filePath);
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
