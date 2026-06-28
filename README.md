# T4

ASP.NET Core Web API solution with:

- `GET /Open/Echo1?name={name}` returning `Hello {name}`
- `GET /Closed/Echo2?name={name}` returning `Good to see you again {name}` for authenticated callers

The protected endpoint uses JWT bearer authentication for Microsoft Entra-issued access tokens. Before using it, replace `Authentication:Audience` in `/home/runner/work/T4/T4/src/T4/appsettings.json` with your API application's App ID URI or client ID from the Microsoft Entra app registration that issues the token.