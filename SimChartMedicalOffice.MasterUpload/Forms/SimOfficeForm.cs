
namespace SimChartMedicalOffice.MasterUpload.Forms
{
    public partial class SimOfficeForm : System.Windows.Forms.Form
    {
        public SimOfficeForm()
        {
            if (IsMdiContainer == false)
            {
                ShowInTaskbar = false;                
                MaximizeBox = false;
                WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }
    }
}
