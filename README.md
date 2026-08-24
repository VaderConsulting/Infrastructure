# Infrastructure

VS 2008 Stratatel Core.Network / Directory / FileTransfer / SQL / Security / Licensing / Compression working copy (.NET 3.5). Test.Form1 can StartServer/Connect on 192.168.1.2:80 and FTP-upload with credentials that are gitignored (see Form1.vb.example). Vendored ARSoft.Tools.Net (Alexander Reinert, Apache 2.0) and SmallDHCPServer_CodeProject sit beside the Stratatel projects. Open `Infrastructure.sln`. This is a historical working copy from Dave Robinson / VaderConsulting.

**Source last updated:** 2017-03-03  
**Language:** C#, VB.NET  
**Target:** v3.5, v4.5, v4.5.2  
**Output:** Exe, Library, WinExe

## What it is

VS 2008 Stratatel Core.Network / Directory / FileTransfer / SQL / Security / Licensing / Compression working copy (.NET 3.5). Test.Form1 can StartServer/Connect on 192.168.1.2:80 and FTP-upload with credentials that are gitignored (see Form1.vb.example). Vendored ARSoft.Tools.Net (Alexander Reinert, Apache 2.0) and SmallDHCPServer_CodeProject sit beside the Stratatel projects. Open `Infrastructure.sln`. This is a historical working copy from Dave Robinson / VaderConsulting.

## Solution structure

| Project | Language | Path |
|---------|----------|------|
| `TinyDHCP` | C# | `SmallDHCPServer_CodeProject/SmallDHCPServer_C/TinyDHCP.csproj` |
| `SmallDHCPServer_C` | C# | `SmallDHCPServer_CodeProject/Backup/SmallDHCPServer_C/SmallDHCPServer_C.csproj` |
| `Test` | VB.NET | `Test/Test.vbproj` |
| `Compression` | VB.NET | `Compression/Compression.vbproj` |
| `Config` | VB.NET | `Config/Config.vbproj` |
| `FileTransfer` | VB.NET | `FileTransfer/FileTransfer.vbproj` |
| `Scheduling` | VB.NET | `Scheduling/Scheduling.vbproj` |
| `Directory` | VB.NET | `Directory/Directory.vbproj` |
| `Tracing` | VB.NET | `Tracing/Tracing.vbproj` |
| `TaskScheduler` | VB.NET | `TaskScheduler/TaskScheduler.vbproj` |
| `Licensing` | VB.NET | `Licensing/Licensing.vbproj` |
| `Common` | VB.NET | `Common/Common.vbproj` |
| `InstallHelper` | VB.NET | `InstallHelper/InstallHelper.vbproj` |
| `ResetPassword` | VB.NET | `ResetPassword/ResetPassword.vbproj` |
| `AutoDiscovery-app-Client` | C# | `AutoDiscovery-app-Client/AutoDiscovery-app-Client.csproj` |
| `XML` | VB.NET | `XML/XML.vbproj` |
| `AutoDiscovery-app-Server` | C# | `AutoDiscovery-app-Server/AutoDiscovery-app-Server.csproj` |
| `Network` | VB.NET | `Network/Network.vbproj` |
| `Encryption` | VB.NET | `Encryption/Encryption.vbproj` |
| `Security` | VB.NET | `Security/Security.vbproj` |
| `ARSoft.Tools.Net` | C# | `ARSoft.Tools.Net/ARSoft.Tools.Net/ARSoft.Tools.Net.csproj` |
| `ZipFile` | VB.NET | `ZipFile/ZipFile.vbproj` |
| `Setup` | VB.NET | `Setup/Setup.vbproj` |
| `Test` | VB.NET | `Serial/Test/Test.vbproj` |
| `PABX` | VB.NET | `Serial/PABX/PABX.vbproj` |
| `PABX` | VB.NET | `Serial/Tx/PABX.vbproj` |
| `Serial` | VB.NET | `Serial/Serial/Serial.vbproj` |
| `Test` | VB.NET | `Serial/Backup/Test/Test.vbproj` |
| `PABX` | VB.NET | `Serial/Backup/PABX/PABX.vbproj` |
| `Serial` | VB.NET | `Serial/Backup/Serial/Serial.vbproj` |
| `SQL` | VB.NET | `SQL/SQL.vbproj` |
| `Logging` | VB.NET | `Logging/Logging.vbproj` |

## How to open

Open `Infrastructure.sln` in Visual Studio.

## Attribution and provenance

- **Assembly company:** Alexander Reinert, Marcello Cauchi Savona, Microsoft, Stratatel Ltd, stratatel
- **Assembly copyright:** Copyright ©  2010, Copyright © Alexander Reinert 2010..2016, Copyright © Marcello Cauchi Savona, Copyright © Microsoft 2009, Copyright © Microsoft 2010, Copyright © Stratatel Ltd 2009, Copyright © Stratatel Ltd 2010, Copyright © Stratatel Ltd 2011, Copyright © stratatel 2008

## License

MIT. See `LICENSE`.
