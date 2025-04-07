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
    public partial class Сотрудники : MetroForm
    {
        private Поставки formПоставки;
        private Продажа продажа;
        private DimForm dim;
        private bool open = false;
        private Добавление добавление;

        private static Сотрудники instance;
        public static Сотрудники Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Сотрудники();
                }
                return instance;
            }
        }

        public Сотрудники()
        {
            InitializeComponent();

            SetButtonColors(Color.FromArgb(255, 255, 255, 255));
            SetPanelColors(Color.FromArgb(255, 0, 174, 219));

            metroRadioButton1.Style = MetroColorStyle.Black;
            metroRadioButton2.Style = MetroColorStyle.Black;

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
            metroPanel4.BackColor = color;
            metroButton15.BackColor = color;
            metroButton1.BackColor = color;
        }

        private void SetPanelColors(Color color)
        {
            metroPanel1.BackColor = color;
            metroLabel1.BackColor = color;
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

        private void Сотрудники_Move(object sender, EventArgs e)
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
            SwapForms.ChangeForm(this, this);
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

        private void metroRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            metroRadioButton1.Checked = false;
            metroRadioButton2.Checked = false;
        }

        private void Сотрудники_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            добавление = new Добавление("Сотрудники");
            добавление.FormClosing += (s, args) => LoadTable();
            добавление.ShowDialog();
        }

        private void LoadTable()
        {
            DataTable dt = ConnectDB.select("select ID_Сотрудника, Фамилия, Имя, Отчество, Должность, Телефон, Админ from Сотрудники");

            if (dt != null)
            {
                metroGrid1.DataSource = dt;
            }
            else
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
        }

        private void metroButton10_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedCells.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedCells[0].RowIndex;

                if (metroGrid1.Rows[selectedIndex].Cells["ID_Сотрудника"].Value is int idToDelete)
                {
                    DialogResult result = MetroMessageBox.Show(
                        Owner, "Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ConnectDB.delete("Сотрудники", "ID_Сотрудника = @id", new Dictionary<string, object> { { "@id", idToDelete } });

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
                    MetroMessageBox.Show(Owner, "Некорректный идентификатор сотрудника.", "Книжный магазин");
                }
            }
            else
            {
                MetroMessageBox.Show(Owner, "Выберите строку для удаления.", "Книжный магазин");
            }
        }

        private void metroGrid1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string ID = metroGrid1.Rows[e.RowIndex].Cells["ID_Сотрудника"].Value.ToString();
                    string LastName = metroGrid1.Rows[e.RowIndex].Cells["Фамилия"].Value.ToString();
                    string FirstName = metroGrid1.Rows[e.RowIndex].Cells["Имя"].Value.ToString();
                    string Surname = metroGrid1.Rows[e.RowIndex].Cells["Отчество"].Value.ToString();
                    string JobTitle = metroGrid1.Rows[e.RowIndex].Cells["Должность"].Value.ToString();
                    string Phone = metroGrid1.Rows[e.RowIndex].Cells["Телефон"].Value.ToString();
                    bool IsAdmin = Convert.ToBoolean(metroGrid1.Rows[e.RowIndex].Cells["Админ"].Value);

                    добавление = new Добавление("СотрудникиUP");

                    добавление.metroTextBox2.Text = LastName;
                    добавление.metroTextBox3.Text = FirstName;
                    добавление.metroTextBox4.Text = Surname;
                    добавление.metroTextBox5.Text = JobTitle;
                    добавление.metroTextBox6.Text = Phone;
                    добавление.metroCheckBox2.Checked = IsAdmin;
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

        private void metroRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroRadioButton1.Checked)
            {
                DataTable dt = ConnectDB.select("select ID_Сотрудника, Фамилия, Имя, Отчество, Должность, Телефон, Админ from Сотрудники WHERE Админ = 1");

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
                string column = metroComboBox1.SelectedIndex == 0 ? "Фамилия" : "Должность";

                string query = $"SELECT ID_Сотрудника, Фамилия, Имя, Отчество, Должность, Телефон, Админ " +
                               $"FROM Сотрудники WHERE {column} LIKE @search";

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

        private void metroButton15_Click(object sender, EventArgs e)
        {
            metroRadioButton1.Checked = false;
            metroRadioButton2.Checked = false;

            metroTextBox2.Text = string.Empty;
            metroComboBox1.SelectedIndex = -1;

            LoadTable();
        }

        private void Сотрудники_Activated(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            PDFGenerator.СформироватьОтчетПоСотрудникам();
        }
    }
}