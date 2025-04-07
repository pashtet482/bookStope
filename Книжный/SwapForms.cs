using MetroFramework.Forms;
using System.Windows.Forms;

namespace Книжный
{
    class SwapForms
    {
        public static void ChangeForm(MetroForm currentForm, MetroForm newForm)
        {
            if (currentForm != null && !currentForm.IsDisposed)
            {
                currentForm.Hide();
            }

            if (newForm != null && !newForm.IsDisposed)
            {
                newForm.Show();
                newForm.WindowState = FormWindowState.Normal;
                newForm.Activate();
            }
        }
    }
}
