# DnD Trading

A Windows desktop application developed for a stationery distribution business as part of a University of KwaZulu-Natal team project in 2025.

The application brings customer, supplier, product, order, payment, and reporting workflows into one Windows Forms interface. It is the desktop component of the DnD Trading project; the [companion website](https://github.com/DarianRobert/DnD-Trading-Website) provides the web interface.

## Technology

| Component | Technology |
| --- | --- |
| Application | C# and Windows Forms |
| Runtime target | .NET Framework 4.7.2 |
| Data access | ADO.NET and a typed DataSet |
| Database | SQL Server |
| Reporting | SAP Crystal Reports; project references use assembly version 13.0.4000.0 |
| Development | Visual Studio solution and project files |

## Features represented in the source

- Customer and user management.
- Supplier and product management.
- Order creation, order requests, and order viewing.
- Payment entry.
- Report screens and Crystal Reports definitions.

## My contribution

I served as project manager and team leader, coordinating weekly development goals and project timelines, and contributed to frontend and backend development. This was a team project; the application contains work from multiple team members.

## Repository layout

The Visual Studio solution and C# source are available directly in the repository root. This portfolio fork preserves the original team commit history.

| Path | Purpose |
| --- | --- |
| `DnD Trading.sln` | Visual Studio solution |
| `DnD Trading.csproj` | Windows Forms application project |
| `Program.cs` | Application entry point |
| `Client.cs`, `Product.cs`, `Supplier.cs`, `User.cs` | Business entity screens |
| `CreateOrder.cs`, `CreateOrderRequest.cs`, `Order.cs`, `Payment.cs` | Order and payment screens |
| `Reports.cs` and the `.rpt` files | Reporting interface and report definitions |
| `WstGrp22DataSet.xsd` | Typed DataSet schema and table adapters |
| `App.config`, `Properties/Settings.settings` | Application and database configuration |

## Development setup

The project targets Windows and the full .NET Framework. It requires Visual Studio and the reporting/database dependencies below.

1. Install Visual Studio with the **.NET desktop development** workload and the **.NET Framework 4.7.2 targeting pack**.
2. Install compatible SAP Crystal Reports development tools and runtime components. Match their architecture to the selected application build configuration and resolve the `CrystalDecisions.*` references.
3. Clone this repository, then open `DnD Trading.sln`:

   ```bash
   git clone https://github.com/DarianRobert/DnD-Trading.git
   cd DnD-Trading
   ```
4. Prepare an isolated SQL Server development database with the tables and relationships expected by the typed DataSet. Configure the application settings and table adapters for that local database.
5. Build the solution and run it through Visual Studio.

## Current limitations

- A database creation script and sample dataset have not yet been packaged. Database-backed workflows require that setup before they can be reproduced.
- Crystal Reports is an external prerequisite.
- Historical database configuration is included in the source. Replace it with configuration for an isolated development database before running the application.
- A clean Windows build and the business workflows have not yet been verified on a fresh development environment.

## Team provenance

This is Darian Robert's portfolio fork of the [original team repository](https://github.com/ArjunRamphal/DnD-Trading). The application and its history include work from multiple team members.

## Related work

- [DnD Trading website](https://github.com/DarianRobert/DnD-Trading-Website)
- [My project portfolio](https://darianrobert.github.io/)

