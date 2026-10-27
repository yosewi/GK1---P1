## Obsługa

Lewy przycisk myszy przeciąga wierzchołek, punkt kontrolny albo cały wielokąt.
Prawy przycisk otwiera menu wierzchołka (usunięcie, ciągłość G0/G1/C1) albo krawędzi (dodanie wierzchołka, Bezier, łuk, długość, prostopadłość, usunięcie ograniczeń).
Wybranie prostopadłości wymaga kliknięcia drugiej krawędzi, do której pierwsza ma być prostopadła.
Esc anuluje wybieranie drugiej krawędzi prostopadłości, F11 włącza pełny ekran.

## Założenia

Ograniczenia mają tylko odcinki, prostopadłość może łączyć krawędzie różnych wielokątów.
Priorytetem jest zawsze to, żeby wierzchołek/punkt kontrolny podążał za kursorem.
Gdy ograniczeń nie da się spełnić, przesuwa się cały wielokąt, lub w wypadku jeśli taki wielokąt w ogóle nie może istnieć, to wyświetla się odpowiedni komunikat przy próbie dodania ograniczenia.

## Model danych

Przestrzeń to lista wielokątów, wielokąt to wierzchołki i krawędzie, a krawędź ma typ (odcinek, Bezier, łuk) i listę ograniczeń.
Wierzchołek zna swoją ciągłość i obie swoje krawędzie, a ograniczenie jest klasą z metodami IsDone (czy jest spełnione) i Fix (popraw krawędź), więc nowe ograniczenie to nowa klasa.

## Algorytm ograniczeń

Od przeciąganego wierzchołka poprawiamy kolejne krawędzie w przód i w tył, aż trafimy na krawędź, która spełnia swoje ograniczenia.
Zatrzymujemy algorytm jak najwcześniej, tzn. w każdą stronę kończymy na pierwszej dobrej krawędzi, a całość powtarzamy tylko dopóki coś się zmienia (najwyżej 1000 razy).
Jeśli się nie uda, przywracamy poprzednie pozycje i przesuwamy cały wielokąt.
Długość poprawiamy, przesuwając dalszy koniec wzdłuż krawędzi, a prostopadłość, obracając krawędź wokół stałego końca.

## Ciągłość

Po każdym ruchu punkty kontrolne Beziera dopasowują się do sąsiedniej krawędzi.

## Rysowanie

Odcinki algorytmem Bresenhama, łuki algorytmem MidpointCircle, uogólnione dla wszystkich 8 kierunków.
Bezier to punkty liczone metodą przyrostową, połączone odcinkami.
