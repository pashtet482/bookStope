using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace Книжный
{
    public partial class Продажа: MetroForm
    {
        private Поставки formПоставки;
        private Продажа продажа;
        private DimForm dim;
        private bool open = false;
        private DateTime? датаОт;
        private DateTime? датаДо;
        private ПродажаСоздание создание;
        private bool isAdmin1;

        private static Продажа instance;
        public static Продажа Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Продажа();
                }
                return instance;
            }
        }
        public Продажа(bool isAdmin = true)
        {
            InitializeComponent();

            SetButtonColors(Color.FromArgb(255, 255, 255, 255));
            SetPanelColors(Color.FromArgb(255, 0, 174, 219));

            metroPanel4.Width = 0;
            metroGrid1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGrid1.ReadOnly = true;
            metroGrid1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            metroGrid1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            metroGrid1.AutoResizeRows();

            Resizable = false;
            MaximizeBox = false;

            instance = this;
            isAdmin1 = isAdmin;

            if (!isAdmin1)
            {
                metroTile1.Hide();
            }
        }

        private void SetButtonColors(Color color)
        {
            metroButton8.BackColor = color;
            metroButton10.BackColor = color;
            metroButton1.BackColor = color;
            metroButton2.BackColor = color;
            metroButton3.BackColor = color;
            metroPanel4.BackColor = color;
            metroButton14.BackColor = color;
            metroButton15.BackColor = color;
            metroButton16.BackColor = color;
        }

        private void SetPanelColors(Color color)
        {
            metroPanel1.BackColor = color;
            metroLabel1.BackColor = color;
            metroPanel2.BackColor = color;
            metroPanel3.BackColor = color;
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            SideBar();
        }

        public void SideBar()
        {
            if (!open)
            {
                ShowSidebar();
                dim = new DimForm(this, SideBar);
                dim.Show();
            }
            else
            {
                HideSidebar();
                dim?.Hide();
                dim = null;
            }

            open = !open;
        }

        private async void HideSidebar()
        {
            int targetWidth = 0;

            while (metroPanel4.Width > targetWidth)
            {
                metroPanel4.Width -= 10;
                await Task.Delay(10);
            }
        }

        private async void ShowSidebar()
        {
            int targetWidth = 190;

            while (metroPanel4.Width < targetWidth)
            {
                metroPanel4.Width += 10;
                await Task.Delay(10);
            }

            metroTile1.Focus();
        }

        private void Продажа_Move(object sender, EventArgs e)
        {
            if (dim != null && !dim.IsDisposed)
            {
                dim.Location = new Point(Location.X + (open ? 200 : 0), Location.Y);
            }
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex == 2)
            {
                metroComboBox1.SelectedIndex = -1;
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            MonthCalendar calendar = new MonthCalendar
            {
                MaxSelectionCount = 1
            };

            Form popup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = metroButton3.PointToScreen(new Point(0, metroButton3.Height)),
                Size = calendar.Size,
                ShowInTaskbar = false
            };

            popup.Controls.Add(calendar);

            calendar.DateSelected += (s, ev) =>
            {
                if (датаДо.HasValue && ev.Start > датаДо.Value)
                {
                    MessageBox.Show("Дата 'От' не может быть позже даты 'До'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    датаОт = ev.Start;
                    metroButton3.Text = датаОт.Value.ToShortDateString();
                    popup.Close();
                    ОбновитьПродажиПоДате();
                }
            };

            popup.Deactivate += (s, ev) => popup.Close();

            popup.ShowDialog();
        }

        private void metroButton14_Click(object sender, EventArgs e)
        {
            MonthCalendar calendar = new MonthCalendar
            {
                MaxSelectionCount = 1
            };

            Form popup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = metroButton14.PointToScreen(new Point(0, metroButton14.Height)),
                Size = calendar.Size,
                ShowInTaskbar = false
            };

            popup.Controls.Add(calendar);

            calendar.DateSelected += (s, ev) =>
            {
                if (датаОт.HasValue && ev.Start < датаОт.Value)
                {
                    MessageBox.Show("Дата 'До' не может быть раньше даты 'От'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    датаДо = ev.Start;
                    metroButton14.Text = датаДо.Value.ToShortDateString();
                    popup.Close();
                    ОбновитьПродажиПоДате();
                }
            };

            popup.Deactivate += (s, ev) => popup.Close();

            popup.ShowDialog();
        }

        private void ОбновитьПродажиПоДате()
        {
            if (датаОт.HasValue && !датаДо.HasValue)
            {
                string датаНачало = датаОт.Value.ToString("yyyy-MM-dd");
                string query = "SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                           "Продажи.Дата_продажи AS 'Дата', " +
                           "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                           "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                           "Книги.Название AS 'Книга', " +
                           "Состав_продажи.Количество AS 'Кол-во', " +
                           "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                           "FROM Продажи " +
                           "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                           "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                           "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                           "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                           $"WHERE Продажи.Дата_продажи >= '{датаНачало}' " +
                           "ORDER BY Продажи.Дата_продажи DESC";

                DataTable dt = ConnectDB.select(query);
                metroGrid1.DataSource = dt;
            }
            if (датаДо.HasValue && !датаОт.HasValue)
            {
                string датаКонец = датаДо.Value.ToString("yyyy-MM-dd");

                string query = "SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                          "Продажи.Дата_продажи AS 'Дата', " +
                          "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                          "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                          "Книги.Название AS 'Книга', " +
                          "Состав_продажи.Количество AS 'Кол-во', " +
                          "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                          "FROM Продажи " +
                          "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                          "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                          "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                          "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                          $"WHERE Продажи.Дата_продажи <= '{датаКонец}' " +
                          "ORDER BY Продажи.Дата_продажи DESC";

                DataTable dt = ConnectDB.select(query);
                metroGrid1.DataSource = dt;
            }

            if (датаОт.HasValue && датаДо.HasValue)
            {
                string датаНачало = датаОт.Value.ToString("yyyy-MM-dd");
                string датаКонец = датаДо.Value.ToString("yyyy-MM-dd");

                string query = "SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                           "Продажи.Дата_продажи AS 'Дата', " +
                           "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                           "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                           "Книги.Название AS 'Книга', " +
                           "Состав_продажи.Количество AS 'Кол-во', " +
                           "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                           "FROM Продажи " +
                           "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                           "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                           "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                           "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                           $"WHERE Продажи.Дата_продажи BETWEEN '{датаНачало}' AND '{датаКонец}' " +
                           "ORDER BY Продажи.Дата_продажи DESC";

                DataTable dt = ConnectDB.select(query);
                metroGrid1.DataSource = dt;
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Главная.Instance);
            SideBar();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Книги.Instance);
            SideBar();
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Покупатели.Instance);
            SideBar();
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Поставщики.Instance);
            SideBar();
        }

        private void metroButton11_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Сотрудники.Instance);
            SideBar();
        }

        private void metroButton12_Click(object sender, EventArgs e)
        {
            if (продажа == null || продажа.IsDisposed)
            {
                продажа = new Продажа();
            }
            SwapForms.ChangeForm(this, this);
            SideBar();
        }

        private void metroButton13_Click(object sender, EventArgs e)
        {
            if (formПоставки == null || formПоставки.IsDisposed)
            {
                formПоставки = new Поставки();
            }
            SwapForms.ChangeForm(this, Поставки.Instance);
            SideBar();
        }

        private void Продажа_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void LoadTable()
        {
            DataTable dt = ConnectDB.select("SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                                             "Продажи.Дата_продажи AS 'Дата', " +
                                             "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                                             "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                                             "Книги.Название AS 'Книга', " +
                                             "Состав_продажи.Количество AS 'Кол-во', " +
                                             "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                                             "FROM Продажи " +
                                             "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                                             "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                             "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                                             "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                                             "ORDER BY Продажи.ID_Продажи ASC;");

            if (dt != null)
            {
                metroGrid1.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Пустой ответ");
            }
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            создание = new ПродажаСоздание();
            создание.FormClosing += (s, args) => LoadTable();
            создание.ShowDialog();
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedCells.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedCells[0].RowIndex;

                if (metroGrid1.Rows[selectedIndex].Cells[0].Value is int idToDelete)
                {
                    DialogResult result = MetroMessageBox.Show(
                        Owner, "Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ConnectDB.delete("Продажи", "ID_Продажи = @id", new Dictionary<string, object> { { "@id", idToDelete } });

                        if (success)
                        {
                            MetroMessageBox.Show(Owner, "Строка успешно удалена из базы данных.", "Книжный магазин");
                            LoadTable();
                        }
                        else
                        {
                            MetroMessageBox.Show(Owner, "Ошибка: запись не найдена или не была удалена.", "Книжный магазин");
                        }
                    }
                    else
                    {
                        MetroMessageBox.Show(Owner, "Удаление отменено", "Книжный магазин");
                    }
                }
                else
                {
                    MetroMessageBox.Show(Owner, "Некорректный идентификатор книги.", "Книжный магазин");
                }
            }
            else
            {
                MetroMessageBox.Show(Owner, "Выберите строку для удаления.", "Книжный магазин");
            }
        }

        private void metroButton15_Click(object sender, EventArgs e)
        {
            metroComboBox1.SelectedIndex = -1;
            metroTextBox2.Text = string.Empty;
            metroButton3.Text = "Дата от:";
            metroButton14.Text = "Дата до:";
            датаДо = null;
            датаОт = null;
            LoadTable();
        }

        private bool isHandlingTextChanged = false;

        private void metroTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (isHandlingTextChanged) return;
            isHandlingTextChanged = true;

            if (metroComboBox1.SelectedIndex == -1)
            {
                metroTextBox2.Text = null;
                MetroMessageBox.Show(Owner, "Укажите категорию поиска!", "Книжный магазин");
            }
            else
            {
                string s = metroTextBox2.Text;
                string column;

                switch (metroComboBox1.SelectedIndex)
                {
                    case 0: column = "Книги.Название"; break;
                    case 1: column = "Сотрудники.Фамилия"; break;
                    default: column = "Книги.Название"; break;
                }

                string query = "SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                               "Продажи.Дата_продажи AS 'Дата', " +
                               "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                               "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                               "Книги.Название AS 'Книга', " +
                               "Состав_продажи.Количество AS 'Кол-во', " +
                               "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                               "FROM Продажи " +
                               "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                               "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                               "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                               $"JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги WHERE {column} LIKE @search";

                var parameters = new Dictionary<string, object>
                {
                    { "@search", s + "%" }
                };

                DataTable dt = ConnectDB.select(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    metroGrid1.DataSource = dt;
                }
            }

            isHandlingTextChanged = false;
        }

        private bool asc = true;

        private void metroButton2_Click(object sender, EventArgs e) => SortBooks(7);

        private void SortBooks(int columnIndex)
        {
            string order = asc ? "ASC" : "DESC";
            asc = !asc;

            DataTable dt = ConnectDB.select($"SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                               "Продажи.Дата_продажи AS 'Дата', " +
                               "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                               "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                               "Книги.Название AS 'Книга', " +
                               "Состав_продажи.Количество AS 'Кол-во', " +
                               "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                               "FROM Продажи " +
                               "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                               "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                               "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                               $"JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги ORDER BY {columnIndex} {order}");

            if (dt == null || dt.Rows.Count == 0)
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
            else
            {
                metroGrid1.DataSource = dt;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e) => SortBooks(6);

        private void metroButton16_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку с продажей.");
                return;
            }

            int idПродажи = Convert.ToInt32(metroGrid1.SelectedRows[0].Cells["Номер продажи"].Value);

            string query = "SELECT Продажи.ID_Продажи AS 'Номер продажи', " +
                           "Продажи.Дата_продажи AS 'Дата', " +
                           "CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель', " +
                           "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Продавец', " +
                           "Книги.Название AS 'Книга', " +
                           "Состав_продажи.Количество AS 'Кол-во', " +
                           "ROUND(Книги.Цена * Состав_Продажи.Количество * 1.2, 2) AS 'Стоимость' " +
                           "FROM Продажи " +
                           "JOIN Покупатели ON Продажи.ID_Покупателя = Покупатели.ID_Покупателя " +
                           "JOIN Сотрудники ON Продажи.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                           "JOIN Состав_продажи ON Продажи.ID_Продажи = Состав_продажи.ID_Продажи " +
                           "JOIN Книги ON Состав_продажи.ID_Книги = Книги.ID_Книги " +
                           $"WHERE Продажи.ID_Продажи = {idПродажи}";

            DataTable чек = ConnectDB.select(query);
            PDFGenerator.GenerateCheck(чек);
        }

        private void Продажа_Activated(object sender, EventArgs e)
        {
            LoadTable();
        }
    }
}