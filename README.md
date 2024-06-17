# Tic-Tac-Toe Game with Dependency Injection and AI

This project is a Tic-Tac-Toe game implemented in Unity with a robust architecture using Dependency Injection. It supports both local multiplayer and player vs AI modes. The game includes features such as game state management, undo functionality, hint provision, and dynamic asset bundle loading.

## Features

- **Local Multiplayer:** Play against a friend on the same device.
- **AI Opponent:** Challenge the AI and test your skills.
- **Undo Functionality:** Undo the last two moves in the game.
- **Hints:** Get hints to improve your gameplay.
- **Dependency Injection:** Clean and maintainable codebase using Dependency Injection.
- **Dynamic Asset Loading:** Load and manage asset bundles dynamically.

## Installation

1. **Clone or download the repository.**
2. **Open the project in Unity:**
   - Launch Unity Hub.
   - Click on "Open" and navigate to the cloned project folder.
   - Select the folder and open the project.
3. **Resolve dependencies:**
   - Ensure you have the required packages installed via Unity Package Manager.

## Usage

- **Start the game:**
  - Press the play button in Unity to start the game.
  - Choose between local multiplayer or AI opponent from the main menu.

- **Gameplay:**
  - Click on the board to make your move.
  - Use the undo button to revert the last two moves.
  - Press the hint button for a suggested move.
  - Use the `LoadNewBundleButton` to dynamically load a new asset bundle by providing the bundle name in the input field.

## Architecture

This project follows a modular and maintainable architecture using Dependency Injection. The core components include:

- **IGameLogic:** Interface for game logic implementation.
- **IPlayerAssigner:** Interface for assigning player IDs and types (local or AI).
- **GameStateView:** Manages UI updates based on game state changes.
- **PlayersController:** Handles player input and game logic interaction.

### Components

#### Dependency Injection

The `Injector` class manages dependency injection, ensuring loose coupling between components.

#### Game Logic

`IGameLogic` interface defines the core game logic, including making moves, undoing moves, and resetting the game.

#### TicTacToeGameLogic Class 

The `TicTacToeGameLogic` class manages the logic of a tic-tac-toe game, including handling moves, checking game states,
and maintaining the game board. It utilizes observable properties to track changes in the game state and board.

- **GameBoard:** Observable property representing the current state of the game board.
- **CurrentGameState:** Observable property representing the current state of the game (e.g., awaiting, in progress, won, draw).
- **OnLastMoveUndo:** Observable property triggered when the last move is undone.
- **Undo Functionality:** Ability to undo the last two moves in the game.
- **Timeout Handling:** Timer-based handling for turn timeouts.
- **Winning Conditions:** Checks for winning conditions based on rows, columns, and diagonals.
- **Hint Functionality:** Provides a hint for the next move.

#### UI Components

- **GameStateView:** Manages the main game UI and state transitions.
- **MainMenuPanel:** Handles main menu interactions.
- **GameEndPanel:** Displays the game end screen with options to restart.
- **LoadNewBundleButton:** Allows users to load new asset bundles dynamically.

#### AssetBundleLoader

The `AssetBundleLoader` class provides functionalities for loading and managing asset bundles asynchronously. It allows dynamic loading of assets such as sprites and supports force reloading of asset bundles.
