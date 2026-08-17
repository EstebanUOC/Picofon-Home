# Picofon Home

Therapeutic speech-language exercise application for children, built with Unity.
Backend-driven activities with adaptive difficulty, Firebase authentication, and
bilingual support (Catalan / Spanish).

---

## Requirements

| Tool            | Version        |
|-----------------|----------------|
| Unity Editor    | 2022.3.62f2    |
| Render Pipeline | URP 14.0.12    |
| .NET / C#       | C# 9.0         |
| Platform        | Android (primary) |
| Version Control | Git            |

---

## Architecture

The codebase follows a **layered architecture** enforced through Unity Assembly
Definitions (asmdefs). Each layer owns a namespace and has explicit, acyclic
dependency relationships. Scripts never reference types from a layer above them.

### Dependency Graph

```
Picofon.Utils                    (lowest layer, no project dependencies)
       |
Picofon.Components               (depends on Utils implicitly via Unity refs)
       |
Picofon.GoogleSignIn             (standalone plugin, no project dependencies)
       |
       +---------------------+
       |                     |
Picofon.Core                Picofon.UI
   |       |                    |
   |       v                    v
   |   Components + Utils   Components + Utils
   |
Picofon.Activities               (highest layer, depends on Core + Utils + Components)
```

### Assembly Definitions

| Assembly             | Root Folder                       | Role                                  |
|----------------------|-----------------------------------|---------------------------------------|
| `Picofon.Utils`      | `Assets/Scripts/Utils`            | Shared utilities, networking, config  |
| `Picofon.Components` | `Assets/Scripts/Components`       | Reusable Unity components             |
| `Picofon.UI`         | `Assets/Scripts/UI`               | UI-only widgets                       |
| `Picofon.GoogleSignIn`| `Assets/GoogleSignIn`            | Google Sign-In (vendored plugin)      |
| `Picofon.Core`       | `Assets/Scripts/Core`             | Auth, session, network, map path      |
| `Picofon.Activities` | `Assets/Scripts/Core/Activities`  | Activity implementations              |

Each asmdef has `autoReferenced: true` so downstream assemblies automatically
see upstream types without manual wiring.

### Namespace Convention

Every script file belongs to a namespace that mirrors its folder path relative
to the asmdef root:

```
Assets/Scripts/Core/Auth/Services/UserService.cs
    -> namespace Picofon.Core.Auth.Services

Assets/Scripts/Utils/HttpClientUnity.cs
    -> namespace Picofon.Utils

Assets/Scripts/Core/Activities/Basket/BasketManagerJG.cs
    -> namespace Picofon.Activities.Basket
```

Unity generates the root namespace automatically from the asmdef's
`rootNamespace` field. New files created via Unity's **Create > C# Script**
menu inherit the namespace of their parent folder.

---

## Project Structure

```
Picofon Home/
  Assets/
    GoogleSignIn/                  Google Sign-In plugin (own asmdef)
    Plugins/
      Dreamteck/                   Splines (vendor, 2 asmdefs)
    Prefabs/
      Auth/                        Login, register, modals
      Activities/                  Activity UI and game objects
      MapPath/                     Level grid, path visualization
    Scenes/
      AuthScene.unity              Login / registration
      MapPathScene.unity           Activity map
      Activities/
        BasketScene_J.unity        Judge variant
        BasketScene_R.unity        Relate variant
        BasketScene_S.unity        Select variant
        CrossRiverScene_J.unity    Cross-the-river (judge)
        Segmentation.unity         Word segmentation
    Scripts/
      Utils/                       Picofon.Utils
      Components/                  Picofon.Components
      UI/                          Picofon.UI
      Core/
        Auth/                      AuthManager, panels, services
        MapPath/                   Level map, path data
        Network/                   API models, response parsing
        Session/                   Runtime session state
        Activities/
          Basket/                  Basket game variants
          CrossRiver/              Cross-the-river game
          Feedback/                End-of-session feedback
          Segmentation/            Word segmentation game
    ScriptableObjects/
      MapPath/                     Level configs, event channels
      Languages/                   Language data (flags, codes)
      Scene Transitions/           Transition configs
  Packages/                        Unity package manifest
  ProjectSettings/                 Unity project settings
```

---

## Key Packages

| Package               | Version  | Purpose                                         |
|-----------------------|----------|--------------------------------------------------|
| UniTask               | git      | Async/await throughout the codebase              |
| PrimeTween            | 1.3.8    | UI animations (fade, scale, tween)               |
| Unity Localization    | 1.5.11   | Runtime string localization                      |
| Unity Addressables    | 1.22.3   | Audio asset loading per language / skill          |
| TextMeshPro           | 3.0.7    | Text rendering                                   |
| Firebase Auth         | 13.8.0   | Email/password + token-based authentication       |
| Firebase Analytics    | 13.8.0   | Usage analytics                                  |
| Dreamteck Splines     | vendor   | Path rendering for the activity map               |

---

## Data Flow

```
AuthScene                          MapPathScene                       Activities
+-----------+                      +--------------+                   +---------------+
| UI Panels |--[AuthManager]-->    | LevelManager |--[LevelPayload]-->| GameManager   |
| (Register |   manages session    | (MapManager) |  static state     | (per-game)    |
|  Login)   |   and services       |              |                   |               |
+-----------+                      +--------------+                   +---------------+
      |                                  |                                  |
      +---> UserService                  +---> SessionService              +---> BasketService
      +---> FirebaseService              +---> TherapyPlanService          +---> AudioLoader
                                                                  (cross-cutting)
```

**State transport between scenes** uses static payloads (`LevelPayload`,
`MapPathPayload`) passed before `SceneManager.LoadScene()`. This is a
deliberate simplification; a DI container or session context object would be
the next evolution for this pattern.

---

## Network Layer

All HTTP communication goes through `HttpClientUnity` (in Picofon.Utils), which
wraps `UnityWebRequest` with async/await via UniTask.

Service classes (in `Picofon.Core.Auth.Services` and activity `Services/`)
follow a consistent pattern:

```
Service -> HttpClientUnity -> API endpoint
       -> parse JSON response
       -> return ApiResult<T>
```

`ApiResult<T>` carries success/failure state and error messages.
`ApiResponseView<T>` unwraps the API envelope (`{success, message, data}`).

Backend URL is configured in `ApiConfig` with automatic fallback between
primary and mirror endpoints.

---

## Adding New Code

### New script in an existing layer

1. Create the file inside the correct folder under `Assets/Scripts/`.
2. Unity assigns the namespace automatically from the folder's asmdef.
3. Use only types from the same layer or layers below it.

### New activity

1. Create a folder under `Assets/Scripts/Core/Activities/<ActivityName>/`.
2. Create an asmdef for the activity if it has many scripts, or place the
   scripts inside `Picofon.Activities` (they inherit its asmdef automatically).
3. Add a new scene under `Assets/Scenes/Activities/`.
4. Add a `LevelConfig` ScriptableObject under `Assets/ScriptableObjects/MapPath/`.

### New API endpoint

1. Add the endpoint path to `ApiConfig` or the service class.
2. Define request/response DTOs in the service's `DTOs/` folder.
3. Return `ApiResult<T>` from the service method.

### Naming rules

- File name must match the primary class name (Unity MonoBehaviour detection).
- Each file contains one public class or interface.
- Namespace must match the folder path relative to the asmdef root.

---

## Editor Notes

- `Assets/Packages/` contains local `.tgz` archives for Firebase packages
  (not managed by Unity Package Manager registry).
- `*.csproj` and `*.sln` files are gitignored; Unity regenerates them on import.
- `Library/` is fully gitignored and contains compiled assemblies, asset cache,
  and Package Manager downloads.
