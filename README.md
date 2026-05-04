Om het te starten, voer je `dotnet run --project backend/src/Assessment.Api` uit vanuit de hoofdmap van `/Assesment` in één terminal, en `npm install` (alleen de eerste keer) en daarna `npm start` vanuit `/Assesment/frontend` in een andere terminal.

Backend solution: `backend/Assessment.sln` (`dotnet build`, `dotnet test`).

Nieuwe EF-migraties (vanuit `backend/`):

`dotnet ef migrations add <Name> -p src/Assessment.Infrastructure -s src/Assessment.Api`
