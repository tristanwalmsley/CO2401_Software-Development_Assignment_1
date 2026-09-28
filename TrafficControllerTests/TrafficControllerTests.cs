using NUnit.Framework;
using NSubstitute;
using System;
using System.Xml;
using SmartTrafficController;


namespace TrafficControllerTests
{
    [TestFixture]
    public class TrafficControllerTests
    {
        // L1R1
        [TestCase("ABC123", "abc123")]
        [TestCase("abc123", "abc123")]
        [TestCase("ABCD",   "abcd"  )]
        [TestCase("abcd",   "abcd"  )]
        [TestCase("Abcd",   "abcd"  )]
        [TestCase("A",      "a"     )]
        [TestCase("a",      "a"     )]
        public void Constructor_ValidID_GivenIntersectionID_SetsLowercaseIntersectionID(string inputID, string expectedID)
        {
            // Arrange
            TrafficController controller = new TrafficController(inputID);

            // Act
            string result = controller.GetIntersectionID();

            // Assert
            Assert.That(result, Is.EqualTo(expectedID));
        }

        // L1R2
        [Test]
        public void Constructor_DefaultVehicleState_IsAmber()
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            string result = controller.GetCurrentVehicleSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("amber"));
        }

        // L1R2
        [Test]
        public void Constructor_DefaultPedestrianState_IsWait()
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            string result = controller.GetCurrentPedestrianSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("wait"));
        }

        // L1R3
        [TestCase("abc123")]
        [TestCase("abcd"  )]
        [TestCase("a"     )]
        public void GetIntersectionID_AfterConstruction_ReturnsCorrectIntersectionID(string inputID)
        {
            // Arrange
            TrafficController controller = new TrafficController(inputID);
            //controller.SetIntersectionID(inputID);

            // Act
            string result = controller.GetIntersectionID();

            // Assert
            Assert.That(result, Is.EqualTo(inputID));
        }

        // L1R3
        [TestCase("abc123", "testing")]
        [TestCase("abcd",   "testing")]
        [TestCase("a",      "testing")]
        public void GetIntersectionID_AfterSetIntersectionID_ReturnsUpdatedIntersectionID(string inputID, string updatedID)
        {
            // Arrange
            TrafficController controller = new TrafficController(inputID);
            controller.SetIntersectionID(updatedID);

            // Act
            string result = controller.GetIntersectionID();

            // Assert
            Assert.That(result, Is.EqualTo(updatedID));
        }

        // L1R4
        [Test]
        public void GetCurrentPedestrianSignalState_DefaultConstructor_ReturnsWait()
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            string result = controller.GetCurrentPedestrianSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("wait"));
        }

        // L1R4
        [Test]
        public void GetCurrentVehicleSignalState_DefaultConstructor_ReturnsAmber()
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            string result = controller.GetCurrentVehicleSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("amber"));
        }

        // L1R5
        [TestCase("red",      "wait")]
        [TestCase("red",      "walk")]
        [TestCase("redamber", "wait")]
        [TestCase("redamber", "walk")]
        [TestCase("green",    "wait")]
        [TestCase("green",    "walk")]
        [TestCase("amber",    "wait")]
        [TestCase("amber",    "walk")]
        [TestCase("oosv",     "oosp")]
        public void SetStateDirect_GivenValidStates_ReturnsTrue(string inputVehicleState, string inputPedestrianState)
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            bool result = controller.SetStateDirect(inputVehicleState, inputPedestrianState);

            // Assert
            Assert.That(result, Is.True);
        }

        // L1R5
        [TestCase("Red",      "Wait")]
        [TestCase("RED",      "Walk")]
        [TestCase("Redamber", "WAIT")]
        [TestCase("REDAMBER", "WALK")]
        [TestCase("green",    "Wait")]
        [TestCase("GREEN",    "Walk")]
        [TestCase("Amber",    "WAIT")]
        [TestCase("AMBER",    "WALK")]
        [TestCase("OOSV",     "OOSP")]
        public void SetStateDirect_GivenUppercaseValidStates_ReturnsTrue(string inputVehicleState, string inputPedestrianState)
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            bool result = controller.SetStateDirect(inputVehicleState, inputPedestrianState);

            // Assert
            Assert.That(result, Is.True);
        }

        // L1R5
        [TestCase("yell", "wait")] // Invalid vehicle state
        [TestCase("red",  "wabh")] // Invalid pedestrian state
        [TestCase("Amng", "Cjnf")] // Both invalid states
        [TestCase("1234", "5678")]
        [TestCase(" ",    " "   )]
        public void SetStateDirect_GivenInvalidStates_ReturnsFalse(string inputVehicleState, string inputPedestrianState)
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123");

            // Act
            bool result = controller.SetStateDirect(inputVehicleState, inputPedestrianState);

            // Assert
            Assert.That(result, Is.False);
        }

        // L2R1
        [TestCase("green",    "amber",    "walk", "wait")]
        [TestCase("amber",    "red",      "wait", "walk")]
        [TestCase("red",      "redamber", "walk", "wait")]
        [TestCase("redamber", "green",    "walk", "wait")]
        [TestCase("green",    "oosv",     "walk", "oosp")]
        [TestCase("amber",    "oosv",     "wait", "oosp")]
        [TestCase("red",      "oosv",     "walk", "oosp")]
        [TestCase("redamber", "oosv",     "walk", "oosp")]
        public void SetCurrentState_GivenValidStatesInTransition_ReturnsTrue(string initialVehicleState, string inputVehicleState, string initialPedestrianState, string inputPedestrianState)
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123", initialVehicleState, initialPedestrianState);

            // Act
            bool result = controller.SetCurrentState(inputVehicleState, inputPedestrianState);

            // Assert
            Assert.That(result, Is.True);
        }

        // L2R1
        [TestCase("green",    "amber",    "wait", "walk")]
        [TestCase("green",    "red",      "wait", "walk")]
        [TestCase("green",    "redamber", "wait", "walk")]
        [TestCase("green",    "red",      "walk", "wait")]
        [TestCase("green",    "redamber", "walk", "wait")]
        [TestCase("redamber", "amber",    "wait", "walk")]
        [TestCase("redamber", "red",      "wait", "walk")]
        [TestCase("redamber", "green",    "wait", "walk")]
        [TestCase("redamber", "amber",    "walk", "wait")]
        [TestCase("redamber", "red",      "walk", "wait")]
        [TestCase("amber",    "green",    "wait", "walk")]
        [TestCase("amber",    "redamber", "wait", "walk")]
        [TestCase("amber",    "green",    "walk", "wait")]
        [TestCase("amber",    "red",      "walk", "wait")]
        [TestCase("amber",    "redamber", "walk", "wait")]
        [TestCase("red",      "green",    "wait", "walk")]
        [TestCase("red",      "amber",    "wait", "walk")]
        [TestCase("red",      "redamber", "wait", "walk")]
        [TestCase("red",      "green",    "walk", "wait")]
        [TestCase("red",      "amber",    "walk", "wait")]
        public void SetCurrentState_GivenValidStatesInInvalidTransition_ReturnsFalse(string initialVehicleState, string inputVehicleState, string initialPedestrianState, string inputPedestrianState)
        {
            // Arrange
            TrafficController controller = new TrafficController("abc123", initialVehicleState, initialPedestrianState);

            // Act
            bool result = controller.SetCurrentState(inputVehicleState, inputPedestrianState);

            // Assert
            Assert.That(result, Is.False);
        }

        // L2R2
        [TestCase("green", "wait")]
        [TestCase("red", "walk")]
        [TestCase("redamber", "wait")]
        [TestCase("amber", "wait")]
        [TestCase("redamber", "walk")]
        [TestCase("RED", "walk")]
        public void Constructor_GivenValidVehiclePedestrianStates_DoesNotThrowException(string inputVehicleState, string inputPedestrianState)
        {
            Assert.DoesNotThrow(() =>
            {
                new TrafficController("abc123", inputVehicleState, inputPedestrianState);
            });
        }

        // L2R2
        [TestCase("sdabjoah", "wait")]
        [TestCase("walK", "green")]
        public void Constructor_GivenValidVehiclePedestrianStates_DoesThrowException(string inputVehicleState, string inputPedestrianState)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new TrafficController("abc123", inputVehicleState, inputPedestrianState);
            });
        }

        // L2R3
        [Test]
        public void Constructor_WithDependencies_InitialisesManagers()
        {
            // Arrange
            IVehicleSignalManager    vehicleManager    = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager             timeManager       = Substitute.For<ITimeManager>();
            IWebService              webService        = Substitute.For<IWebService>();
            IEmailService            emailService      = Substitute.For<IEmailService>();

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            // Assert
            Assert.That(controller, Is.Not.Null);
        }

        // L2R4
        [Test]
        public void GetStatusReport_ManagerGetStatusMethods_ReturnsStatuses()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns("OK,");
            pedestrianManager.GetStatus().Returns("OK,");
            timeManager.GetStatus().Returns("OK,");

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            string statusReport = controller.GetStatusReport();

            // Assert
            Assert.That(statusReport, Is.EqualTo("OK,OK,OK,"));
        }

        // L3R1
        [Test]
        public void SetCurrentSate_RedToAmberVehicleStateOnSuccess_ReturnsTrue()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.SetAllRed(true).Returns(true);
            pedestrianManager.SetWalk(true).Returns(true);
            pedestrianManager.SetAudible(true).Returns(true);
            timeManager.Delay(3).Returns(true);
            timeManager.Delay(60).Returns(true);

            // Act
            TrafficController controller = new TrafficController("abc123", "amber", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            bool result = controller.SetCurrentState("red", "walk");

            // Assert
            Assert.That(result, Is.True);

        }

        // L3R1
        [TestCase(false, true, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        [TestCase(false, false, true, true)]
        [TestCase(true, false, false, true)]
        [TestCase(true, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, false)]
        public void SetCurrentSate_RedToAmberVehicleStateOnFailure_ReturnsFalse(bool setAllRedReturnVal, bool setWalkReturnVal, bool setAudibleReturnVal, bool delayReturnVal)
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.SetAllRed(true).Returns(setAllRedReturnVal);
            pedestrianManager.SetWalk(true).Returns(setWalkReturnVal);
            pedestrianManager.SetAudible(true).Returns(setAudibleReturnVal);
            timeManager.Delay(3).Returns(delayReturnVal);
            timeManager.Delay(60).Returns(delayReturnVal);

            // Act
            TrafficController controller = new TrafficController("abc123", "amber", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            bool result = controller.SetCurrentState("red", "walk");

            // Assert
            Assert.That(result, Is.False);

        }

        // L3R1
        [Test]
        public void SetCurrentSate_RedToAmberVehicleState_VehicleStateChangesToRedamber()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            timeManager.Delay(3).Returns(true);
            vehicleManager.SetAllRed(true).Returns(true);
            pedestrianManager.SetWalk(true).Returns(true);
            pedestrianManager.SetAudible(true).Returns(true);
            timeManager.Delay(60).Returns(true);

            // Act
            TrafficController controller = new TrafficController("abc123", "amber", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            controller.SetCurrentState("red", "walk");
            string result = controller.GetCurrentVehicleSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("redamber"));

        }

        // L3R2
        [Test]
        public void SetCurrentSate_RedamberToGreenVehicleStateOnSuccess_ReturnsTrue()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            timeManager.Delay(3).Returns(true);
            pedestrianManager.SetWalk(false).Returns(true);
            pedestrianManager.SetWait(true).Returns(true);
            pedestrianManager.SetAudible(false).Returns(true);
            vehicleManager.SetAllGreen(true).Returns(true);
            timeManager.Delay(120).Returns(true);

            // Act
            TrafficController controller = new TrafficController("abc123", "redamber", "walk", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            bool result = controller.SetCurrentState("green", "wait");

            // Assert
            Assert.That(result, Is.True);

        }

        // L3R2
        [TestCase(false, true, true, true, false)]
        [TestCase(true, false, true, true, true)]
        [TestCase(true, true, false, true, false)]
        [TestCase(true, true, true, false, true)]
        [TestCase(false, false, true, true, true)]
        [TestCase(true, false, false, true, false)]
        [TestCase(true, true, false, false, true)]
        [TestCase(false, true, true, false, false)]
        [TestCase(false, false, false, true, false)]
        [TestCase(true, false, false, false, false)]
        [TestCase(false, true, false, false, true)]
        [TestCase(false, false, true, false, true)]
        [TestCase(false, false, false, false, false)]
        public void SetCurrentSate_RedamberToGreenVehicleStateOnFailure_ReturnsFalse(bool setWalkReturnVal, bool setWaitReturnVal, bool setAudibleReturnVal, bool setAllGreenReturnVal, bool delayReturnVal)
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            timeManager.Delay(3).Returns(delayReturnVal);
            pedestrianManager.SetWalk(false).Returns(setWalkReturnVal);
            pedestrianManager.SetWait(true).Returns(setWaitReturnVal);
            pedestrianManager.SetAudible(false).Returns(setAudibleReturnVal);
            vehicleManager.SetAllGreen(true).Returns(setAllGreenReturnVal);
            timeManager.Delay(120).Returns(delayReturnVal);

            // Act
            TrafficController controller = new TrafficController("abc123", "redamber", "walk", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            bool result = controller.SetCurrentState("green", "wait");

            // Assert
            Assert.That(result, Is.False);

        }

        // L3R2
        [Test]
        public void SetCurrentSate_RedamberToGreenVehicleState_VehicleStateChangesToRedamber()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            timeManager.Delay(3).Returns(true);
            pedestrianManager.SetWalk(false).Returns(true);
            pedestrianManager.SetWait(true).Returns(true);
            pedestrianManager.SetAudible(false).Returns(true);
            vehicleManager.SetAllGreen(true).Returns(true);
            timeManager.Delay(120).Returns(true);

            // Act
            TrafficController controller = new TrafficController("abc123", "redamber", "walk", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            controller.SetCurrentState("green", "wait");
            string result = controller.GetCurrentVehicleSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("amber"));

        }

        // L3R3
        [TestCase("FAULT,", "OK,", "OK,")]
        [TestCase("OK,", "FAULT,", "OK,")]
        [TestCase("OK,", "OK,", "FAULT,")]
        [TestCase("FAULT,", "FAULT,", "OK,")]
        [TestCase("FAULT,", "OK,", "FAULT,")]
        [TestCase("OK,", "FAULT,", "FAULT,")]
        [TestCase("FAULT,", "FAULT,", "FAULT,")]
        [TestCase("OK,FAULT,", "OK,", "OK,")]
        [TestCase("FAULT,OK,", "OK,", "OK,FAULT,OK,")]
        [TestCase("FAULT,OK,FAULT", "OK,", "OK,")]
        public void GetStatusReport_FaultDetected_PedestrianSignalSetToOosp(string vehicleStatuses, string pedestrianStatuses, string timeStatuses)
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns(vehicleStatuses);
            pedestrianManager.GetStatus().Returns(pedestrianStatuses);
            timeManager.GetStatus().Returns(timeStatuses);

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            controller.GetStatusReport();

            string result = controller.GetCurrentPedestrianSignalState();

            // Assert
            Assert.That(result, Is.EqualTo("oosp"));

        }

        // L3R3
        [Test]
        public void GetStatusReport_FaultDetected_LogEngineerRequiredCalled()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns("FAULT,");
            pedestrianManager.GetStatus().Returns("FAULT,");
            timeManager.GetStatus().Returns("FAULT,");

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            controller.GetStatusReport();

            // Assert
            webService.Received().LogEngineerRequired("out of service");

        }

        // L3R4
        [TestCase("FAULT,", "OK,", "OK,", "VehicleSignal,")]
        [TestCase("OK,", "FAULT,", "OK,", "PedestrianSignal,")]
        [TestCase("OK,", "OK,", "FAULT,", "Timer,")]
        [TestCase("FAULT,", "FAULT,", "OK,", "VehicleSignal,PedestrianSignal,")]
        [TestCase("OK,", "FAULT,", "FAULT,", "PedestrianSignal,Timer,")]
        [TestCase("FAULT,", "OK,", "FAULT,", "VehicleSignal,Timer,")]
        [TestCase("FAULT,", "FAULT,", "FAULT,", "VehicleSignal,PedestrianSignal,Timer,")]
        public void GetStatusReport_FaultDetected_LogEngineerRequiredCorrectParamaters(string vehicleStatus, string pedestrianStatus, string timeStatus, string logParamater)
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns(vehicleStatus);
            pedestrianManager.GetStatus().Returns(pedestrianStatus);
            timeManager.GetStatus().Returns(timeStatus);

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            controller.GetStatusReport();

            // Assert
            webService.Received().LogEngineerRequired(logParamater);

        }

        // L3R5
        [Test]
        public void GetStatusReport_LogEngineerRequiredThrowsException_DoesNotThrow()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns("FAULT,");
            pedestrianManager.GetStatus().Returns("FAULT,");
            timeManager.GetStatus().Returns("FAULT,");

            webService.When(param => param.LogEngineerRequired("out of service")).Do(param =>
            {
                throw new ArgumentException();
            });

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            // Assert
            Assert.DoesNotThrow(() => controller.GetStatusReport());

        }

        // L3R5
        [Test]
        public void GetStatusReport_LogEngineerRequiredThrowsException_EmailSent()
        {
            // Arrange
            IVehicleSignalManager vehicleManager = Substitute.For<IVehicleSignalManager>();
            IPedestrianSignalManager pedestrianManager = Substitute.For<IPedestrianSignalManager>();
            ITimeManager timeManager = Substitute.For<ITimeManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();

            vehicleManager.GetStatus().Returns("FAULT,");
            pedestrianManager.GetStatus().Returns("FAULT,");
            timeManager.GetStatus().Returns("FAULT,");
            emailService.SendMail("transportoffice@gmail.com", "failed to log out of service", "test exception message");

            webService.When(param => param.LogEngineerRequired("out of service")).Do(param =>
            {
                throw new ArgumentException("test exception message");
            });

            // Act
            TrafficController controller = new TrafficController("abc123", "green", "wait", vehicleManager, pedestrianManager,
                                  timeManager, webService, emailService);

            // Assert
            emailService.Received().SendMail("transportoffice@gmail.com", "failed to log out of service", "test exception message");

        }


    }

}
