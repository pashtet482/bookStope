using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Книжный
{
    public partial class DimForm : Form
    {
        private Action closeSidebar;

        public DimForm(Книги parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        public DimForm(Покупатели parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        public DimForm(Поставщики parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        public DimForm(Сотрудники parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        public DimForm(Продажа parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        public DimForm(Поставки parent, Action closeSidebarAction)
        {
            InitializeDimForm(parent, closeSidebarAction);
        }

        private DimForm()
        {
            Close();
        }

        private void InitializeDimForm(MetroForm parent, Action closeSidebarAction)
        {
            closeSidebar = closeSidebarAction;

            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            Opacity = 0.5;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = parent.ClientSize;
            Location = parent.Location;
            Width -= 200;
            Left += 200;
            Owner = parent;
        }

        private void DimForm_Click(object sender, EventArgs e)
        {
            closeSidebar?.Invoke();
        }
    }
}
