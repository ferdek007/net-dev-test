-- a)
SELECT DISTINCT e.id, e.name
FROM Employee e
INNER JOIN Team t ON e.teamId = t.id AND t.name = '.NET'
INNER JOIN Vacations v ON e.id = v.employeeId
WHERE v.dateSince >= '2019-01-01' AND v.dateUntil <= '2019-12-31';

-- b)
SELECT 
    e.id,
    e.name AS EmployeeName,
    COALESCE(SUM(
        CASE 
            -- Only count vacation days if the vacation started before today
            WHEN CAST(v.dateSince AS DATE) < CAST(GETDATE() AS DATE) THEN
                DATEDIFF(day, 
                    -- Use either the vacation start date or Jan 1 of current year, whichever is later
                    CASE 
                        WHEN v.dateSince < DATEFROMPARTS(YEAR(GETDATE()), 1, 1) 
                        THEN DATEFROMPARTS(YEAR(GETDATE()), 1, 1)  -- Jan 1 of current year
                        ELSE v.dateSince  -- Actual vacation start date
                    END,
                    -- Use either the vacation end date or yesterday's date, whichever is earlier
                    CASE 
                        WHEN v.dateUntil >= CAST(GETDATE() AS DATE) 
                        THEN DATEADD(day, -1, CAST(GETDATE() AS DATE))  -- Yesterday (excludes today)
                        ELSE v.dateUntil  -- Actual vacation end date
                    END
                ) + 1  -- Add 1 to include both start and end dates in count
            ELSE 0  -- Return 0 for vacations that start today or in the future
        END
    ), 0) AS UsedVacationDays
FROM 
    Employee e
LEFT JOIN 
    Vacations v ON e.id = v.employeeId
    -- Join conditions to filter vacations:
    -- 1) Must end in current year or later (has days in current year)
    -- 2) Must have started before today (excludes future vacations)
    -- 3) Must have at least some days in current year (redundant safety check)
    AND (
        v.dateUntil >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)  -- Ends in or after current year
        AND v.dateSince < CAST(GETDATE() AS DATE)            -- Started before today
        AND v.dateUntil >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)  -- Safety: ensure not entirely before current year
    )
GROUP BY 
    e.id, e.name
ORDER BY 
    e.name;

-- c)
SELECT t.id, t.name
FROM Team t
WHERE t.id NOT IN (
    SELECT DISTINCT e.teamId
    FROM Employee e
    INNER JOIN Vacations v ON e.id = v.employeeId
    WHERE v.dateSince >= '2019-01-01' AND v.dateUntil <= '2019-12-31'
);
