using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace Книжный
{
    public partial class Поставка_создание: MetroForm
    {
        private Добавление добавление;
        bool isLoading = false;
        private List<(int idКниги, string название, int количество)> поставленныеКниги = new List<(int idКниги, string название, int количество)>();
        public Поставка_создание()
        {
            InitializeComponent();
            setUp();

            metroGrid1.Columns.Clear();
            metroGrid1.Columns.Add("Название", "Название");
            metroGrid1.Columns.Add("Количество", "Количество");
            metroGrid1.Columns.Add("ID_Книги", "ID_Книги");

            metroGrid1.Columns["ID_Книги"].Visible = false;

            metroGrid1.ReadOnly = true;
            metroGrid1.AllowUserToAddRows = false;
            metroGrid1.AllowUserToDeleteRows = false;
            metroGrid1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            metroGrid1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            Resizable = false;
            MaximizeBox = false;
        }

        private void setUp()
        {
            try
            {
                isLoading = true;

                DataTable поставщики = ConnectDB.select("SELECT ID_Поставщика, Название FROM Поставщики");
                metroComboBox1.DataSource = поставщики;
                metroComboBox1.DisplayMember = "Название";
                metroComboBox1.ValueMember = "ID_Поставщика";
                metroComboBox1.SelectedIndex = -1;

                DataTable продавцы = ConnectDB.select("SELECT ID_Сотрудника, CONCAT(Фамилия, ' ', Имя) AS Сотрудник FROM Сотрудники WHERE Должность IN ('Продавец', 'Складской_Сотрудник', 'Бухгалтер')");
                metroComboBox2.DataSource = продавцы;
                metroComboBox2.DisplayMember = "Сотрудник";
                metroComboBox2.ValueMember = "ID_Сотрудника";
                metroComboBox2.SelectedIndex = -1;

                DataTable книги = ConnectDB.select("SELECT ID_Книги, Название, Цена FROM Книги");

                DataRow новаяКнига = книги.NewRow();
                новаяКнига["ID_Книги"] = -1;
                новаяКнига["Название"] = "Новая книга";
                книги.Rows.InsertAt(новаяКнига, 0);

                metroComboBox3.DataSource = книги;
                metroComboBox3.DisplayMember = "Название";
                metroComboBox3.ValueMember = "ID_Книги";
                metroComboBox3.SelectedIndex = -1;

                isLoading = false;
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message, "Ошибка");
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int idКниги = Convert.ToInt32(metroComboBox3.SelectedValue);
                string название = metroComboBox3.Text;
                int количество = Convert.ToInt32(metroTextBox1.Text);

                if (поставленныеКниги.Any(k => k.idКниги == idКниги))
                {
                    MetroMessageBox.Show(this, "Эта книга уже добавлена.", "Ошибка");
                    return;
                }

                поставленныеКниги.Add((idКниги, название, количество));

                metroGrid1.Rows.Add(название, количество, idКниги);

                metroTextBox1.Text = "";
                metroComboBox3.SelectedIndex = -1;

                metroGrid1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                metroGrid1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                metroGrid1.AutoResizeRows();
            }
            catch
            {
                MetroMessageBox.Show(this, "Введите корректное количество", "Ошибка");
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex == -1 || metroComboBox2.SelectedIndex == -1 || поставленныеКниги.Count == 0)
            {
                MetroMessageBox.Show(this, "Заполните все поля и добавьте хотя бы одну книгу.", "Ошибка");
                return;
            }

            if(metroDateTime1.Value > DateTime.Now)
            {
                MetroMessageBox.Show(Owner, "Дата поставки не может быть больше сегодняшней!", "Книжный магазин");
            }
            else
            {
                string дата = metroDateTime1.Value.ToString("yyyy-MM-dd");

                var поставка = new Dictionary<string, object>
                {
                    { "Дата_Поставки", дата },
                    { "ID_Сотрудника", metroComboBox2.SelectedValue },
                    { "ID_Поставщика", metroComboBox1.SelectedValue }
                };

                int idПоставки = ConnectDB.InsertAndGetId("Поставка", поставка);

                foreach (var (idКниги, _, _) in поставленныеКниги)
                {
                    DataTable dt = ConnectDB.select($"SELECT Цена FROM Книги WHERE ID_Книги = {idКниги}");
                    decimal цена = Convert.ToDecimal(dt.Rows[0]["Цена"]);

                    foreach (DataGridViewRow row in metroGrid1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int текущийID = Convert.ToInt32(row.Cells["ID_Книги"].Value);

                        if (текущийID == idКниги)
                        {
                            int количество = Convert.ToInt32(row.Cells["Количество"].Value);
                            decimal закупочнаяЦена = цена * количество;

                            var составПоставки = new Dictionary<string, object>
                            {
                                { "ID_Поставки", idПоставки },
                                { "ID_Книги", idКниги },
                                { "Закупочная_цена", закупочнаяЦена },
                                { "Количество", количество }
                            };

                            ConnectDB.insert("Состав_Поставки", составПоставки);
                            break;
                        }
                    }
                }
            }

            MetroMessageBox.Show(this, "Поставка успешно оформлена", "Успешно");
                Close();
            }

        private void metroTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) || (metroTextBox1.Text.Length == 0 && e.KeyChar == '0'))
            {
                e.Handled = true;
            }
        }

        private void metroComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            if (metroComboBox3.SelectedIndex == 0)
            {
                добавление = new Добавление("Книги");
                добавление.FormClosing += (s, args) => setUp();
                добавление.ShowDialog();
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedRows.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedRows[0].Index;

                int idКниги = Convert.ToInt32(metroGrid1.Rows[selectedIndex].Cells["ID_Книги"].Value);
                поставленныеКниги.RemoveAll(k => k.idКниги == idКниги);

                metroGrid1.Rows.RemoveAt(selectedIndex);
            }
            else
            {
                MetroMessageBox.Show(this, "Выберите строку для удаления.", "Книжный магазин");
            }
        }
    }
}
