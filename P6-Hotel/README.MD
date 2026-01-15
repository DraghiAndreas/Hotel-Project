# Proiect 6 - Sistem pentru gestionarea unui Hotel cu Self Check-in

Aplicatie de consola pentru administrarea unui hotel, scrisa in C# (.NET). Proiectul foloseste o arhitectura stratificata si salveaza datele in fisiere JSON.

```c++
* 3.2.X - Finisaje finale, structura, adaugarea README-ului
* 3.2 -  Adaugat functionalitate, + multiple finisaje atat la cod cat si de interfata
* 3.1 - Am separat MenuService-ul, dat set-up la settere/gettere pt _config
* 3 - UI-ul prezenta prea multe bug-uri, a decis sa reveni la interfata in Consola
// Note : Commiturile 2.X nu mai sunt vizibile datorita faptului ca, la revenire la forma in consola
// s-a folosit un `git push --force`
|
|
* 2.1 - Adaugare functionalitate in UI
* 2 - Adaugat interfata UI
|
|
* 1 - 1.2 Forma initiala a proiectului, cea mai basic, cu functionalitate redusa
// Note : Versiunile initiale au fost concepute IRL
```


## Descriere
Acest proiect simuleaza un sistem de rezervari complet, cu interfete separate pentru **Administratori** si **Clienti**. Am separat logica aplicatiei (`HotelService`) de meniul utilizatorului (`MenuService`) si de salvarea fisierelor (`FileHandler`), pentru a pastra codul curat si modular.

## Functionalitati

### Modul Client
* **Cautare inteligenta**: Poti cauta camere disponibile intr-un anumit interval de timp. Sistemul verifica daca datele se suprapun cu alte rezervari existente.
* **Rezervari**: Crearea rezervarilor cu validari automate (de exemplu, sa nu depasesti numarul maxim de zile permis).
* **Self Check-in / Check-out**: Clientul isi poate marca singur sosirea si plecarea.
* **Recuperare Cont**: Daca uiti parola, te poti loga folosind un **Token unic** primit la inregistrare.

### Modul Administrator
* **Management Camere**: Poti adauga camere noi, le poti modifica pretul sau statusul (de exemplu, daca intra in mentenanta) si le poti sterge.
* **Raportare**: Vezi toate rezervarile din sistem si cati bani a generat fiecare.
* **Configurare**: Poti schimba regulile hotelului (numarul minim sau maxim de zile pentru o rezervare) direct din meniu, iar setarile raman salvate.

## Detalii Tehnice

### Arhitectura
* **HotelService**: Aici se intampla toata "magia" (calcule, validari, logica de rezervare).
* **MenuService**: Se ocupa doar de afisarea textului in consola si citirea comenzilor de la tastatura.
* **FileHandler**: O clasa care salveaza si incarca datele din fisiere JSON (`DataStorage`).
* **Dependency Injection**: Folosim `Microsoft.Extensions.DependencyInjection` pentru a lega serviciile intre ele.

### Entitati Principale
* **User**: Are roluri (`Admin`, `Client`) si un token de siguranta.
* **Room**: Tipul camerei (`Single`, `Double`, `Suite`), pret si disponibilitate.
* **Reservation**: Leaga un utilizator de o camera pentru o perioada de timp.

## Structura Fisierelor
Datele se salveaza automat in folderul `DataStorage` (se creeaza singur la prima rulare):
* `Users.json`: Utilizatori si parole.
* `Rooms.json`: Lista camerelor.
* `Reservations.json`: Istoricul rezervarilor.
* `Config.json`: Setari globale (limite de zile).

## Cum se ruleaza proiectul

1. Deschide solutia in **Visual Studio** sau **Rider**.
2. Da-i un **Build** ca sa se descarce pachetele necesare (NuGet).
3. Ruleaza aplicatia (`Run`).

### Conturi Implicite
Daca e prima data cand rulezi aplicatia si nu ai conturi, sistemul le creeaza automat pe acestea:
* **Admin**: user `admin` / parola `admin`
* **Client**: user `client` / parola `client`
