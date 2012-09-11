using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        private void OpenDialogWindow(SimOfficeForm simOfficeForm)
        {
            simOfficeForm.ShowDialog();
        }
        private void btnCompetency_Click(object sender, EventArgs e)
        {
            OpenDialogWindow((SimOfficeForm) new CompetencyForm());
        }

        
    }
}
