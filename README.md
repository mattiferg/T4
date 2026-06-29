# T4

ASP.NET Core Web API solution with:

- `GET /Open/Echo1?name={name}` returning `Hello {name}`
- `GET /Closed/Echo2?name={name}` returning `Good to see you again {name}` for authenticated callers

The protected endpoint uses JWT bearer authentication and can accept tokens from:

- the existing Microsoft Entra authority configured in `Authentication:Authority`
- a second Microsoft Entra tenant configured in `Authentication:AdditionalEntraAuthority`
- Apple via `Authentication:Apple`
- Google via `Authentication:Google`

Before using it, update the audience values in `src/T4/appsettings.json` so they match the application identifier expected by each provider:

- `Authentication:Audience` for the existing and additional Entra-issued tokens
- `Authentication:Apple:Audience` for the Apple-issued token
- `Authentication:Google:Audience` for the Google-issued token