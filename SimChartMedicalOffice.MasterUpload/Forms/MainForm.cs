using System;
using System.Windows.Forms;
using SimChartMedicalOffice.MasterUpload.Forms;

namespace SimChartMedicalOffice.MasterUpload
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {

        }
        private void OpenDialogWindow(SimOfficeForm simOfficeForm)
        {
            simOfficeForm.ShowDialog();
        }
        private void BtnCompetencyClick(object sender, EventArgs e)
        {
            OpenDialogWindow(new CompetencyForm());
        }

        
    }
}
