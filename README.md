# Avalonia Clean Architecture Cross-Platform Template
## avalonia-clean-xplat

An opinionated `dotnet new` template for Avalonia cross-platform applications following Clean Architecture principles.

Targets desktop (Windows, Linux, macOS), browser (WASM), and mobile (Android phone, Android tablet, iOS, iPadOS) with an optional ASP.NET Core API backend.

---

## Architecture

```
MyApp.Domain
    ← MyApp.Application
        ← MyApp.Infrastructure        (--NoApi removes this)
            ← MyApp.API               (--NoApi removes this)
        ← MyApp.Views.Desktop
            ← MyApp.Desktop
            ← MyApp.Browser           (--NoBrowser removes this)
        ← MyApp.Mobile
            ← MyApp.Android.Phone
            ← MyApp.Android.Tablet
            ← MyApp.iOS
            ← MyApp.iPadOS
```

---

## Install

```
dotnet new install .
```

Or from a NuGet package once published:

```
dotnet new install AvaloniaCleanXplat.Templates
```

---

## Usage

```
dotnet new avalonia-clean-xplat -n MyApp
```

### Options

| Flag                | Default  | Description                                                          |
| ------------------- | -------- | -------------------------------------------------------------------- |
| `--NoApi`           | `false`  | Exclude `MyApp.Infrastructure`, `MyApp.API`, and their test projects |
| `--NoBrowser`       | `false`  | Exclude `MyApp.Browser` (WASM target)                                |
| `--AvaloniaVersion` | `12.0.2` | Avalonia package version across all projects                         |


### Examples

Full solution:

```
dotnet new avalonia-clean-xplat -n MyApp
```

Client-only (no backend, no WASM):

```
dotnet new avalonia-clean-xplat -n MyApp --NoApi --NoBrowser
```

Specific Avalonia version:

```
dotnet new avalonia-clean-xplat -n MyApp --AvaloniaVersion 12.1.0
```

---

## What you get

After `dotnet new avalonia-clean-xplat -n TodoApp`:

```
TodoApp/
└── src/
    ├── TodoApp.Domain/
    ├── TodoApp.Application/
    ├── TodoApp.Infrastructure/
    ├── TodoApp.API/
    ├── TodoApp.Desktop/
    ├── TodoApp.Views.Desktop/
    ├── TodoApp.Browser/
    ├── TodoApp.Mobile/
    ├── TodoApp.Android.Phone/
    ├── TodoApp.Android.Tablet/
    ├── TodoApp.iOS/
    ├── TodoApp.iPadOS/
    ├── TodoApp.Domain.Tests/
    ├── TodoApp.Application.Tests/
    ├── TodoApp.Infrastructure.Tests/
    ├── TodoApp.API.Tests/
    ├── TodoApp.Desktop.Tests/
    ├── TodoApp.Mobile.Tests/
    └── TodoApp.slnx
```

The solution file is pre-wired. Open `TodoApp.slnx` and build.

---

## Test frameworks

Test projects reference only their production counterpart — no test framework NuGet packages are included. Add your preferred framework:

```
dotnet add TodoApp.Domain.Tests package xunit
dotnet add TodoApp.Domain.Tests package xunit.runner.visualstudio
dotnet add TodoApp.Domain.Tests package FluentAssertions
```

Popular options: **xUnit**, NUnit, MSTest · **NSubstitute**, Moq, FakeItEasy · **FluentAssertions**, Shouldly

---

## Packaging as a NuGet template

```
dotnet new install Microsoft.TemplateEngine.Authoring.Templates
dotnet new templatepack -n "AvaloniaCleanXplat.Templates"
```

Move the `template/` folder into the generated `content/` folder, fill in the `.csproj` metadata, then:

```
dotnet pack
dotnet new install bin/Release/AvaloniaCleanXplat.Templates.1.0.0.nupkg
```

---

## Licence

AGPL-3.0-only. See [LICENCE](LICENCE).
