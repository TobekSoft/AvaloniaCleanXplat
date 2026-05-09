# Avalonia Clean Architecture Cross-Platform Template Specification

## Overview

An opinionated `dotnet new` template for Avalonia cross-platform applications following Clean Architecture principles. Designed as a starting point for apps targeting desktop (Windows, Linux, macOS), browser (WASM), and mobile (Android phone, Android tablet, iOS, iPadOS) with an optional ASP.NET Core API backend.

This template has grown from a desire to have a more complex OOTB template for Avalonia apps. The official Avalonia cross-platform template assumes Views, ViewModels, and Models are shared across all platforms. This template opines that that doesn't go far enough. Depending on your context and UI/UX needs, desktop and mobile may be different consumers with different data shapes, different interaction patterns, and different platform capabilities.

The browser target shares desktop views on the basis that a browser on a desktop is closer to a desktop experience than a mobile one. Developers who disagree are free to extract `MyApp.Views.Browser` and fork accordingly .

---

## Licence

AGPLv3. Anyone using this template in a networked application must share their changes.

---

## Template Identity

- **Template name:** `avalonia-clean-xplat`
- **Short name:** `dotnet new avalonia-clean-xplat`
- **Framework:** .NET 10+
- **Avalonia version:** 12.0.2+

---

## Optional Template Groups

The template supports two optional groups excluded via template parameters:

### `--no-api` — excludes the API layer

Removes `MyApp.Infrastructure`, `MyApp.API`, `MyApp.Infrastructure.Tests`, and `MyApp.API.Tests` from the generated solution. Use when the backend is  provided externally or the app connects to a third-party API.

### `--no-browser` — excludes the browser target

Removes `MyApp.Browser` from the generated solution. Use when a WASM target
is not required.

Both flags can be combined:

```
dotnet new avalonia-clean-xplat --no-api --no-browser
```

---

## Project Structure

```
Solution/
│
├── MyApp.Domain
├── MyApp.Application
├── MyApp.Infrastructure            ← optional (--no-api)
├── MyApp.API                       ← optional (--no-api)
│
├── MyApp.Desktop
├── MyApp.Views.Desktop
├── MyApp.Browser                   ← optional (--no-browser)
│
├── MyApp.Mobile
├── MyApp.Android.Phone
├── MyApp.Android.Tablet
├── MyApp.iOS
├── MyApp.iPadOS
│
├── MyApp.Domain.Tests
├── MyApp.Application.Tests
├── MyApp.Infrastructure.Tests      ← optional (--no-api)
├── MyApp.API.Tests                 ← optional (--no-api)
├── MyApp.Desktop.Tests
└── MyApp.Mobile.Tests
```

---

## Dependency Graph

```
MyApp.Domain
    ← MyApp.Application
        ← MyApp.Infrastructure
            ← MyApp.API
        ← MyApp.Views.Desktop
            ← MyApp.Desktop
            ← MyApp.Browser
        ← MyApp.Mobile
            ← MyApp.Android.Phone
            ← MyApp.Android.Tablet
            ← MyApp.iOS
            ← MyApp.iPadOS
```

No project references anything above itself in this graph. Infrastructure and API are server-side only — client projects never reference them.

---

## Project Responsibilities

---

### MyApp.Domain

Pure domain layer. No dependencies on anything. The one project shared across the entire solution including the API. Changes here affect everyone.

```
MyApp.Domain/
    Entities/               ← POCOs, pure domain objects
    Enums/                  ← domain enumerations
    Exceptions/             ← domain-specific exceptions
    Interfaces/
        Repositories/       ← IProductRepository etc
        Services/           ← IProductService etc
    ValueObjects/           ← immutable domain value types
```

**NuGet references:** none  
**Project references:** none

---

### MyApp.Application

Orchestration and shared HTTP contracts. Shared between the API and both client families. The correct home for request/response DTOs — not Domain. Domain does not know what HTTP is.

```
MyApp.Application/
    DTOs/
        Requests/           ← inbound HTTP contract shapes
        Responses/          ← outbound HTTP contract shapes
    Interfaces/             ← application-level service interfaces
    Mappings/               ← DTO to/from POCO mapping profiles
    Services/               ← orchestration
    Validators/             ← request validation logic
```

**NuGet references:** none  
**Project references:** MyApp.Domain

---

### MyApp.Infrastructure *(optional — excluded by --no-api)*

Implements Domain interfaces. Pure data access, no orchestration logic. Orchestration belongs in Application services, not here.

```
MyApp.Infrastructure/
    Data/
        Configurations/     ← EF Core entity configurations
        Migrations/         ← EF Core migrations
    Repositories/           ← implements Domain repository interfaces
    Services/               ← implements Domain service interfaces
```

**NuGet references:** EF Core, Dapper, database provider  
**Project references:** MyApp.Domain, MyApp.Application

---

### MyApp.API *(optional — excluded by --no-api)*

Thin ASP.NET Core layer. Maps HTTP to Application layer use cases. Controllers should contain no business logic — that lives in Application.

```
MyApp.API/
    Controllers/
    Middleware/
    Extensions/             ← service registration, pipeline setup
    Properties/             ← launchSettings.json
    Program.cs
```

**NuGet references:** ASP.NET Core  
**Project references:** MyApp.Application, MyApp.Infrastructure

---

### MyApp.Desktop

Avalonia desktop entry point and all desktop client concerns. Targets Windows, Linux, and macOS through build targets — no separate per-OS projects needed. Views live in MyApp.Views.Desktop to allow sharing with the browser target.

```
MyApp.Desktop/
    Assets/                 ← icons, images, fonts
    Services/               ← HTTP client service implementations
    DTOs/                   ← desktop-specific DTOs
                               full fat, broadly editable
    ViewModels/
        Base/
            ViewModelBase.cs    ← extends ObservableObject
                                   common desktop VM infrastructure
    Converters/             ← IValueConverter implementations
    Extensions/             ← service registration helpers
    Program.cs
    App.xaml
    App.xaml.cs             ← DI setup, view locator registration
                               registers MyApp.Views.Desktop views
```

**NuGet references:** Avalonia, Avalonia.Desktop, CommunityToolkit.Mvvm  
**Project references:** MyApp.Domain, MyApp.Application, MyApp.Views.Desktop

---

### MyApp.Views.Desktop

Extracted desktop view library. Exists as a separate project so that both MyApp.Desktop and MyApp.Browser can reference it without circular dependencies.

```
MyApp.Views.Desktop/
    Controls/               ← reusable user controls
    Pages/                  ← full page views
    Converters/             ← desktop-specific value converters
```

**NuGet references:** Avalonia  
**Project references:** MyApp.Domain, MyApp.Application

---

### MyApp.Browser *(optional — excluded by --no-browser)*

Avalonia WASM entry point hosted via Blazor. Shares desktop views on the basis that a browser on a desktop is a desktop experience. Developers targeting mobile browsers should extract MyApp.Views.Browser and wire accordingly.

```
MyApp.Browser/
    wwwroot/                ← static web assets
    App.xaml
    App.xaml.cs             ← DI setup, registers desktop view locator
    Program.cs              ← Blazor hosted WASM entry point
```

**NuGet references:** Avalonia, Avalonia.Web, Avalonia.Browser  
**Project references:** MyApp.Domain, MyApp.Application, MyApp.Views.Desktop

---

### MyApp.Mobile

Shared mobile class library. Consumed by all four mobile entry point projects. Contains all mobile concerns — services, SQLite cache, DTOs, ViewModels, and views for both phone and tablet form factors. The view split between phone and tablet lives at the folder level inside this project. The four entry points select which folder's views to register in their respective App.xaml.cs via MobileAppSetup.

#### On DTOs

Mobile DTOs are record-based and trimmed. Most mobile data is for viewing only, with limited input or editing. Full classes with change tracking are reserved for collections that can be edited or added to on mobile — which should be the minority.

```
MyApp.Mobile/
    Assets/                 ← mobile-specific icons, images
    Cache/
        Entities/           ← SQLite cache table definitions
                               distinct from Domain entities
                               these are a local persistence concern
                               not a domain concern
        Repositories/       ← cache read/write implementations
    Services/               ← HTTP client service implementations
    DTOs/                   ← mobile-specific DTOs
                               record-based for view-only data
                               full classes for editable collections only
    ViewModels/
        Base/
            ViewModelBase.cs    ← extends ObservableObject
                                   mobile-specific base concerns
                                   connectivity awareness
                                   cache state
    Views/
        Phone/              ← phone-specific views
        Tablet/             ← tablet-specific views
        Controls/           ← shared mobile controls
    Converters/             ← mobile-specific value converters
    Bootstrap/
        MobileAppSetup.cs   ← shared DI registration
                               called by all four entry points
                               accepts view locator as parameter
```

**NuGet references:** Avalonia, CommunityToolkit.Mvvm, sqlite-net-pcl  
**Project references:** MyApp.Domain, MyApp.Application

---

### MyApp.Android.Phone

Android phone entry point. Thin bootstrapper only. All substantive mobile code lives in MyApp.Mobile. The only meaningful difference between the four mobile entry points is which view folder they register.

```
MyApp.Android.Phone/
    Assets/
    Resources/
    App.xaml
    App.xaml.cs             ← calls MobileAppSetup.Configure()
                               registers Views/Phone/ view mappings
    MainActivity.cs
```

**Project references:** MyApp.Mobile

---

### MyApp.Android.Tablet

Android tablet entry point. Identical structure to Android.Phone. Differs only in view locator registration pointing at Views/Tablet/.

```
MyApp.Android.Tablet/
    Assets/
    Resources/
    App.xaml
    App.xaml.cs             ← calls MobileAppSetup.Configure()
                               registers Views/Tablet/ view mappings
    MainActivity.cs
```

**Project references:** MyApp.Mobile

---

### MyApp.iOS

iOS phone entry point.

```
MyApp.iOS/
    Assets.xcassets/
    Resources/
    App.xaml
    App.xaml.cs             ← calls MobileAppSetup.Configure()
                               registers Views/Phone/ view mappings
    AppDelegate.cs
    Info.plist
```

**Project references:** MyApp.Mobile

---

### MyApp.iPadOS

iPadOS tablet entry point. Apple provides an explicit OS-level split between iOS and iPadOS, making this a genuine separate target rather than a runtime adaptation. Take advantage of it.

```
MyApp.iPadOS/
    Assets.xcassets/
    Resources/
    App.xaml
    App.xaml.cs             ← calls MobileAppSetup.Configure()
                               registers Views/Tablet/ view mappings
    AppDelegate.cs
    Info.plist
```

**Project references:** MyApp.Mobile

---

## Test Projects

Test projects mirror their production counterparts in folder structure. No test framework NuGet references are included. The choice of test framework, mocking library, and assertion library is left to the developer.

Popular options are noted in each placeholder file but not prescribed.

```
MyApp.Domain.Tests/
    Entities/
    ValueObjects/

MyApp.Application.Tests/
    Services/
    Validators/
    Mappings/

MyApp.Infrastructure.Tests/     ← optional (--no-api)
    Repositories/
    Services/

MyApp.API.Tests/                ← optional (--no-api)
    Controllers/
    Middleware/

MyApp.Desktop.Tests/
    Services/
    ViewModels/

MyApp.Mobile.Tests/
    Services/
    Cache/
    ViewModels/
```

Each test project contains a single `PlaceholderTests.cs`:

```csharp
// Add your preferred test framework NuGet package to this project.
//
// Popular test frameworks:   xUnit, NUnit, MSTest
// Popular mocking libraries: NSubstitute, Moq, FakeItEasy
// Popular assertion libraries: FluentAssertions, Shouldly
//
// This project references [ProjectUnderTest] and is ready to go.

public class PlaceholderTests
{
    // Add your tests here
}
```

---

## Key Architectural Decisions

### Why are Desktop and Mobile separate client families?

Desktop and mobile are different consumers of the same API. They have different data shapes, different interaction patterns, different caching strategies, and different what-matters-right-now priorities. The Avalonia Xplat template treats them as the same thing. Here, they are not the same thing.

### Why are there four mobile entry points?

Android and iOS require separate entry point projects by necessity — different build toolchains, different bootstrappers. Android phone and tablet are separate projects because sometimes developing two apps is cleaner than one app doing runtime gymnastics to figure out what size screen it's dealing with. Apple provides an explicit iOS/iPadOS split.

### Why does Browser share Desktop views?

A browser on a desktop is a desktop experience. This is an opinionated default. Fork it if you disagree — that's what the AGPL licence is for.

### Why are mobile DTOs record-based?

Most mobile data is for viewing only. Records are immutable, lightweight, and carry no change tracking overhead. The mobile experience is a trimmed, focused conversation with the same underlying data — not a full editing suite that happens to run on a small screen.

### Why is Application a separate layer from Domain?

Domain should be pure — POCOs, interfaces, value objects, business rules with zero external dependencies. Request/response DTOs are HTTP contract concerns, not domain concerns. They live in Application. This distinction matters at template level even if it gets blurred in practice on smaller projects.

### Why are ViewModels not shared between Desktop and Mobile?

ViewModels diverge because the layers below them diverge. Desktop ViewModels bind to full DTOs with broad editability. Mobile ViewModels are more likely to bind to trimmed record-based DTOs with limited editing scope. What starts as 90% overlap quietly becomes 60% overlap as the mobile experience matures. Starting them separate avoids base class contortions later.

### Why are test frameworks not prescribed?

A template that bakes in a test framework is making a choice on behalf of every developer who uses it (even if you *should* choose xUnit). CommunityToolkit.Mvvm is prescribed because it is the de facto standard for Avalonia MVVM development and it's a less controversial opinion. Test frameworks are a different kind of decision and this template stays out of it.

---

## GitHub Repository Structure

```
/
├── template/               ← the actual dotnet new template
│   ├── .template.config/
│   │   └── template.json
│   └── src/
│       └── [all projects]
├── README.md               ← architectural rationale, usage instructions
├── CONTRIBUTING.md         ← contribution guidelines
└── LICENCE                 ← AGPLv3
```

---

## Notes for Claude Code

This document is the specification. Build the template from this, not from
the official Avalonia cross-platform template structure. The official template
is a parts donor for mobile plumbing only — Android and iOS bootstrapper
boilerplate, platform manifests, asset catalogs. The architecture is this
document.

When generating the template:

1. Start with Domain and work outward following the dependency graph
2. Verify each project builds before moving to the next
3. The four mobile entry points are nearly identical — build one, copy three,
   adjust view locator registration in each
4. MyApp.Views.Desktop exists solely to break the circular dependency between
   Desktop, Browser, and their shared views — keep it thin
5. `template.json` should implement `--no-api` and `--no-browser` as
   conditional source inclusions using the `sources` and `modifiers` mechanism
6. Avalonia version should be a template parameter so it can be updated
   without editing every csproj manually
7. The mobile bootstrapper plumbing (MainActivity, AppDelegate, Info.plist,
   AndroidManifest.xml) should be sourced from a fresh Avalonia xplat template
   at the current Avalonia version rather than hand-authored — get the
   plumbing right, then reshape the architecture on top of it
