using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimChartMedicalOffice.MasterUpload.Forms
{
    public partial class SimOfficeForm : System.Windows.Forms.Form
    {
        public SimOfficeForm()
        {
            if (this.IsMdiContainer == false)
            {
                this.ShowInTaskbar = false;                
                this.MaximizeBox = false;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }
    }
}
