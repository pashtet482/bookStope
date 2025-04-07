using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace Книжный
{
    public partial class ПродажаСоздание: MetroForm
    {
        private Добавление добавление;
        bool isLoading = false;
        private List<(int idКниги, string название, int количество)> книгиВЧеке = new List<(int idКниги, string название, int количество)>();

        public ПродажаСоздание()
        {
            InitializeComponent();

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
            metroLabelFullPrice.Text = "Итого: 0 руб";

            setUp();
        }

        private void setUp()
        {
            try
            {
                isLoading = true;

                DataTable покупателиDataTable = ConnectDB.select("select ID_Покупателя, CONCAT(Покупатели.Фамилия, ' ', Покупатели.Имя) AS 'Покупатель' from Покупатели");
                DataRow новыйПокупатель = покупателиDataTable.NewRow();
                новыйПокупатель["ID_Покупателя"] = -1;
                новыйПокупатель["Покупатель"] = "Новый покупатель";
                покупателиDataTable.Rows.InsertAt(новыйПокупатель, 0);

                metroComboBox1.DataSource = покупателиDataTable;
                metroComboBox1.DisplayMember = "Покупатель";
                metroComboBox1.ValueMember = "ID_Покупателя";
                metroComboBox1.SelectedIndex = -1;

                DataTable продавцыDataTable = ConnectDB.select("select ID_Сотрудника, CONCAT(Сотрудники.Фамилия, ' ', Сотрудники.Имя) AS 'Сотрудник' from Сотрудники " +
                                                                "WHERE Должность IN ('Продавец', 'Кассир');");
                metroComboBox2.DataSource = продавцыDataTable;
                metroComboBox2.DisplayMember = "Сотрудник";
                metroComboBox2.ValueMember = "ID_Сотрудника";
                metroComboBox2.SelectedIndex = -1;

                DataTable книгиDataTable = ConnectDB.select("select ID_Книги, Название from Книги");
                metroComboBox3.DataSource = книгиDataTable;
                metroComboBox3.DisplayMember = "Название";
                metroComboBox3.ValueMember = "ID_Книги";
                metroComboBox3.SelectedIndex = -1;

                isLoading = false;
            }
            catch(Exception ex)
            {
                MetroMessageBox.Show(Owner, ex.Message, "Книжный магазин");
            }
        }

        private void ОбновитьСтоимостьЧека()
        {
            decimal сумма = 0;

            foreach (var (idКниги, _, количество) in книгиВЧеке)
            {
                DataTable dt = ConnectDB.select($"SELECT Цена FROM Книги WHERE ID_Книги = {idКниги}");
                if (dt.Rows.Count > 0)
                {
                    decimal цена = Convert.ToDecimal(dt.Rows[0]["Цена"]);
                    сумма += цена * количество;
                }
            }

            metroLabelFullPrice.Text = $"Итого: {сумма} руб \nвключая НДС 20%";
        }


        private void metroButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (metroComboBox1.SelectedIndex == -1 ||
                    metroComboBox2.SelectedIndex == -1 ||
                    metroGrid1.Rows.Count == 0)
                {
                    MetroMessageBox.Show(Owner, "Укажите покупателя, продавца и добавьте хотя бы одну книгу.", "Книжный магазин");
                    return;
                }

                if (metroDateTime1.Value > DateTime.Now)
                {
                    MetroMessageBox.Show(Owner, "Дата продажи не может быть больше сегодняшней!", "Книжный магазин");
                    return;
                }

                string дата = metroDateTime1.Value.ToString("yyyy-MM-dd");

                var продажа = new Dictionary<string, object>
        {
            { "Дата_продажи", дата },
            { "ID_Покупателя", metroComboBox1.SelectedValue },
            { "ID_Сотрудника", metroComboBox2.SelectedValue }
        };

                int idПродажи = ConnectDB.InsertAndGetId("Продажи", продажа);

                bool всеУспешно = true;

                foreach (var (idКниги, _, количество) in книгиВЧеке)
                {
                    var запись = new Dictionary<string, object>
                {
                    { "ID_Продажи", idПродажи },
                    { "ID_Книги", idКниги },
                    { "Количество", количество }
                };

                    if (!ConnectDB.insert("Состав_Продажи", запись))
                    {
                        всеУспешно = false;
                        break;
                    }
                }

                decimal сумма = 0;

                foreach (var (idКниги, _, количество) in книгиВЧеке)
                {
                    DataTable dt = ConnectDB.select($"SELECT Цена FROM Книги WHERE ID_Книги = {idКниги}");
                    if (dt.Rows.Count > 0)
                    {
                        decimal цена = Convert.ToDecimal(dt.Rows[0]["Цена"]);
                        сумма += цена * количество;
                    }
                }

                decimal ндс = сумма * 0.2m;
                decimal итоговаяСумма = сумма + ндс;


                MetroMessageBox.Show(Owner,
                    всеУспешно ? "Покупка успешно оформлена." : "Произошла ошибка при оформлении покупки.",
                    "Книжный магазин");

                if (всеУспешно)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(Owner, "Ошибка при оформлении покупки: " + ex.Message, "Ошибка");
            }
        }


        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            if (metroComboBox1.SelectedIndex == 0)
            {
                добавление = new Добавление("Покупатели");
                добавление.FormClosing += (s, args) => setUp();
                добавление.ShowDialog();
            }
        }

        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (metroTextBox1.Text != string.Empty)
                {

                    int количество = Convert.ToInt32(metroTextBox1.Text);
                    int idКниги = Convert.ToInt32(metroComboBox3.SelectedValue);

                    DataTable z = ConnectDB.select(
                        $"SELECT Цена FROM Книги WHERE ID_Книги = {idКниги}"
                    );

                    if (z.Rows.Count > 0)
                    {
                        int цена = Convert.ToInt32(z.Rows[0]["Цена"]);
                        double стоимость = цена * количество * 1.2;
                        metroLabelPrice.Text = Math.Round(стоимость, 2) + " руб включая НДС 20%";
                    }
                    else
                    {
                        MetroMessageBox.Show(Owner, "Книга не найдена.", "Ошибка");
                    }
                }
                else
                {
                    metroLabelPrice.Text = "";
                }
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(Owner, "Ошибка при расчёте стоимости: " + ex.Message, "Ошибка");
            }
        }

        private void metroTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) || (metroTextBox1.Text.Length == 0 && e.KeyChar == '0'))
            {
                e.Handled = true;
            }
        }

        
        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (metroComboBox3.SelectedIndex == -1 || string.IsNullOrWhiteSpace(metroTextBox1.Text))
                {
                    MetroMessageBox.Show(this, "Укажите книгу и количество.", "Ошибка");
                    return;
                }

                int idКниги = Convert.ToInt32(metroComboBox3.SelectedValue);
                string название = metroComboBox3.Text;
                int количество = Convert.ToInt32(metroTextBox1.Text);

                if (книгиВЧеке.Any(k => k.idКниги == idКниги))
                {
                    MetroMessageBox.Show(this, "Эта книга уже добавлена в чек.", "Ошибка");
                    return;
                }

                string sql = $"SELECT Количество_на_складе FROM Книги WHERE ID_Книги = {idКниги}";
                DataTable result = ConnectDB.select(sql);

                if (result.Rows.Count == 0)
                {
                    MetroMessageBox.Show(this, "Ошибка при получении данных о книге.", "Ошибка");
                    return;
                }

                int доступно = Convert.ToInt32(result.Rows[0]["Количество_на_складе"]);

                if (количество > доступно)
                {
                    MetroMessageBox.Show(this, $"Недостаточно книг на складе. В наличии: {доступно}", "Ошибка");

                    metroTextBox1.Text = "";
                    metroComboBox3.SelectedIndex = -1;
                    metroLabelPrice.Text = "";

                    return;
                }

                книгиВЧеке.Add((idКниги, название, количество));
                metroGrid1.Rows.Add(название, количество, idКниги);
                ОбновитьСтоимостьЧека();

                metroTextBox1.Text = "";
                metroComboBox3.SelectedIndex = -1;
                metroLabelPrice.Text = "";

                metroGrid1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                metroGrid1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                metroGrid1.AutoResizeRows();
            }
            catch (FormatException)
            {
                MetroMessageBox.Show(this, "Введите корректное количество", "Ошибка");
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, "Ошибка при добавлении книги: " + ex.Message, "Ошибка");
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if (metroGrid1.SelectedRows.Count > 0)
            {
                int selectedIndex = metroGrid1.SelectedRows[0].Index;

                int idКниги = Convert.ToInt32(metroGrid1.Rows[selectedIndex].Cells["ID_Книги"].Value);
                книгиВЧеке.RemoveAll(k => k.idКниги == idКниги);

                metroGrid1.Rows.RemoveAt(selectedIndex);
                ОбновитьСтоимостьЧека();
            }
            else
            {
                MetroMessageBox.Show(this, "Выберите строку для удаления.", "Книжный магазин");
            }
        }
    }
}
