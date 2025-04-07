using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework;

namespace Книжный
{
    public partial class Поставщики: MetroForm
    {
        private Поставки formПоставки;
        private Продажа продажа;
        private DimForm dim;
        private bool open = false;
        private Добавление добавление;

        private static Поставщики instance;
        public static Поставщики Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new Поставщики();
                }
                return instance;
            }
        }
        public Поставщики()
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

        private void Поставщики_Move(object sender, EventArgs e)
        {
            if (dim != null && !dim.IsDisposed)
            {
                dim.Location = new Point(Location.X + (open ? 200 : 0), Location.Y);
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
            SwapForms.ChangeForm(this, this);
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

        private void Поставщики_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void LoadTable()
        {
            DataTable dt = ConnectDB.select("SELECT Поставщики.Название, Контактные_данные AS 'Контактные данные', COUNT(Поставка.ID_Поставщика) AS 'Кол-во поставок' FROM Поставщики " +
                "LEFT JOIN Поставка ON Поставщики.ID_Поставщика = Поставка.ID_Поставщика " +
                "GROUP BY Поставщики.Название");

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

                if (metroGrid1.Rows[selectedIndex].Cells["ID_Поставщика"].Value is int idToDelete)
                {
                    DialogResult result = MetroMessageBox.Show(
                        Owner, "Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ConnectDB.delete("Поставщики", "ID_Поставщика = @id", new Dictionary<string, object> { { "@id", idToDelete } });

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

        private bool isHandlingTextChanged = false;

        private void metroTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (isHandlingTextChanged) return;
            isHandlingTextChanged = true;
            
            string s = metroTextBox2.Text;

            string query = $"SELECT Поставщики.Название, Контактные_данные AS 'Контактные данные', COUNT(Поставка.ID_Поставщика) AS 'Кол-во поставок' FROM Поставщики " +
                           $"LEFT JOIN Поставка ON Поставщики.ID_Поставщика = Поставка.ID_Поставщика GROUP BY Поставщики.Название " +
                           $"WHERE Название LIKE @search";

            var parameters = new Dictionary<string, object>
            {
                { "@search", s + "%" }
            };

            DataTable dt = ConnectDB.select(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                metroGrid1.DataSource = dt;
            }
            

            isHandlingTextChanged = false;
        }

        private void metroButton15_Click(object sender, EventArgs e)
        {
            metroTextBox2.Text = string.Empty;
            LoadTable();
        }

        private bool asc = true;

        private void metroButton3_Click(object sender, EventArgs e) => SortBuyers(2);

        private void SortBuyers(int columnIndex)
        {
            string order = asc ? "ASC" : "DESC";
            asc = !asc;

            DataTable dt = ConnectDB.select($"SELECT Поставщики.Название, Контактные_данные AS 'Контактные данные', COUNT(Поставка.ID_Поставщика) AS 'Кол-во поставок' FROM Поставщики " +
                $"LEFT JOIN Поставка ON Поставщики.ID_Поставщика = Поставка.ID_Поставщика " +
                $"GROUP BY Поставщики.НазваниеORDER BY {columnIndex} {order}");

            if (dt == null || dt.Rows.Count == 0)
            {
                MetroMessageBox.Show(Owner, "Пустой ответ", "Книжный магазин");
            }
            else
            {
                metroGrid1.DataSource = dt;
            }
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            добавление = new Добавление(Name);
            добавление.FormClosing += (s, args) => LoadTable();
            добавление.ShowDialog();
        }

        private void metroGrid1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string ID = metroGrid1.Rows[e.RowIndex].Cells["ID_Поставщика"].Value.ToString();
                    string Name = metroGrid1.Rows[e.RowIndex].Cells["Название"].Value.ToString();
                    string contc = metroGrid1.Rows[e.RowIndex].Cells["Контактные данные"].Value.ToString();


                    добавление = new Добавление("ПоставщикиUP");

                    добавление.metroTextBox2.Text = Name;
                    добавление.metroTextBox3.Text = contc;
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

        private void Поставщики_Activated(object sender, EventArgs e)
        {
            LoadTable();
        }
    }
}
