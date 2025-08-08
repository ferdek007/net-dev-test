namespace DevTask2
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

    internal class Program
    {

        static void Main(string[] args)
        {
            // Example data
            var currentYear = DateTime.Now.Year;

            var vacationPackage = new VacationPackage
            {
                Id = 1,
                Name = "Standard Package",
                GrantedDays = 26,
                Year = currentYear
            };


            var employee = new Employee
            {
                Id = 1,
                Name = "Jan Kowalski",
                TeamId = 1,
                PositionId = 1,
                VacationPackageId = 1,
            };

            var vacations = new List<Vacation>
            {
                new Vacation // Vacation in the current year (5 working days)
                {
                    Id = 1,
                    DateSince = new DateTime(currentYear, 6, 1),
                    DateUntil = new DateTime(currentYear, 6, 7), // 1-7 June (weekend 4-5 June)
                    EmployeeId = 1,
                    IsPartialVacation = 0
                },
                new Vacation // Vacation starting in the previous year
                {
                    Id = 2,
                    DateSince = new DateTime(currentYear - 1, 12, 29),
                    DateUntil = new DateTime(currentYear, 1, 2), // 29 Dec - 2 Jan (only 2 working days in current year)
                    EmployeeId = 1,
                    IsPartialVacation = 0
                },
                new Vacation // Vacation ending in the next year
                {
                    Id = 3,
                    DateSince = new DateTime(currentYear, 12, 27),
                    DateUntil = new DateTime(currentYear + 1, 1, 3), // In current year: 27-31 Dec (3 working days)
                    EmployeeId = 1,
                    IsPartialVacation = 0
                }
            };

            int remainingDays = CountFreeDaysForEmployee(employee, vacations, vacationPackage);

            Console.WriteLine($"Pracownik {employee.Name} ma jeszcze {remainingDays} dni urlopu do wykorzystania w {currentYear} roku.");
            Console.WriteLine($"Użyte dni: {vacationPackage.GrantedDays - remainingDays}/{vacationPackage.GrantedDays}");
        }

        /// <summary>
        /// Calculates remaining vacation days for an employee in the current year.
        /// - Only counts working days (excludes weekends).
        /// </summary>
        /// <param name="employee">The employee to calculate days for</param>
        /// <param name="vacations">List of vacations (returns full days if null)</param>
        /// <param name="vacationPackage">The vacation package with granted days</param>
        /// <returns>
        /// Number of remaining vacation days. 
        /// Returns vacationPackage.GrantedDays if vacations is null (no vacations taken).
        /// Returns 0 if remaining days would be negative.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if employee or vacationPackage is null.</exception>
        /// <exception cref="ArgumentException">Thrown if DateUntil is earlier than DateSince or vacationPackage.GrantedDays is negative.</exception> 
        public static int CountFreeDaysForEmployee(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage)
        {
            ArgumentNullException.ThrowIfNull(employee);
            ArgumentNullException.ThrowIfNull(vacationPackage);

            if (vacationPackage.GrantedDays < 0)
                throw new ArgumentException("Granted days cannot be negative.");

            if (vacations == null)
                return vacationPackage.GrantedDays; // No vacations taken

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

            return Math.Max(0, vacationPackage.GrantedDays - usedDays);
        }
    }
}
