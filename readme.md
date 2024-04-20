# Microservices kubernetes shop

## Opis projektu

Projekt `microservices-kubernetes-shop` to aplikacja składająca się z serwisów mikroserwisowych zbudowanych z użyciem technologii .NET, Docker i Kubernetes. Celem projektu jest zapewnienie skalowalnej i elastycznej architektury, która umożliwia łatwe zarządzanie poszczególnymi komponentami aplikacji e-commerce.

## Spis treści

- [Wykorzystane technologie](#wykorzystane-technologie)
- [Baza danych](#baza-danych)
- [Autoryzacja](#autoryzacja)
- [Struktura plików](#struktura-plików)
- [Opis serwisów](#opis-serwisów) #TODO DODAĆ DIAGRAM
  - [Api Gateway](#api-gateway)
  - [Product Api](#product-api)
  - [User Api](#user-api)
  - [Order Api](#order-api)
  - [Rating Api](#rating-api)
- [Kubernetes](#kubernetes)
  - [Deployments](#deployments) #TODO
  - [Services](#services) #TODO
  - [Load Balancer](#load-balancer) #TODO
  - [ConfigMaps](#configmaps) #TODO
- [Docker](#docker) #TODO
- [Health checks](#health-checks) #TODO
- [Zaimplementowane funkcjonalności](#zaimplementowane-funkcjonalności)

## Wykorzystane technologie

- [.NET 8](https://dotnet.microsoft.com/download/dotnet)
- [Docker](https://www.docker.com/)
- [Kubernetes](https://kubernetes.io/)
- [YARP](https://microsoft.github.io/reverse-proxy)
- [MongoDb Atlas](https://www.mongodb.com/cloud/atlas)

## Baza danych

Projekt wykorzystuje bazę danych MongoDB do przechowywania danych. Baza danych jest hostowana w chmurze na platformie MongoDB Atlas. W celu połączenia się z bazą danych, należy skonfigurować odpowiednie parametry połączenia w pliku `appsettings.json` każdego serwisu.

```json
{
  "MongoSettings": {
    "ConnectionString": "mongodb+srv://<username>:<password>@<cluster-url>/<database>?retryWrites=true&w=majority",
    "Database": "MicroservicesApp"
  }
}
```

## Autoryzacja

Projekt wykorzystuje autoryzację JWT do uwierzytelniania użytkowników. Aby uzyskać dostęp do chronionych zasobów, należy przekazać token JWT w nagłówku `Authorization` zgodnie z poniższym schematem:

`Authorization: Bearer [token]`

## Struktura plików

```plaintext
├── ShopMicroservices.sln <-- plik solucji
├── docker-compose.yml <-- plik konfiguracyjny Docker Compose
├── kubernetes <-- folder z plikami konfiguracyjnymi Kubernetes
├── readme.md <-- plik z opisem projektu
├── requests <-- folder z przykładowymi zapytaniami HTTP
└── src
    ├── Api.Gateway <-- serwis bramki API
    ├── Order.Api <-- serwis zamówień
    ├── Product.Api <-- serwis produktów
    ├── Rating.Api <-- serwis ocen
    └── User.Api <-- serwis użytkowników
```

## Opis serwisów

Projekt składa się z kilku serwisów, z których każdy odpowiada za określone zadanie. Poniżej znajduje się lista serwisów wraz z opisem ich funkcjonalności.

### Api Gateway

Bramka API jest punktem wejścia do aplikacji i zarządza ruchem sieciowym między klientem a serwisami mikroserwisów. Odpowiada za przekierowywanie żądań do odpowiednich serwisów oraz agregację odpowiedzi.

Przekierowanie żądań do odpowiednich serwisów odbywa się na podstawie ścieżki URL oraz metody HTTP. Bramka API obsługuje również uwierzytelnianie i autoryzację użytkowników.

Aktualnie bramka API obsługuje następujące ścieżki:

- `/api/products` - serwis produktów
- `/api/users` - serwis użytkowników
- `/api/orders` - serwis zamówień
- `/api/ratings` - serwis ocen

Aby wykonać zapytanie do wskazanego serwisu należy użyć odpowiedniej ścieżki URL oraz metody HTTP według poniższego schematu:

`[ŻĄDANIE] [adres-bramki]/api/[nazwa-serwisu]/[ścieżka-do-endpointu]`

Przykład:

`GET [adres-bramki]/api/products/search`

Bramka wykorzystuje pakiet nuget `Yarp.ReverseProxy` do przekierowywania żądań do odpowiednich serwisów.

Konfiguracja bramki API znajduje się w pliku `appsettings.json`.

### Product Api

Serwis produktów odpowiada za zarządzanie produktami w sklepie internetowym. Umożliwia dodawanie, usuwanie, edytowanie oraz przeglądanie produktów.

Endpointy serwisu produktów:

- `GET /` - zwraca listę wszystkich produktów
- `POST /` - dodaje nowy produkt
- `PUT /` - aktualizuje produkt
- `DELETE /` - usuwa produkt
- `GET /search` - wyszukuje produkty po wskazanych kryteriach

### User Api

Serwis użytkowników odpowiada za zarządzanie użytkownikami w sklepie internetowym. Umożliwia rejestrację, logowanie oraz wysyłanie powiadomienia do użytkownika.

Endpointy serwisu użytkowników:

- `POST /register` - rejestracja nowego użytkownika
- `POST /login` - logowanie użytkownika
- `POST /notify` - wysyłanie powiadomienia do użytkownika

### Order Api

Serwis zamówień odpowiada za zarządzanie zamówieniami w sklepie internetowym. Umożliwia składanie nowych zamówień, przeglądanie zamówień oraz aktualizację statusu zamówienia.

Endpointy serwisu zamówień:

- `POST /cart/{productCode}` - dodaje produkt do koszyka
- `DELETE /cart/{productCode}` - usuwa produkt z koszyka
- `POST /cart/submit` - składa zamówienie
- `POST /cart/cancel` - anuluje zamówienie
- `GET /cart` - zwraca listę wszystkich zamówień dla zalogowanego użytkownika
- `GET /cart/{orderId}` - zwraca szczegóły zamówienia

### Rating Api

Serwis ocen odpowiada za zarządzanie ocenami produktów w sklepie internetowym. Umożliwia dodawanie nowych ocen, przeglądanie ocen oraz obliczanie średniej oceny produktu.

Endpointy serwisu ocen:

- `POST /{productCode}/review` - dodaje nową ocenę produktu
- `DELETE /{productCode}/review` - usuwa ocenę produktu
- `GET /{productCode}/review` - zwraca listę wszystkich ocen produktu

## Kubernetes

### Deployments

### Services

### Load Balancer

### ConfigMaps

## Docker

## Health checks

## Zaimplementowane funkcjonalności

1. [x] Stworzenie podstawowego mikroserwisu
2. [x] Utrzymywanie danych i buforowanie
3. [x] Integracja z usługą odkrywania serwisów
4. [x] Konfiguracja bramy API
5. [x] Komunikacja międzyserwisowa
6. [x] Konteneryzacja mikroserwisów
7. [x] Centralizowana konfiguracja
8. [x] Uwierzytelnianie i autoryzacja
9. [x] Odporność i tolerancja na błędy
10. [ ] Śledzenie rozproszone
11. [x] Architektura sterowania zdarzeniami
12. [x] Bilansowanie obciążenia
13. [ ] Testowanie mikroserwisów
14. [x] Monitorowanie i logowanie
15. [ ] Wersjonowanie API
16. [x] Wdrażanie i skalowanie mikroserwisów
17. [x] Najlepsze praktyki bezpieczeństwa
