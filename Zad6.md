# Sposoby na optymalizację liczby zapytań SQL do metody z zad 3

1. Eager Loading

- Pobieranie wszystkich potrzebnych danych w jednym zapytaniu z użyciem JOIN.

- Przykład: Zamiast osobno pobierać pracownika, jego pakiety urlopowe i wnioski urlopowe, wykonujemy jedno zapytanie SQL z odpowiednimi złączeniami.

2. Batch Fetching

- Grupowanie wielu małych zapytań w jedno większe.

- Przykład: Jeśli metoda jest wywoływana dla wielu pracowników, lepiej pobrać dane wszystkich naraz niż osobno dla każdego.

3. Cache'owanie wyników

- Przechowywanie często używanych danych w pamięci podręcznej.

- Przykład: Pakiety urlopowe mogą być cache'owane, jeśli rzadko się zmieniają.

4. Stored Procedures

- Przeniesienie logiki obliczeniowej do procedury SQL składowanej w bazie danych.

- Przykład: Cała metoda CountFreeDaysForEmployee mogłaby być zaimplementowana jako procedura SQL.
