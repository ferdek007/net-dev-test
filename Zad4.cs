namespace DevTask4
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TeamId { get; set; }
        public int PositionId { get; set; }
        public int VacationPackageId { get; set; }


    }

    public class VacationPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GrantedDays { get; set; }
        public int Year { get; set; }
    }

    public class Vacation
    {
        public int Id { get; set; }
        public DateTime DateSince { get; set; }
        public DateTime DateUntil { get; set; }
        public int NumberOfHours { get; set; }
        public int IsPartialVacation { get; set; }
        public int EmployeeId { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Example data
            var employee = new Employee { Id = 1, Name = "Jan Kowalski" };
            var currentYear = DateTime.Now.Year;

            // Case 1: Employee with available vacation days
            var package1 = new VacationPackage { Id = 1, Name = "Standard Package", GrantedDays = 20, Year = currentYear };
            var vacations1 = new List<Vacation>
            {
                new Vacation
                    {
                    EmployeeId = 1,
                    DateSince = new DateTime(currentYear, 1, 10), // Wednesday
                    DateUntil = new DateTime(currentYear, 1, 14)  // Friday (3 working days)
                    }
            };

            // Case 2: Employee with no remaining days
            var package2 = new VacationPackage { Id = 2, Name = "Premium Package", GrantedDays = 5, Year = currentYear };
            var vacations2 = new List<Vacation>
            {
                new Vacation
                {
                    EmployeeId = 1,
                    DateSince = new DateTime(currentYear, 6, 1), // Thursday
                    DateUntil = new DateTime(currentYear, 6, 9)  // Friday next week (6 working days)
                }
            };

            // Case 3: Employee with no vacations taken
            var package3 = new VacationPackage { Id = 3, Name = "Executive Package", GrantedDays = 15, Year = currentYear };

            // Case 4: Vacation spanning year boundary
            var package4 = new VacationPackage { Id = 4, Name = "Custom Package", GrantedDays = 30, Year = currentYear };
            var vacations4 = new List<Vacation>
            {
                new Vacation
                {
                    EmployeeId = 1,
                    DateSince = new DateTime(currentYear-1, 12, 28), // Last year
                    DateUntil = new DateTime(currentYear, 1, 5)      // New year (3 working days in current year)
                }
            };

            try
            {
                Console.WriteLine("Case 1: Should be True (has remaining days)");
                Console.WriteLine(IfEmployeeCanRequestVacation(employee, vacations1, package1));

                Console.WriteLine("\nCase 2: Should be False (no remaining days)");
                Console.WriteLine(IfEmployeeCanRequestVacation(employee, vacations2, package2));

                Console.WriteLine("\nCase 3: Should be True (no vacations taken)");
                Console.WriteLine(IfEmployeeCanRequestVacation(employee, null, package3));

                Console.WriteLine("\nCase 4: Should be True (year-spanning vacation)");
                Console.WriteLine(IfEmployeeCanRequestVacation(employee, vacations4, package4));

                Console.WriteLine("\nCase 5: Should throw (null employee)");
                Console.WriteLine(IfEmployeeCanRequestVacation(null, vacations1, package1));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name} - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if an employee can request vacation based on remaining vacation days.
        /// </summary>
        /// <param name="employee">The employee requesting vacation.</param>
        /// <param name="vacations">List of the employee's vacations (null means no vacations taken).</param>
        /// <param name="vacationPackage">The employee's vacation package.</param>
        /// <returns>True if employee can request vacation (has remaining days), false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if employee or vacationPackage is null.</exception>
        /// <exception cref="ArgumentException">Thrown if DateUntil is earlier than DateSince or vacationPackage.GrantedDays is negative.</exception>

        public static bool IfEmployeeCanRequestVacation(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage)
        {
            ArgumentNullException.ThrowIfNull(employee);
            ArgumentNullException.ThrowIfNull(vacationPackage);

            if (vacationPackage.GrantedDays < 0)
                throw new ArgumentException("Granted days cannot be negative.");

            if (vacationPackage.GrantedDays == 0)
                return false;

            if (vacations == null)
                return true;

            var employeeVacations = vacations
                .Where(vacation => vacation.EmployeeId == employee.Id)
                .ToList();

            int usedDays = 0;
            int currentVacationPackageYear = vacationPackage.Year;

            foreach (var vacation in employeeVacations)
            {
                if (vacation.DateUntil.Year < currentVacationPackageYear || vacation.DateSince.Year > currentVacationPackageYear)
                    continue;

                DateTime startDate = vacation.DateSince.Year < currentVacationPackageYear
                    ? new DateTime(currentVacationPackageYear, 1, 1)
                    : vacation.DateSince;

                DateTime endDate = vacation.DateUntil.Year > currentVacationPackageYear
                    ? new DateTime(currentVacationPackageYear, 12, 31)
                    : vacation.DateUntil;

                if (endDate < startDate)
                    throw new ArgumentException($"Invalid vacation dates: {vacation.DateSince} - {vacation.DateUntil}");

                // Exclude weekends
                int workingDaysCount = 0;
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        workingDaysCount++;
                }
                usedDays += workingDaysCount;
            }

            int remainingDays = vacationPackage.GrantedDays - usedDays;

            return remainingDays > 0;
        }
    }
}
