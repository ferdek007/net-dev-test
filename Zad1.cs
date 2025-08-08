namespace DevTask1
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SuperiorId { get; set; }
        public virtual Employee Superior { get; set; }
    }

    public class EmployeesStructure
    {
        private readonly Dictionary<(int employeeId, int superiorId), int> _relations = new();

        /// <summary>
        /// Adds a superior-employee relationship with the specified level.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when invalid arguments are provided.</exception>
        public void AddRelation(int employeeId, int superiorId, int level)
        {
            if (employeeId <= 0) throw new ArgumentException("Invalid employee ID", nameof(employeeId));
            if (superiorId <= 0) throw new ArgumentException("Invalid superior ID", nameof(superiorId));
            if (level <= 0) throw new ArgumentException("Level must be positive", nameof(level));

            _relations[(employeeId, superiorId)] = level;
        }

        /// <summary>
        /// Gets the hierarchy level between an employee and their superior.
        /// </summary>
        /// <returns>The level if relationship exists, otherwise null.</returns>
        public int? GetSuperiorLevelOfEmployee(int employeeId, int superiorId)
        {
            return _relations.TryGetValue((employeeId, superiorId), out int level) ? level : null;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Jan Kowalski", SuperiorId = null },
                new Employee { Id = 2, Name = "Kamil Nowak", SuperiorId = 1 },
                new Employee { Id = 3, Name = "Anna Mariacka", SuperiorId = 1 },
                new Employee { Id = 4, Name = "Andrzej Abacki", SuperiorId = 2 },
                new Employee { Id = 5, Name = "Marek Kowalczyk", SuperiorId = 3 },
                new Employee { Id = 6, Name = "Tomasz Wiśniewski", SuperiorId = 5 }
            };

            var structure = BuildEmployeeHierarchy(employees);

            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 2, 1)); // 1
            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 4, 3)); // null
            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 4, 1)); // 2
            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 4, 2)); // 1
            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 1, 2)); // null
            Console.WriteLine(GetSuperiorLevelOfEmployee(structure, 3, 5)); // null
        }

        /// <summary>
        /// Builds a hierarchy structure from a list of employees.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when employees list is null.</exception>
        /// <exception cref="ArgumentException">Thrown when duplicate employee IDs exist.</exception>
        public static EmployeesStructure BuildEmployeeHierarchy(List<Employee> employees)
        {
            if (employees == null) throw new ArgumentNullException(nameof(employees));

            var structure = new EmployeesStructure();
            var employeeDict = employees.ToDictionary(employee => employee.Id);

            // Check for duplicate IDs
            if (employeeDict.Count != employees.Count)
                throw new ArgumentException("Duplicate employee IDs detected.");

            var visited = new HashSet<(int, int)>();

            foreach (var employee in employees)
            {
                if (employee.SuperiorId == null) continue;

                var currentEmployee = employee;
                var level = 1;

                while (currentEmployee.SuperiorId != null)
                {
                    var key = (employee.Id, currentEmployee.SuperiorId.Value);

                    if (!visited.Add(key))
                        break; // HashSet.Add returns false if already exists

                    structure.AddRelation(employee.Id, currentEmployee.SuperiorId.Value, level);

                    if (!employeeDict.TryGetValue(currentEmployee.SuperiorId.Value, out currentEmployee))
                        break;

                    level++;
                }
            }

            return structure;
        }

        /// <summary>
        /// Gets the hierarchy level between an employee and their superior.
        /// </summary>
        public static int? GetSuperiorLevelOfEmployee(EmployeesStructure structure, int employeeId, int superiorId)
        {
            return structure?.GetSuperiorLevelOfEmployee(employeeId, superiorId);
        }
    }
}