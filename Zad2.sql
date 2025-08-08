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
            -- Oblicz dni tylko jeśli urlop nie zaczyna się dzisiaj
            WHEN CAST(v.dateSince AS DATE) < CAST(GETDATE() AS DATE) THEN
                DATEDIFF(day, 
                    -- Data rozpoczęcia: późniejsza z (dateSince, początek roku)
                    CASE 
                        WHEN v.dateSince < DATEFROMPARTS(YEAR(GETDATE()), 1, 1) 
                        THEN DATEFROMPARTS(YEAR(GETDATE()), 1, 1) 
                        ELSE v.dateSince 
                    END,
                    -- Data zakończenia: wcześniejsza z (dateUntil, wczoraj)
                    CASE 
                        WHEN v.dateUntil >= CAST(GETDATE() AS DATE) 
                        THEN DATEADD(day, -1, CAST(GETDATE() AS DATE))  -- Wczoraj (nie liczymy dzisiaj)
                        ELSE v.dateUntil 
                    END
                ) + 1  -- +1 aby wliczyć obie skrajne daty
            ELSE 0  -- Pomijaj urlopy zaczynające się dzisiaj
        END
    ), 0) AS UsedVacationDays
FROM 
    Employee e
LEFT JOIN 
    Vacations v ON e.id = v.employeeId
    -- Warunki:
    -- 1) Urlopy, które mają jakąkolwiek część w bieżącym roku
    -- 2) Urlopy, które już się rozpoczęły (przed dzisiaj)
    -- 3) Urlopy, które mają przynajmniej 1 dzień w przeszłości
    AND (
        v.dateUntil >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)  -- Koniec urlopu w bieżącym roku lub później
        AND v.dateSince < CAST(GETDATE() AS DATE)            -- Początek urlopu przed dzisiaj (wykluczamy dzisiejsze)
        AND v.dateUntil >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)  -- Zabezpieczenie przed urlopami sprzed roku
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
