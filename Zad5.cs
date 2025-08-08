using Microsoft.VisualStudio.TestPlatform.TestHost;
using DevTask4;

namespace Tests.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty (no setup required for these tests)
        }

        [Test]
        public void employee_can_request_vacation()
        {
            // Arrange
            var employee = new Employee { Id = 1 };
            var vacationPackage = new VacationPackage
            {
                Year = DateTime.Now.Year,
                GrantedDays = 20
            };
            var vacations = new List<Vacation>();

            // Act
            var result = DevTask4.Program.IfEmployeeCanRequestVacation(employee, vacations, vacationPackage);

            // Assert
            Assert.That(result, Is.True, "Method should return true when employee has remaining granted vacation days.");
        }

        [Test]
        public void employee_cant_request_vacation()
        {
            // Arrange
            var employee = new Employee { Id = 1 };
            var vacationPackage = new VacationPackage
            {
                Year = DateTime.Now.Year,
                GrantedDays = 5
            };

            var vacations = new List<Vacation>
            {
                new Vacation
                {
                    EmployeeId = 1,
                    DateSince = DateTime.Today.AddDays(-10),
                    DateUntil = DateTime.Today
                }
            };

            // Act
            var result = DevTask4.Program.IfEmployeeCanRequestVacation(employee, vacations, vacationPackage);

            // Assert
            Assert.That(result, Is.False, "Method should return false when granted vacation days are exhausted.");
        }
    }
}