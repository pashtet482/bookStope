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
    public partial class Поставки : MetroForm
    {
        private Поставки formПоставки;
        private Продажа продажа;
        private DimForm dim;
        private bool open = false;
        private DateTime? датаОт;
        private DateTime? датаДо;
        private Поставка_создание создание;
        private bool isAdmin1;

        private static Поставки instance;
        public static Поставки Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Поставки();
                }
                return instance;
            }
        }

        public Поставки(bool isAdmin = true)
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

        private void Поставки_Move(object sender, EventArgs e)
        {
            if (dim != null && !dim.IsDisposed)
            {
                dim.Location = new Point(Location.X + (open ? 200 : 0), Location.Y);
            }
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex == 3)
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
                    Activate();
                    ОбновитьПоставкиПоДате();
                }
            };

            popup.Deactivate += (s, ev) => popup.Close();

            popup.Show();
        }

        private void metroButton14_Click_1(object sender, EventArgs e)
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
                    Activate();
                    ОбновитьПоставкиПоДате();
                }
            };

            popup.Deactivate += (s, ev) => popup.Close();

            popup.Show();
        }

        private void ОбновитьПоставкиПоДате()
        {
            string whereClause = "";

            if (датаОт.HasValue && !датаДо.HasValue)
            {
                string от = датаОт.Value.ToString("yyyy-MM-dd");
                whereClause = $"WHERE Поставка.Дата_Поставки >= '{от}'";
            }
            else if (датаДо.HasValue && !датаОт.HasValue)
            {
                string до = датаДо.Value.ToString("yyyy-MM-dd");
                whereClause = $"WHERE Поставка.Дата_Поставки <= '{до}'";
            }
            else if (датаОт.HasValue && датаДо.HasValue)
            {
                string от = датаОт.Value.ToString("yyyy-MM-dd");
                string до = датаДо.Value.ToString("yyyy-MM-dd");
                whereClause = $"WHERE Поставка.Дата_Поставки BETWEEN '{от}' AND '{до}'";
            }

            string query = "SELECT Поставка.ID_Поставки AS 'ID Поставки', Поставка.Дата_Поставки AS 'Дата Поставки', " +
                                            "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник', " +
                                            "Поставщики.Название AS 'Поставщик', Книги.Название AS 'Книга', Состав_поставки.Закупочная_Цена AS 'Закупочная цена', " +
                                            "Состав_поставки.Количество " +
                                            "FROM Поставка JOIN Сотрудники ON Поставка.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                            "JOIN Поставщики ON Поставка.ID_Поставщика = Поставщики.ID_Поставщика " +
                                            "JOIN Состав_поставки ON Поставка.ID_Поставки = Состав_поставки.ID_Поставки " +
                                            "JOIN Книги ON Состав_поставки.ID_Книги = Книги.ID_Книги " +
                                            $"{whereClause};";

            DataTable dt = ConnectDB.select(query);
            metroGrid1.DataSource = dt;
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
            SwapForms.ChangeForm(this, Поставки.Instance);
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
            SwapForms.ChangeForm(this, Продажа.Instance);
            SideBar();
        }

        private void metroButton13_Click(object sender, EventArgs e)
        {
            if (formПоставки == null || formПоставки.IsDisposed)
            {
                formПоставки = new Поставки();
            }
            SwapForms.ChangeForm(this, this);
            SideBar();
        }

        private void Поставки_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void LoadTable()
        {
            DataTable dt = ConnectDB.select("SELECT Поставка.ID_Поставки AS 'ID Поставки', Поставка.Дата_Поставки AS 'Дата Поставки', " +
                                            "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник', " +
                                            "Поставщики.Название AS 'Поставщик', Книги.Название AS 'Книга', Состав_поставки.Закупочная_Цена AS 'Закупочная цена', " +
                                            "Состав_поставки.Количество " +
                                            "FROM Поставка JOIN Сотрудники ON Поставка.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                            "JOIN Поставщики ON Поставка.ID_Поставщика = Поставщики.ID_Поставщика " +
                                            "JOIN Состав_поставки ON Поставка.ID_Поставки = Состав_поставки.ID_Поставки " +
                                            "JOIN Книги ON Состав_поставки.ID_Книги = Книги.ID_Книги;");

            if (dt != null)
            {
                metroGrid1.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Пустой ответ");
            }
        }

        private void Поставки_Activated(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            создание = new Поставка_создание();
            создание.FormClosing += (s, args) => LoadTable();
            создание.ShowDialog();
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedCells.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedCells[0].RowIndex;

                if (metroGrid1.Rows[selectedIndex].Cells["ID Поставки"].Value is int idToDelete)
                {
                    DialogResult result = MetroMessageBox.Show(
                        Owner, "Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ConnectDB.delete("Поставка", "ID_Поставки = @id", new Dictionary<string, object> { { "@id", idToDelete } });

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
                    MetroMessageBox.Show(Owner, "Некорректный идентификатор поставщика.", "Книжный магазин");
                }
            }
            else
            {
                MetroMessageBox.Show(Owner, "Выберите строку для удаления.", "Книжный магазин");
            }
        }

        private void metroButton2_Click(object sender, EventArgs e) => SortBooks(4);

        private bool asc = true;

        private void SortBooks(int columnIndex)
        {
            string order = asc ? "ASC" : "DESC";
            asc = !asc;

            DataTable dt = ConnectDB.select($"SELECT Поставка.ID_Поставки AS 'ID Поставки', Поставка.Дата_Поставки AS 'Дата Поставки', " +
                                            "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник', " +
                                            "Поставщики.Название AS 'Поставщик', Книги.Название AS 'Книга', Состав_поставки.Закупочная_Цена AS 'Закупочная цена', " +
                                            "Состав_поставки.Количество " +
                                            "FROM Поставка JOIN Сотрудники ON Поставка.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                            "JOIN Поставщики ON Поставка.ID_Поставщика = Поставщики.ID_Поставщика " +
                                            "JOIN Состав_поставки ON Поставка.ID_Поставки = Состав_поставки.ID_Поставки " +
                                            $"JOIN Книги ON Состав_поставки.ID_Книги = Книги.ID_Книги ORDER BY {columnIndex} {order}");

            if (dt == null || dt.Rows.Count == 0)
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
            else
            {
                metroGrid1.DataSource = dt;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e) => SortBooks(5);

        private void metroButton15_Click(object sender, EventArgs e)
        {
            metroTextBox2.Text = string.Empty;
            metroComboBox1.SelectedIndex = -1;
            датаДо = null;
            датаОт = null;
            metroButton3.Text = "Дата от";
            metroButton14.Text = "Дата до";
            LoadTable();
        }

        private void metroButton16_Click(object sender, EventArgs e)
        {
            DataTable dt = ConnectDB.select("SELECT Поставка.ID_Поставки AS 'ID Поставки', Поставка.Дата_Поставки AS 'Дата Поставки', " +
                                            "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник', " +
                                            "Поставщики.Название AS 'Поставщик', Книги.Название AS 'Книга', Состав_поставки.Закупочная_Цена AS 'Закупочная цена', " +
                                            "Состав_поставки.Количество " +
                                            "FROM Поставка JOIN Сотрудники ON Поставка.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                            "JOIN Поставщики ON Поставка.ID_Поставщика = Поставщики.ID_Поставщика " +
                                            "JOIN Состав_поставки ON Поставка.ID_Поставки = Состав_поставки.ID_Поставки " +
                                            "JOIN Книги ON Состав_поставки.ID_Книги = Книги.ID_Книги;");
            PDFGenerator.GenerateSupplyReport(dt);
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
                    case 0: column = "Поставщики.Название"; break;
                    case 1: column = "Книги.Название"; break;
                    case 2: column = "Сотрудники.Фамилия"; break;
                    default: column = "ID Поставки"; break;
                }

                string query = "SELECT Поставка.ID_Поставки AS 'ID Поставки', Поставка.Дата_Поставки AS 'Дата Поставки', " +
                                            "CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник', " +
                                            "Поставщики.Название AS 'Поставщик', Книги.Название AS 'Книга', Состав_поставки.Закупочная_Цена AS 'Закупочная цена', " +
                                            "Состав_поставки.Количество " +
                                            "FROM Поставка JOIN Сотрудники ON Поставка.ID_Сотрудника = Сотрудники.ID_Сотрудника " +
                                            "JOIN Поставщики ON Поставка.ID_Поставщика = Поставщики.ID_Поставщика " +
                                            "JOIN Состав_поставки ON Поставка.ID_Поставки = Состав_поставки.ID_Поставки " +
                                            $"JOIN Книги ON Состав_поставки.ID_Книги = Книги.ID_Книги WHERE {column} LIKE @search";

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
    }
}