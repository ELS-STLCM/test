namespace SimChartMedicalOffice.Core.DropBox
{
    public class DropBoxLink
    {
        /// <summary>
        /// This property holds the UID value for dropbox
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// This property holds the CID value for dropbox
        /// </summary>
        public string Cid { get; set; }
        
        /// <summary>
        /// This property holds the ScenarioId value for dropbox
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// This property holds the user role for dropbox (Admin/Instructor/Student
        /// </summary>
        public string UserRole { get; set; }
    }
}
