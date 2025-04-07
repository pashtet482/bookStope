using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;

namespace Книжный
{
    public partial class Авторизация : MetroForm
    {
        public Авторизация()
        {
            InitializeComponent();
            ActiveControl = metroLabel1;
            metroButton1.BackColor = Color.FromArgb(255, 0, 174, 219);
            metroButton2.BackColor = Color.FromArgb(255, 0, 174, 219);
            metroButton3.BackColor = Color.FromArgb(255, 0, 174, 219);
            Resizable = false;
            MaximizeBox = false;
            metroTextBox2.UseSystemPasswordChar = true;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string username = metroTextBox1.Text;
            string password = metroTextBox2.Text;

            bool isAdmin;
            if (ConnectDB.AuthenticateUser(username, password, out isAdmin))
            {
                MetroForm mainForm = isAdmin ? new Главная() : (MetroForm)new ГлавнаяНЕадмин(isAdmin);

                mainForm.FormClosed += (s, args) =>
                {
                    metroTextBox1.Clear();
                    metroTextBox2.Clear();
                    Show();
                };
                
                mainForm.Show();
                Hide();
            }
            else
            {
                MetroMessageBox.Show(this, "Неверный логин или пароль!", "Ошибка");
            }
        }

        private void metroButton2_MouseDown(object sender, MouseEventArgs e)
        {
            metroTextBox2.UseSystemPasswordChar = false;
        }

        private void metroButton2_MouseUp(object sender, MouseEventArgs e)
        {
            metroTextBox2.UseSystemPasswordChar = true;
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            Добавление добавление = new Добавление("Auth");
            добавление.ShowDialog();
        }
    }

    public static class CurrentUser
    {
        public static bool Role { get; set; } = false;
    }

}
