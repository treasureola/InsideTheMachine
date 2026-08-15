# Inside the Machine

A 3D interactive museum exhibit built in Unity 6, exploring the internal components of a personal computer. Visitors free-roam a cinematic corridor lined with ten pedestals, each displaying a real PC component, and can inspect each part, view it in X-ray mode, and test their knowledge with a build-order assembly quiz.

Created as a final project for Multimedia Foundations (Design of Interactive Media).

---

## Features

- **Free-roam navigation** — WASD movement with mouse look through a lit museum corridor
- **Ten component pedestals** — CPU, GPU, motherboard, RAM, PSU, storage, cooling, and more
- **Interactive info panels** — click a component to open a panel with a real photograph and specifications; panels position themselves to the left or right of the component depending on available screen space
- **X-Ray mode** — runtime material swap that fades component opacity and tints the motherboard, revealing internal structure
- **PC assembly quiz** — select components in correct build order via dropdown; correct picks fly to the motherboard along a Bezier arc
- **Cinematic camera** — SmoothStep flip transition on entry and an orbit mode for inspecting individual parts
- **Welcome screen** — CanvasGroup fade-in with camera lock until the visitor is ready
- **Layered audio** — 12-channel audio manager handling ambience, UI feedback, and assembly cues

All animation in this project is scripted from scratch in C#. Unity's Animator component is not used anywhere — every transition, fade, arc, and camera move is driven by runtime interpolation.

---

## Controls

| Input | Action |
|---|---|
| `W` `A` `S` `D` | Move through the corridor |
| Mouse | Look around |
| Left click | Open a component's info panel |
| `X` | Toggle X-Ray mode |
| `Esc` | Open / close menu |

> Update this table if your bindings differ.

---

## Requirements

- **Unity 6** (6000.0 or later) with the **Universal Render Pipeline (URP)**
- **Git LFS** — required to pull the 3D models, textures, and audio
- Roughly 2 GB of free disk space once the Library folder is generated

---

## Running the project

### 1. Install Git LFS

The repository stores all models, textures, and audio through Git LFS. Cloning without it gives you small text pointer files instead of real assets, and the scene will open with missing meshes.

```bash
git lfs install
```

### 2. Clone the repository

```bash
git clone https://github.com/treasureola/InsideTheMachine.git
cd InsideTheMachine
```

If you cloned before installing LFS, you can repair it in place:

```bash
git lfs install
git lfs pull
```

### 3. Open in Unity

1. Open **Unity Hub** → **Add** → select the cloned project folder
2. Open it with **Unity 6**
3. Wait for the first import — Unity rebuilds the `Library/` folder from scratch, which can take several minutes on first open

### 4. Play

Open the main scene from `Assets/Scenes/`, then press **Play** in the editor.

---

## Project structure

```
Assets/
├── Scenes/          Main exhibit scene
├── Scripts/         All C# runtime logic
├── Models/          Component meshes
├── Materials/       URP materials, including X-Ray swap targets
├── Textures/        Component photographs and surface maps
├── Audio/           Ambience, UI, and assembly SFX
└── UI/              Canvas prefabs, panels, TMP assets
Packages/
ProjectSettings/
```

`Library/`, `Temp/`, `Logs/`, and `Builds/` are intentionally excluded from version control — Unity regenerates them.

---

## Scripts

| Script | Responsibility |
|---|---|
| `AssemblyManager.cs` | Assembly flow, Bezier arc flight animation, quiz logic, dropdown handling, collider management |
| `CameraController.cs` | WASD movement, SmoothStep cinematic flip, Atan2-based orbit mode, menu reveal trigger |
| `XRayMode.cs` | Runtime material swap, alpha fade, motherboard tint, material restore |
| `InfoPanelManager.cs` | Smart left/right panel positioning |
| `AudioManager.cs` | 12-channel singleton with `DontDestroyOnLoad` and `PlayOneShot` |
| `WelcomeScreen.cs` | CanvasGroup fade with camera lock on entry |

---

## Implementation notes

A few decisions worth documenting for anyone reading the source:

**Runtime material swapping over shader property edits.** The imported GLTF models use a shader that doesn't expose the transparency properties URP expects, so changing alpha at runtime had no visible effect. X-Ray mode instead swaps in a separate URP Lit material and animates that, restoring the originals on exit.

**Dropdown index 0 workaround.** TextMeshPro's dropdown does not fire `onValueChanged` when the user selects the item already at index 0. The quiz works around this with a duplicate entry at the top, a placeholder overlay text object, and an `isResettingDropdown` flag to suppress spurious callbacks during programmatic resets.

**System-level collider management.** Clicking a UI element during assembly was bleeding through and triggering the info panel behind it. Rather than guarding each object individually, `SetComponentColliders` disables all component colliders for the duration of the assembly sequence.

**Atan2 orbit initialization.** Entering orbit mode used to snap the camera to a fixed starting angle. Deriving the initial angle from the camera's current position with `Atan2` makes the transition continuous from wherever the visitor happens to be standing.

---

## Credits

- 3D models sourced from [Sketchfab](https://sketchfab.com) (used unedited)
- Sound effects from [Mixkit](https://mixkit.co)
- Component photographs — *add your sources here*

> Replace this section with your full IEEE-formatted source list to match your submission document.

---

## Author

**Treasure Oluwalade**
Multimedia Foundations — Design of Interactive Media
