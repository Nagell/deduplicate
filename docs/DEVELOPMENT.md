<a id="development-top"></a>

# Development

<!-- TABLE OF CONTENTS -->
- [Development](#development)
  - [Documentation](#documentation)
  - [Common Commands](#common-commands)
    - [VS Code shortcuts](#vs-code-shortcuts)
    - [Production build](#production-build)

## Documentation

- [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
- [.NET 9](https://dotnet.microsoft.com/)
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Windows App SDK 1.6](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)

<p align="right">(<a href="#development-top">back to top</a>)</p>

## Common Commands

```sh
# Compile the project
dotnet build

# Build and launch the app
dotnet run

# Launch with .NET Hot Reload (C# changes applied live)
dotnet watch run

# Remove build artifacts
dotnet clean
```

<p align="right">(<a href="#development-top">back to top</a>)</p>

### VS Code shortcuts

| Shortcut | Action |
| --- | --- |
| `Ctrl+Shift+B` | Build |
| `F5` | Debug (attach debugger, breakpoints) |
| `Ctrl+Shift+P` → `Tasks: Run Task` | Run / Watch / Clean |

<p align="right">(<a href="#development-top">back to top</a>)</p>

### Production build

Publish a self-contained exe that runs without .NET installed on the target machine:

```sh
dotnet publish -c Release -r win-x64 -p:Platform=x64 --self-contained true
```

Output: `bin\x64\Release\net9.0-windows10.0.19041.0\win-x64\publish\Deduplicate.exe`

> [!NOTE]
> You can drop `--self-contained true` if you're okay requiring .NET 9 on the target machine (smaller output).

<p align="right">(<a href="#development-top">back to top</a>)</p>
