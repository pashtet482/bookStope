using MetroFramework;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Книжный
{
    public partial class Книги : MetroForm
    {
        private Поставки formПоставки;
        private Продажа продажа;
        private DimForm dim;
        private bool open = false;
        private Добавление добавление;

        private static Книги instance;
        public static Книги Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Книги();
                }
                return instance;
            }
        }
        public Книги()
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
        }

        private void SetButtonColors(Color color)
        {
            metroButton8.BackColor = color;
            metroButton10.BackColor = color;
            metroButton2.BackColor = color;
            metroButton3.BackColor = color;
            metroPanel4.BackColor = color;
            metroButton15.BackColor = color;
            metroButton14.BackColor = color;
        }

        private void SetPanelColors(Color color)
        {
            metroPanel1.BackColor = color;
            metroLabel1.BackColor = color;
            metroPanel2.BackColor = color;
            metroPanel3.BackColor = color;
            metroPanel5.BackColor = color;
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

        private void Книги_Move(object sender, EventArgs e)
        {
            if (dim != null && !dim.IsDisposed)
            {
                dim.Location = new Point(Location.X + (open ? 200 : 0), Location.Y);
            }
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex == 4)
            {
                metroComboBox1.SelectedIndex = -1;
                LoadTable();
                metroTextBox2.Text = null;
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, Главная.Instance);
            SideBar();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            SwapForms.ChangeForm(this, this);
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
            SwapForms.ChangeForm(this, Продажа.Instance);
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

        private void Книги_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            добавление = new Добавление(Name);
            добавление.FormClosing += (s, args) => LoadTable();
            добавление.ShowDialog();
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedCells.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedCells[0].RowIndex;

                if (metroGrid1.Rows[selectedIndex].Cells["ID_Книги"].Value is int idToDelete)
                {
                    DialogResult result = MetroMessageBox.Show(
                        Owner, "Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ConnectDB.delete("Книги", "ID_Книги = @id", new Dictionary<string, object> { { "@id", idToDelete } });

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

        private void LoadTable()
        {
            DataTable dt = ConnectDB.select("select ID_Книги, Название, Автор, Жанр, Год_издания AS 'Год издания', Цена, Издательство, ISBN, " +
                                            "Количество_на_складе AS 'Количество' from Книги");

            if (dt != null)
            {
                metroGrid1.DataSource = dt;
            }
            else
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
        }

        private bool asc = true;

        private void metroButton3_Click(object sender, EventArgs e) => SortBooks(2);

        private void metroButton2_Click(object sender, EventArgs e) => SortBooks(5);

        private void SortBooks(int columnIndex)
        {
            string order = asc ? "ASC" : "DESC";
            asc = !asc;

            DataTable dt = ConnectDB.select($"SELECT ID_Книги, Название, Автор, Жанр, Год_издания AS 'Год издания', Цена, Издательство, ISBN, " +
                                            $"Количество_на_складе AS 'Количество' " +
                                            $"FROM Книги ORDER BY {columnIndex} {order}");

            if (dt == null || dt.Rows.Count == 0)
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
            else
            {
                metroGrid1.DataSource = dt;
            }
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
                    case 0: column = "Название"; break;
                    case 1: column = "Автор"; break;
                    case 2: column = "Жанр"; break;
                    case 3: column = "Год_издания"; break;
                    default: column = "Название"; break;
                }

                string query = $"SELECT ID_Книги, Название, Автор, Жанр, Год_издания AS 'Год издания', Цена, " +
                               $"Издательство, ISBN, Количество_на_складе AS 'Количество' " +
                               $"FROM Книги WHERE {column} LIKE @search";

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

        private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCheckBox2.Checked)
            {
                DataTable dt = ConnectDB.select($"SELECT ID_Книги, Название, Автор, Жанр, Год_издания AS 'Год издания', Цена, Издательство, ISBN, " +
                                                $"Количество_на_складе AS 'Количество' " +
                                                $"FROM Книги WHERE Цена <= 500");

                if (dt == null || dt.Rows.Count == 0)
                {
                    MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
                }
                else
                {
                    metroGrid1.DataSource = dt;
                }
            }

            else
            {
                LoadTable();
            }
        }

        private void metroButton15_Click(object sender, EventArgs e)
        {
            metroCheckBox1.Checked = false;
            metroCheckBox2.Checked = false;
            metroCheckBox3.Checked = false;

            metroTextBox2.Text = string.Empty;
            metroComboBox1.SelectedIndex = -1;

            LoadTable();
        }

        private void metroGrid1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string ID = metroGrid1.Rows[e.RowIndex].Cells["ID_Книги"].Value.ToString();
                    string Name = metroGrid1.Rows[e.RowIndex].Cells["Название"].Value.ToString();
                    string Author = metroGrid1.Rows[e.RowIndex].Cells["Автор"].Value.ToString();
                    string Genre = metroGrid1.Rows[e.RowIndex].Cells["Жанр"].Value.ToString();
                    string Year = metroGrid1.Rows[e.RowIndex].Cells["Год издания"].Value.ToString();
                    string Price = metroGrid1.Rows[e.RowIndex].Cells["Цена"].Value.ToString();
                    string Publisher = metroGrid1.Rows[e.RowIndex].Cells["Издательство"].Value.ToString();
                    string ISBN = metroGrid1.Rows[e.RowIndex].Cells["ISBN"].Value.ToString();
                    string count = metroGrid1.Rows[e.RowIndex].Cells["Количество"].Value.ToString();


                    добавление = new Добавление("КнигиUP");

                    добавление.metroTextBox2.Text = Name;
                    добавление.metroTextBox3.Text = Author;
                    добавление.metroTextBox4.Text = Genre;
                    добавление.metroTextBox5.Text = Year;
                    добавление.metroTextBox6.Text = Price;
                    добавление.metroTextBox7.Text = Publisher;
                    добавление.metroTextBox8.Text = ISBN;
                    добавление.metroTextBox9.Text = count;
                    добавление.ID = ID;

                    добавление.FormClosing += (s, args) => LoadTable();
                    добавление.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(Owner, "Ошибка: " + ex.Message, "Книжный магазин");
            }
        }

        private void metroButton14_Click(object sender, EventArgs e)
        {
            DataTable dt = ConnectDB.select("select Название, Автор, Количество_на_складе AS 'Количество' from Книги");

            PDFGenerator.GenerateBookReport(dt);
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCheckBox1.Checked)
            {
                DataTable dt = ConnectDB.select($"SELECT ID_Книги, Название, Автор, Жанр, Год_издания AS 'Год издания', Цена, Издательство, ISBN, " +
                                                $"Количество_на_складе AS 'Количество' " +
                                                $"FROM Книги WHERE Количество_на_складе > 1");

                if (dt == null || dt.Rows.Count == 0)
                {
                    MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
                }
                else
                {
                    metroGrid1.DataSource = dt;
                }
            }

            else
            {
                LoadTable();
            }
        }

        private void metroCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCheckBox3.Checked)
            {
                DateTime today = DateTime.Today;
                int delta = DayOfWeek.Monday - today.DayOfWeek;
                DateTime startOfWeek = today.AddDays(delta);
                DateTime endOfWeek = startOfWeek.AddDays(6);

                string query = @"SELECT 
                                Книги.ID_Книги, 
                                Книги.Название, 
                                Книги.Автор, 
                                Книги.Жанр, 
                                Книги.Год_издания AS 'Год издания', 
                                Книги.Цена, 
                                Книги.Издательство, 
                                Книги.ISBN,
                                Книги.Количество_на_складе AS 'Количество'
                                FROM Книги
                                JOIN Состав_поставки ON Книги.ID_Книги = Состав_поставки.ID_книги
                                JOIN Поставка ON Состав_поставки.ID_поставки = Поставка.ID_поставки
                                WHERE Поставка.Дата_поставки BETWEEN @startOfWeek AND @endOfWeek";

                var parameters = new Dictionary<string, object>()
        {
            {"@startOfWeek", startOfWeek},
            {"@endOfWeek", endOfWeek}
        };

                DataTable dt = ConnectDB.select(query, parameters);
                metroGrid1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("На этой неделе новинок нет.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                LoadTable();
            }
        }

        private void Книги_Activated(object sender, EventArgs e)
        {
            LoadTable();
        }
    }
}