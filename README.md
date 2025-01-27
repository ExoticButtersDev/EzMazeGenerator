# EzMazeGenerator

---

This package is designed to help you create procedurally generated mazes directly within the Unity Editor. It’s a simple and straightforward tool for creating randomized mazes with just a few clicks. Whether you're working on a dungeon crawler, a maze-based puzzle game, or simply need a fun maze for testing, this package has you covered!

## Features
- **Customizable Maze Size**: Easily set the width, height, and depth of your maze. Whether you want a small maze or a large labyrinth, everything is customizable.
- **Control Wall and Structure Placement**: Adjust the probability of walls and structures appearing in the maze. You can control the density of the maze or have interesting structures emerge.
- **Lighting Support**: The package includes built-in lighting options, allowing you to place lights inside your maze. These lights can be customized with various types, intensities, and colors to enhance the appearance of your maze.
- **Reflection Probes**: Add reflection probes to your maze to improve lighting and reflections, which is especially useful if your maze includes shiny or reflective surfaces.
- **Materials Customization**: Easily change the materials on the maze's walls, ground, and ceiling to match the look of your game.
- **Generate and Clear Mazes**: Once you've set your parameters, you can create a new maze with the click of a button. You can also easily clear a generated maze and start again.

---

## Installation
1. Download the Unity package file from the [Releases](https://github.com/ExoticButtersDev/EzMazeGenerator/releases/latest).
2. Drag the file into your Unity project and extract it.
3. After extraction, go to the EzMazeGenerator folder and drag the Prefab into your scene.

---

## Usage
**The prefab itself is already set up, but you can tinker with the settings as needed.**

### Maze Settings

- **`mazeScale`** (`Vector3Int`):
  - Defines the width (X), height (Y), and depth (Z) of the maze.
  - Example: `(width, height, depth)`

- **`wallLength`** (`float`):
  - Sets the size of the walls in the maze. It controls how large each wall segment is in your scene.
  - Recommended: Use a round value (e.g., `2`, `5`) to avoid any scaling issues.

- **`wallSpawnPercentage`** (`int` [0–100]):
  - Determines the chance (in percentage) that a wall will be placed at a given position in the maze.
  - Example: `70` means there’s a 70% chance a wall will appear.

- **`structureSpawnPercentage`** (`int` [0–100]):
  - Similar to `wallSpawnPercentage`, this controls the chance (in percentage) that structures will be placed instead of walls.
  - Example: `30` means a 30% chance for structures.

### Objects

- **`structures`** (`List<GameObject>`):
  - A list of GameObjects that can be randomly placed inside the maze as structures (e.g., obstacles, items, or other interactive objects).
  - You can add your own custom structures here.

- **`lightPrefab`** (`GameObject`):
  - The prefab used for placing lights in the maze. You can use Unity's built-in light objects or create a custom one.

### Object Materials

- **`wallMaterial`** (`Material`):
  - The material applied to the maze's walls. You can customize it to fit your visual style.
  
- **`groundMaterial`** (`Material`):
  - The material applied to the ground of the maze (i.e., the floor).
  
- **`roofMaterial`** (`Material`):
  - The material applied to the roof/ceiling of the maze.

### Light Settings

- **`lightType`** (`LightType`):
  - Specifies the type of light to use in the maze. You can choose between **Point**, **Directional**, or **Rectangle**.
  
- **`intensity`** (`float`):
  - Controls the intensity (brightness) of the light placed in the maze.
  
- **`range`** (`float`):
  - Controls the range (distance) that the light affects in the scene.
  
- **`color`** (`Color`):
  - The color of the light placed in the maze.

### Reflection Probe Settings

- **`spawnReflectionProbes`** (`bool`):
  - If enabled, reflection probes will be placed inside the maze for better lighting and reflection effects.
  
- **`probeResolution`** (`int`):
  - Sets the resolution of the reflection probes. Higher values provide more detail in reflections but require more computing power.
  - Recommended: `1024` for good quality.

- **`boxProjection`** (`bool`):
  - Determines whether box projection is used for the reflection probes. Enable it for a more accurate reflection for box-shaped environments.

---

## License

EzMazeGenerator is **MIT Licensed**, so you’re free to use it, modify it, and distribute it as you wish!

---

## Contributing

Feel free to fork the repo and make any changes you like! If you spot a bug or have a suggestion, don’t hesitate to open an issue or submit a pull request.
