using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework;
using System.Globalization;

namespace Книжный
{
    public partial class Добавление : MetroForm
    {
        private string FormName;
        internal string ID;
        private bool success;

        public Добавление(string FormName = "default")
        {
            InitializeComponent();
            metroButton1.BackColor = Color.FromArgb(255, 255, 255, 255);
            this.FormName = FormName ?? "default";

            Resizable = false;
            MaximizeBox = false;
            metroCheckBox2.Visible = false;

            setUpForm();
        }


        private void setUpForm()
        {
            switch (FormName)
            {
                case ("Книги"):

                    metroLabel1.Text = "Название";
                    metroLabel2.Text = "Автор";
                    metroLabel3.Text = "Жанр";
                    metroLabel4.Text = "Год издания";
                    metroLabel5.Text = "Цена";
                    metroLabel6.Text = "Издательство";
                    metroLabel7.Text = "ISBN";
                    metroLabel8.Text = "Количество";
                    metroButton1.Text = "Добавить \nкнигу";

                    metroTextBox5.MaxLength = 4;
                    metroTextBox8.MaxLength = 13;

                    Size = new Size(335, 555);
                    metroButton1.Location = new Point(47, 486);

                    break;

                case ("default"):
                    break;

                case ("КнигиUP"):

                    Text = "Изменить данные о книге";
                    metroLabel1.Text = "Изменить \nназвание";
                    metroLabel2.Text = "Изменить \nавтора";
                    metroLabel3.Text = "Изменить \nжанр";
                    metroLabel4.Text = "Изменить \nгод издания";
                    metroLabel5.Text = "Изменить \nцену";
                    metroLabel6.Text = "Изменить \nиздательство";
                    metroLabel7.Text = "Изменить \nISBN";
                    metroLabel8.Text = "Изменить \nколичество";
                    metroButton1.Text = "Внести изменения \nо книге";

                    metroTextBox5.MaxLength = 4;
                    metroTextBox8.MaxLength = 13;

                    Size = new Size(335, 555);
                    metroButton1.Location = new Point(47, 486);

                    break;

                case ("Auth"):
                    Text = "Регистрация";
                    metroLabel1.Text = "Фамилия";
                    metroLabel2.Text = "Имя";
                    metroLabel3.Text = "Отчество \n(если есть)";
                    metroLabel4.Text = "Должность";
                    metroLabel5.Text = "Телефон";
                    metroLabel6.Text = "Логин";
                    metroLabel7.Text = "Пароль";
                    metroLabel8.Text = "Администратор";
                    metroButton1.Text = "Зарегистрироваться";

                    metroCheckBox2.Visible = true;
                    metroTextBox6.MaxLength = 11;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 555);
                    metroButton1.Location = new Point(47, 486);
                    break;

                case ("Сотрудники"):
                    Text = "Регистрация";
                    metroLabel1.Text = "Фамилия";
                    metroLabel2.Text = "Имя";
                    metroLabel3.Text = "Отчество \n(если есть)";
                    metroLabel4.Text = "Должность";
                    metroLabel5.Text = "Телефон";
                    metroLabel6.Text = "Логин";
                    metroLabel7.Text = "Пароль";
                    metroLabel8.Text = "Администратор";
                    metroButton1.Text = "Зарегистрироваться";

                    metroCheckBox2.Visible = true;
                    metroTextBox6.MaxLength = 11;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 555);
                    metroButton1.Location = new Point(47, 486);
                    break;

                case ("СотрудникиUP"):
                    Text = "Изменить данные";
                    metroLabel1.Text = "Фамилия";
                    metroLabel2.Text = "Имя";
                    metroLabel3.Text = "Отчество \n(если есть)";
                    metroLabel4.Text = "Должность";
                    metroLabel5.Text = "Телефон";
                    metroLabel6.Text = "Администратор";
                    metroButton1.Text = "Обновить данные";

                    metroCheckBox2.Visible = true;
                    metroCheckBox2.Location = new Point(metroTextBox7.Location.X, metroTextBox7.Location.Y);
                    metroTextBox7.Visible = false;
                    metroTextBox8.Visible = false;
                    metroLabel7.Visible = false;
                    metroLabel8.Visible = false;
                    metroTextBox6.MaxLength = 11;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 400);
                    metroButton1.Location = new Point(47, 354);
                    break;

                case ("ПокупателиUP"):
                    Text = "Изменить данные";
                    metroLabel1.Text = "Фамилия";
                    metroLabel2.Text = "Имя";
                    metroLabel3.Text = "Отчество \n(если есть)";
                    metroLabel4.Text = "Телефон";
                    metroLabel5.Text = "email";
                    metroButton1.Text = "Обновить данные";

                    metroTextBox5.MaxLength = 11;
                    metroLabel6.Visible = false;
                    metroLabel7.Visible = false;
                    metroLabel8.Visible= false;
                    metroTextBox7.Visible = false;
                    metroTextBox8.Visible = false;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 400);
                    metroButton1.Location = new Point(32, 302);
                    break;

                case ("Покупатели"):
                    Text = "Добавить покупателя";
                    metroLabel1.Text = "Фамилия";
                    metroLabel2.Text = "Имя";
                    metroLabel3.Text = "Отчество \n(если есть)";
                    metroLabel4.Text = "Телефон";
                    metroLabel5.Text = "email";
                    metroButton1.Text = "Добавить покупателя";

                    metroTextBox5.MaxLength = 11;
                    metroLabel6.Visible = false;
                    metroLabel7.Visible = false;
                    metroLabel8.Visible = false;
                    metroTextBox7.Visible = false;
                    metroTextBox8.Visible = false;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 400);
                    metroButton1.Location = new Point(32, 302);
                    break;

                case ("Поставщики"):
                    Text = "Добавить поставщика";
                    metroLabel1.Text = "Название";
                    metroLabel2.Text = "Контактные\nданные";
                    metroButton1.Text = "Добавить поставщика";

                    metroLabel3.Visible = false;
                    metroLabel4.Visible = false;
                    metroLabel5.Visible = false;
                    metroLabel6.Visible = false;
                    metroLabel7.Visible = false;
                    metroLabel8.Visible = false;

                    metroTextBox4.Visible = false;
                    metroTextBox5.Visible = false;
                    metroTextBox6.Visible = false;
                    metroTextBox7.Visible = false;
                    metroTextBox8.Visible = false;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 217);
                    metroButton1.Location = new Point(47, 155);
                    break;

                case ("ПоставщикиUP"):
                    Text = "Изменить данные";
                    metroLabel1.Text = "Изменить\nназвание";
                    metroLabel2.Text = "Изменить\nконтактные данные";
                    metroButton1.Text = "Добавить поставщика";

                    metroLabel3.Visible = false;
                    metroLabel4.Visible = false;
                    metroLabel5.Visible = false;
                    metroLabel6.Visible = false;
                    metroLabel7.Visible = false;
                    metroLabel8.Visible = false;

                    metroTextBox4.Visible = false;
                    metroTextBox5.Visible = false;
                    metroTextBox6.Visible = false;
                    metroTextBox7.Visible = false;
                    metroTextBox8.Visible = false;
                    metroTextBox9.Visible = false;

                    Size = new Size(335, 217);
                    metroButton1.Location = new Point(47, 155);
                    break;
            }
        }

        internal void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                switch (FormName)
                {
                    case ("Книги"):

                        if (int.TryParse(metroTextBox5.Text, out int year) && year >= 1900 && year <= DateTime.Now.Year)
                        {
                            var parameters = new Dictionary<string, object>
                        {
                            {"Название", metroTextBox2.Text},
                            {"Автор", metroTextBox3.Text},
                            {"Жанр", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                            {"Год_издания", year},
                            {"Цена", decimal.Parse(metroTextBox6.Text.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)},
                            {"Издательство", string.IsNullOrEmpty(metroTextBox7.Text) ? (object)DBNull.Value : metroTextBox7.Text},
                            {"ISBN", metroTextBox8.Text},
                            {"Количество_на_складе", Convert.ToInt32(metroTextBox9.Text)}
                        };

                            success = ConnectDB.insert("Книги", parameters);

                            MetroMessageBox.Show(Owner, success ? "Книга успешно добавлена." : "Книга не добавлена.", "Книжный магазин");
                            if (success) Close();
                        }
                        else
                        {
                            MetroMessageBox.Show(Owner, "Введите корректный год!", "Книжный магазин");
                        }
                        break;

                    case ("КнигиUP"):

                        if (int.TryParse(metroTextBox5.Text, out year) && year >= 1900 && year <= DateTime.Now.Year)
                        {
                            var updateValues = new Dictionary<string, object>
                        {
                            {"Название", metroTextBox2.Text},
                            {"Автор", metroTextBox3.Text},
                            {"Жанр", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                            {"Год_издания", year},
                            {"Цена", decimal.Parse(metroTextBox6.Text.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)},
                            {"Издательство", string.IsNullOrEmpty(metroTextBox7.Text) ? (object)DBNull.Value : metroTextBox7.Text},
                            {"ISBN", metroTextBox8.Text},
                            {"Количество_на_складе", Convert.ToInt32(metroTextBox9.Text)}
                        };

                            var condition = "ID_Книги = @ID";
                            var conditionParams = new Dictionary<string, object> { { "ID", ID } };

                            success = ConnectDB.update("Книги", updateValues, condition, conditionParams);

                            MetroMessageBox.Show(Owner, success ? "Книга успешно обновлена." : "Книга не обновлена.", "Книжный магазин");
                            if (success) Close();
                        }
                        else
                        {
                            MetroMessageBox.Show(Owner, "Введите корректный год!", "Книжный магазин");
                        }
                        break;

                    case ("Auth"):

                        var authParams = new Dictionary<string, object>
                    {
                        {"Фамилия", metroTextBox2.Text},
                        {"Имя", metroTextBox3.Text},
                        {"Отчество", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                        {"Должность", metroTextBox5.Text},
                        {"Телефон", metroTextBox6.Text},
                        {"Логин", metroTextBox7.Text},
                        {"Пароль", BCrypt.Net.BCrypt.HashPassword(metroTextBox8.Text)},
                        {"Админ", metroCheckBox2.Checked}
                    };

                        success = ConnectDB.insert("Сотрудники", authParams);

                        MetroMessageBox.Show(Owner, success ? "Регистрация успешна!" : "Регистрация не удалась.", "Книжный магазин");
                        if (success) Close();
                        break;

                    case ("Сотрудники"):

                        var empAddParams = new Dictionary<string, object>
                    {
                        {"Фамилия", metroTextBox2.Text},
                        {"Имя", metroTextBox3.Text},
                        {"Отчество", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                        {"Должность", metroTextBox5.Text},
                        {"Телефон", metroTextBox6.Text},
                        {"Логин", metroTextBox7.Text},
                        {"Пароль", BCrypt.Net.BCrypt.HashPassword(metroTextBox8.Text)},
                        {"Админ", metroCheckBox2.Checked}
                    };

                        success = ConnectDB.insert("Сотрудники", empAddParams);

                        MetroMessageBox.Show(Owner, success ? "Сотрудник успешно добавлен." : "Сотрудник не добавлен.", "Книжный магазин");
                        if (success) Close();

                        break;

                    case ("Покупатели"):

                        var buyersParams = new Dictionary<string, object>
                    {
                        {"Фамилия", metroTextBox2.Text},
                        {"Имя", metroTextBox3.Text},
                        {"Отчество", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                        {"Телефон", metroTextBox5.Text},
                        {"Email", metroTextBox6.Text},
                    };

                        success = ConnectDB.insert("Покупатели", buyersParams);

                        MetroMessageBox.Show(Owner, success ? "Покупатель успешно добавлен." : "Покупатель не добавлен.", "Книжный магазин");
                        if (success) Close();

                        break;

                    case ("СотрудникиUP"):

                        var employeeUpdateParams = new Dictionary<string, object>
                    {
                        {"Фамилия", metroTextBox2.Text},
                        {"Имя", metroTextBox3.Text},
                        {"Отчество", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                        {"Должность", metroTextBox5.Text},
                        {"Телефон", metroTextBox6.Text},
                        {"Админ", metroCheckBox2.Checked}
                    };

                        var empCondition = "ID_Сотрудника = @ID";
                        var empConditionParams = new Dictionary<string, object> { { "ID", ID } };

                        success = ConnectDB.update("Сотрудники", employeeUpdateParams, empCondition, empConditionParams);

                        MetroMessageBox.Show(Owner, success ? "Данные успешно обновлены!" : "Данные не обновлены.", "Книжный магазин");
                        if (success) Close();
                        break;

                    case ("ПокупателиUP"):

                        var buyersUpParams = new Dictionary<string, object>
                    {
                        {"Фамилия", metroTextBox2.Text},
                        {"Имя", metroTextBox3.Text},
                        {"Отчество", string.IsNullOrEmpty(metroTextBox4.Text) ? (object)DBNull.Value : metroTextBox4.Text},
                        {"Телефон", metroTextBox5.Text},
                        {"Email", metroTextBox6.Text},
                    };

                        var buyersCondition = "ID_Покупателя  = @ID";
                        var buyersConditionParams = new Dictionary<string, object> { { "ID", ID } };

                        success = ConnectDB.update("Покупатели", buyersUpParams, buyersCondition, buyersConditionParams);

                        MetroMessageBox.Show(Owner, success ? "Данные успешно обновлены!" : "Данные не обновлены.", "Книжный магазин");
                        if (success) Close();

                        break;

                    case ("Поставщики"):

                        var supplParams = new Dictionary<string, object>
                    {
                        {"Название", metroTextBox2.Text},
                        {"Контактные_данные", metroTextBox3.Text},
                    };

                        success = ConnectDB.insert("Поставщики", supplParams);

                        MetroMessageBox.Show(Owner, success ? "Поставщик успешно добавлен." : "Поставщик не добавлен.", "Книжный магазин");
                        if (success) Close();

                        break;

                    case ("ПоставщикиUP"):

                        var supplUpParams = new Dictionary<string, object>
                    {
                        {"Название", metroTextBox2.Text},
                        {"Контактные_данные", metroTextBox3.Text},
                    };

                        var supplCondition = "ID_Поставщика  = @ID";
                        var supplConditionParams = new Dictionary<string, object> { { "ID", ID } };

                        success = ConnectDB.update("Поставщики", supplUpParams, supplCondition, supplConditionParams);

                        MetroMessageBox.Show(Owner, success ? "Данные успешно обновлены!" : "Данные не обновлены.", "Книжный магазин");
                        if (success) Close();

                        break;
                }
            }
            catch(Exception ex)
            {
                MetroMessageBox.Show(Owner, ex.ToString(), "Книжный магазин");
            }
        }


        private void metroTextBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Книги" || FormName == "КнигиUP")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void metroTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(FormName == "Книги" || FormName == "КнигиUP" || FormName == "Покупатели" || FormName == "ПокупателиUP")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            
        }

        private void metroTextBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Книги" || FormName == "КнигиUP" || FormName == "СотрудникиUP" || FormName == "Сотрудники" || FormName == "Auth")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void metroTextBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Книги" || FormName == "КнигиUP")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void metroTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Сотрудники" || FormName == "СотрудникиUP" || FormName == "Покупатели" || FormName == "ПокупателиUP" || FormName == "Auth")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void metroTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Сотрудники" || FormName == "СотрудникиUP" || FormName == "Покупатели" || FormName == "ПокупателиUP" || FormName == "Auth")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void metroTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FormName == "Сотрудники" || FormName == "СотрудникиUP" || FormName == "Покупатели" || FormName == "ПокупателиUP" || FormName == "Auth")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }
    }
}
