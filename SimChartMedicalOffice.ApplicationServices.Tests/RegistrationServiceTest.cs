using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimChartMedicalOffice.Core.TempObject;

namespace SimChartMedicalOffice.ApplicationServices.Tests
{


    /// <summary>
    ///This is a test class for RegistrationServiceTest and is intended
    ///to contain all RegistrationServiceTest Unit Tests
    ///</summary>
    [TestClass]
    public class RegistrationServiceTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SavePatientAppointment
        ///</summary>
        [TestMethod]
        public void SavePatientAppointmentTest()
        {
            //IRegistrationDocument registrationDocumentInstance = null;
            //IAppointmentDocument appointmentDocument = null;
            //RegistrationService target;


            //registrationDocumentInstance = MockRepository.GenerateStub<IRegistrationDocument>();
            //appointmentDocument = MockRepository.GenerateStub<IAppointmentDocument>();
            //target = new RegistrationService(registrationDocumentInstance, appointmentDocument);
            //string appointmentUrl="ggg";
            //string returnUrl = "SimApp/" + Guid.NewGuid().ToString();
            //appointmentDocument.Stub(x => x.SaveOrUpdate(appointmentUrl, GetAppointmentData())).Return(returnUrl);

            //string patientGuid = string.Empty;
            //Appointment appointmentData = null;
            //bool expected = false;
            //bool actual;
            //actual = target.SavePatientAppointment(patientGuid, appointmentData);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private Appointment GetAppointmentData()
        {
            Appointment patientAppointment = new Appointment
                                                 {ScheduledTimeStamp = DateTime.Now.AddDays(3), IsRecurrence = false};

            return patientAppointment;
        }

        /// <summary>
        ///A test for GetPatientRegistration
        ///</summary>
        [TestMethod]
        public void GetPatientRegistrationTest()
        {
            //IRegistrationDocument registrationDocumentInstance = null; // TODO: Initialize to an appropriate value
            //IAppointmentDocument appointmentDocument = null; // TODO: Initialize to an appropriate value

            //registrationDocumentInstance= MockRepository.GenerateStub<IRegistrationDocument>();
            //appointmentDocument = MockRepository.GenerateStub<IAppointmentDocument>();
            //RegistrationService target = new RegistrationService(registrationDocumentInstance, appointmentDocument);
            //string patientGuid = string.Empty;
            //registrationDocumentInstance.Stub(x => x.Get(x.GetAssignmentUrl(), patientGuid)).Return(GetPatientRegistrationData());
            
            //Registration expected = null;
            //Registration actual;
            //actual = target.GetPatientRegistration(patientGuid);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private Registration GetPatientRegistrationData()
        {
            Registration patientRegistration = new Registration {FirstName = "Test"};
            return patientRegistration;
        }
    }
}
