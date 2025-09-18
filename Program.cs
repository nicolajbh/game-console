using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;

internal class Program
{
  static Random rnd = new();

  public static void Main(string[] args)
  {
    PrintWelcomeScreen();
    ShowMainMenu();
  }

  static void ShowMainMenu()
  {
    var gameMenu = new Dictionary<string, Action>
    {
      { "Number Battle", PlayNumberGame},
      { "Kryds & Bolle", XO },
      { "Game of Life", PlayGameOfLife},
      { "Exit", () => Environment.Exit(0) }
    };
    var menuTitles = gameMenu.Keys.ToArray();
    int selectedIndex = 0;
    while (true)
    {
      Console.Clear();
      PrintMenu(menuTitles, selectedIndex);
      int result = MenuSelect(menuTitles, selectedIndex);
      if (result == -1) break; // user made selection, exit menu loop
      selectedIndex = result;
    }
    Console.Clear();
    string selectedGame = menuTitles[selectedIndex];
    Console.WriteLine(
        selectedGame == "Exit"
        ? "Exiting..."
        : $"Starting {selectedGame}"
        );

    Thread.Sleep(2000); // simulate game loading
    gameMenu[selectedGame]();
  }

  /// <summary>
  /// Outputs welcome graphics to console
  /// Ascii art generated using https://patorjk.com/software/taag/
  /// </summary>
  static void PrintWelcomeScreen()
  {
    Console.Clear();
    Console.WriteLine(@"====================================================================================================");
    Console.WriteLine(@" /$$      /$$ /$$   /$$        /$$$$$$   /$$$$$$  /$$   /$$  /$$$$$$   /$$$$$$  /$$       /$$$$$$$$");
    Console.WriteLine(@"| $$$    /$$$| $$$ | $$       /$$__  $$ /$$__  $$| $$$ | $$ /$$__  $$ /$$__  $$| $$      | $$_____/");
    Console.WriteLine(@"| $$$$  /$$$$| $$$$| $$      | $$  \__/| $$  \ $$| $$$$| $$| $$  \__/| $$  \ $$| $$      | $$      ");
    Console.WriteLine(@"| $$ $$/$$ $$| $$ $$ $$      | $$      | $$  | $$| $$ $$ $$|  $$$$$$ | $$  | $$| $$      | $$$$$   ");
    Console.WriteLine(@"| $$  $$$| $$| $$  $$$$      | $$      | $$  | $$| $$  $$$$ \____  $$| $$  | $$| $$      | $$__/   ");
    Console.WriteLine(@"| $$\  $ | $$| $$\  $$$      | $$    $$| $$  | $$| $$\  $$$ /$$  \ $$| $$  | $$| $$      | $$      ");
    Console.WriteLine(@"| $$ \/  | $$| $$ \  $$      |  $$$$$$/|  $$$$$$/| $$ \  $$|  $$$$$$/|  $$$$$$/| $$$$$$$$| $$$$$$$$");
    Console.WriteLine(@"|__/     |__/|__/  \__/       \______/  \______/ |__/  \__/ \______/  \______/ |________/|________/");
    Console.WriteLine(@"====================================================================================================");
    Console.WriteLine();
    Console.WriteLine(@"                                  Welcome to the MNConsole v1.0!");
    Console.WriteLine(@"====================================================================================================");
    Console.WriteLine("                                      Press any key to start...");
    Console.ReadKey();
  }

  /// <summary>
  /// Loops through menu titles and prints to console
  /// Adds arrow to item currently selected from menu
  /// </summary>
  static void PrintMenu(string[] menuTitles, int currentIndex)
  {
    for (int i = 0; i < menuTitles.Length; i++)
    {
      if (i == currentIndex)
      {
        Console.WriteLine($"> {menuTitles[i]}");
      }
      else
      {
        Console.WriteLine($"{menuTitles[i]}  "); // spaces to write over previously selected items
      }
    }
    Console.WriteLine("\nUse ^/v arrows to navigate, Enter to select");
  }

  /// <summary>
  /// Reads key presses for selecting menu items
  /// </summary>
  /// <returns> New index for selector or -1 for enter </returns>
  static int MenuSelect(string[] menuTitles, int currentIndex)
  {
    return Console.ReadKey().Key switch
    {
      ConsoleKey.UpArrow => Math.Max(0, currentIndex - 1),
      ConsoleKey.DownArrow => Math.Min(menuTitles.Length - 1, currentIndex + 1),
      ConsoleKey.Enter => -1,
      _ => currentIndex,
    };
  }

  static int PlayAgain()
  {
    Console.Clear();
    Console.Write("Play Again? (Y/N): ");
    string input;
    while (true)
    {
      input = (Console.ReadLine() ?? "").ToUpper();
      if (input == "Y" || input == "N") break;
    }
    return input switch
    {
      "Y" => 1,
      "N" => 0,
      _ => -1,
    };
  }

  // ==================================================
  // Kryds & Bolle
  // Af: Matias
  // ==================================================
  public static void XO()
  {
    string playerToken = rnd.Next(0, 2) == 0 ? "X" : "O"; // Gemmer spillerens type af spilbrik.
    string botToken = playerToken == "X" ? "O" : "X"; // Tildeler den anden brik til bot'en.
    string userInput = "";
    string[,] gameBoard = { { " ", " ", " " }, { " ", " ", " " }, { " ", " ", " " } }; // Opretter spilbrættet som en matrix.
    string msgWelcome = "Din modstander {randomBotName} har udfordret dig. Tryk ENTER for at få dine brikker...\n";
    string msgTokenX = "Du har fået kryds (X) derfor begynder du. Tryk ENTER for at begynde spillet...";
    string msgTokenO = "Du har fået bolle (O) derfor begynder {randomBotName}. Tryk ENTER for at begynde spillet...";
    int botTokenCount = 0;
    int playerTokenCount = 0;
    bool winCondition = false;
    (int x, int y) removedToken = (-1, -1); // Bruger Tuple frem for Array da det gør min boolean condition i botPlaceToken() mere læsbar.

    // Velkomstbesked
    gameHeader();
    Console.WriteLine(msgWelcome);

    // Tildeling af spilbrik 
    Console.ReadKey();
    gameHeader();
    Console.WriteLine(
        (playerToken) switch
        {
          "X" => msgTokenX,
          "O" => msgTokenO,
          _ => "Ugyldig brik"
        }
    );

    //Renderer et tomt startbræt
    Console.ReadKey();
    gameHeader();
    updateBoard();

    RunXO();

    while (true)
    {
        if (PlayAgain() == 0) break; // user selected N
        else { XO(); }
    }
    ShowMainMenu();

    void RunXO()
    {
        while (!winCondition)
        {
            if (playerToken == "X")
            {
                playerTurn();
                if (!winCondition) { botTurn(); }
            }
            else
            {
                botTurn();
                if (!winCondition) { playerTurn(); }
            }
        }

        Console.ReadKey();
    }

    void botTurn()
    {
      if (botTokenCount < 3)
      {
        botPlaceToken();
      }
      else
      {
        botRemoveToken();
        botPlaceToken();
      }

    }

    void botRemoveToken()
    {
      int x = rnd.Next(0, 3);
      int y = rnd.Next(0, 3);

      if (gameBoard[x, y] == botToken)
      {
        gameBoard[x, y] = " ";
        botTokenCount--;
        removedToken = (x, y);
      }
      else
      {
        botRemoveToken();
      }
    }

    void botPlaceToken()
    {
      int x = rnd.Next(0, 3);
      int y = rnd.Next(0, 3);

      bool isEmpty = gameBoard[x, y] == " ";
      bool newPosition = removedToken.x != x && removedToken.y != y;

        if (isEmpty && newPosition) 
        {
            gameBoard[x, y] = botToken;
            botTokenCount++;
            updateBoard();
            endGame();
        }
        else
        {
            botPlaceToken();
        }
    }

    void playerTurn()
    {
      if (playerTokenCount < 3)
      {
        playerPlaceToken();
      }
      else
      {
        playerRemoveToken();
        playerPlaceToken();
      }
    }

    void playerRemoveToken()
    {
      userInput = Console.ReadLine();
      int x = userInput[0] - 'a';
      int y = userInput.Length > 1 ? int.Parse(userInput[1].ToString()) - 1 : -1;

      if (userInput == "q")
      {
        Environment.Exit(0);
      }
      else if (gameBoard[x, y] == playerToken)
      {
        gameBoard[x, y] = " ";
        playerTokenCount--;
        removedToken = (x, y);
      }
      else
      {
        botRemoveToken();
      }
    }

    void playerPlaceToken()
    {
      userInput = Console.ReadLine();
      int x = userInput[0] - 'a';
      int y = userInput.Length > 1 ? int.Parse(userInput[1].ToString()) - 1 : -1;

        if (userInput == "q")
        {
            Environment.Exit(0);
        }
        else
        {
            gameBoard[x, y] = playerToken;
            playerTokenCount++;
            updateBoard();
            endGame();
        }
    }

    void updateBoard() // Renderer spilbrættets data i et indrammet spilbræt.
    {
      gameHeader();

      //Symboler brugt til at bygge spilbrætrammen.
      string borderPipe = "| ";
      string borderEmDash = "— ";
      string borderSpace = "  ";

      // Indsætter spilbrættet i en ramme.
      string[,] gameBorders =
      {
                { borderSpace, borderSpace, "1 ", borderSpace, "2 ", borderSpace, "3 ", borderSpace },
                { borderSpace, borderSpace, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderSpace },
                { "a ", borderPipe, gameBoard[0, 0] + " ", borderPipe, gameBoard[0, 1] + " ", borderPipe, gameBoard[0, 2] + " ", borderPipe },
                { borderSpace, borderPipe, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderPipe },
                { "b ", borderPipe, gameBoard[1, 0] + " ", borderPipe, gameBoard[1, 1] + " ", borderPipe, gameBoard[1, 2] + " ", borderPipe },
                { borderSpace, borderPipe, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderPipe },
                { "c ", borderPipe, gameBoard[2, 0] + " ", borderPipe, gameBoard[2, 1] + " ", borderPipe, gameBoard[2, 2] + " ", borderPipe },
                { borderSpace, borderSpace, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderEmDash, borderSpace }
            };

      //Printer spilbræt og ramme ud til konsollen.
      for (int i = 0; i < gameBorders.GetLength(0); i++)
      {
        for (int j = 0; j < gameBorders.GetLength(1); j++)
        {
          Console.Write(gameBorders[i, j]);
        }
          ;
        Console.WriteLine();
      }
      ;
    }

    void gameHeader() //Renderer spiltitlen
    {
      Console.Clear();
      Console.WriteLine("""
            XOXOXOXOXOXOXOXOXOXO

               KRYDS & BOLLE!

            XOXOXOXOXOXOXOXOXOXO

            """);
    }
    ;

    void endGame()
        {
            string a1 = gameBoard[0, 0], a2 = gameBoard[0, 1], a3 = gameBoard[0, 2], b1 = gameBoard[1, 0], b2 = gameBoard[1, 1], b3 = gameBoard[1, 2], c1 = gameBoard[2, 0], c2 = gameBoard[2, 1], c3 = gameBoard[2, 2];
            var lines = new Dictionary<string, string[]>
            {
                { "a1:a3", new string[] { a1, a2, a3 } },
                { "b1:b3", new string[] { b1, b2, b3 } },
                { "c1:c3", new string[] { c1, c2, c3 } },
                { "a1:c1", new string[] { a1, b1, c1 } },
                { "a2:c2", new string[] { a2, b2, c2 } },
                { "a3:c3", new string[] { a3, b3, c3 } },
                { "a1:c3", new string[] { a1, b2, c3 } },
                { "a3:c1", new string[] { a3, b2, c1 } }
            };

            foreach (var line in lines)
            {
                int countX = 0;
                int countO = 0;

                foreach (string position in line.Value)
                {
                    switch (position)
                    {
                        case "X": countX++;
                            break;
                        case "O": countO++;
                            break;
                        default: break;

                    }
                }

                if (countX > 2)
                {
                    winCondition = true;
                    break;
                } 
                else if (countO > 2)
                {
                    winCondition = true;
                    break;
                }
            }
        }
  }


  // ==================================================
  // Number guessing battle??
  // Af: Nicolaj
  // ==================================================

  static void PlayNumberGame()
  {
    // TODO alternate who starts each turn
    // TODO powerups? critical hits? damage multipliers?
    NumberGameIntro();
    while (true)
    {
      NumberGame();
      if (PlayAgain() == 0) break; // user selected N
    }
    ShowMainMenu();
  }

  static void NumberGame()
  {
    Console.Clear();

    int numPlayers = GetNumPlayers();
    string[] playerNames = GetPlayerNames(numPlayers);

    int healthPlayerOne = 100;
    int healthPlayerTwo = 100;

    while (healthPlayerOne > 0 && healthPlayerTwo > 0)
    {
      PrintGame(playerNames, healthPlayerOne, healthPlayerTwo);
      int[] guesses = GetUserGuess(playerNames, numPlayers);
      if (playerNames[1] == "Computer")
      {
        Console.WriteLine($"{playerNames[1]} guesses {guesses[1]}");
      }
      Thread.Sleep(2000); // delay before revealing answer
      ApplyDamage(playerNames, guesses, ref healthPlayerOne, ref healthPlayerTwo);
      Console.ReadKey(); // wait for user input to start next round
    }
    PrintWinner(playerNames, healthPlayerOne, healthPlayerTwo);
  }

  static void PrintWinner(string[] playerNames, int healthPlayerOne, int healthPlayerTwo)
  {
    Console.Clear();
    if (healthPlayerOne > healthPlayerTwo)
    {
      Console.WriteLine($"{playerNames[0]} won!");
    }
    else
    {
      Console.WriteLine($"{playerNames[1]} won!");
    }
    Console.ReadKey();
  }

  /// <summary>
  /// Prompts user for number of players
  /// </summary>
  /// <returns> int 1 || int 2 </returns>
  static int GetNumPlayers()
  {
    while (true)
    {
      Console.Write("Enter number of players (1-2): ");
      string input = Console.ReadLine() ?? "";
      if (!int.TryParse(input, out int numPlayers)) continue;
      if (numPlayers is 1 or 2) return numPlayers;
    }
  }

  /// <summary>
  /// Prompts user for player names
  /// If number of players is 1, "Computer" is added as second player
  /// </summary>
  /// <returns> Array of strings of names of the players </returns>
  static string[] GetPlayerNames(int numPlayers)
  {
    string[] players = new string[2];
    for (int i = 0; i < numPlayers; i++)
    {
      string input;
      do
      {
        Console.Write($"Name of player {i + 1}: ");
        input = Console.ReadLine() ?? "";
      }
      while (string.IsNullOrWhiteSpace(input));
      players[i] = input;
    }
    if (numPlayers == 1) players[1] = "Computer";
    return players;
  }

  /// <summary>
  /// Prompts users for guesses
  /// If number of players is 1, second guess is a random integer between 1 and 100
  /// </summary>
  /// <returns> array of guesses </returns>
  static int[] GetUserGuess(string[] playerNames, int numPlayers)
  {
    int[] guesses = new int[2];
    Console.WriteLine("Guess a number between 1-100");
    for (int i = 0; i < numPlayers; i++)
    {
      int lengthUserGuess = 0;
      while (true)
      {
        Console.SetCursorPosition(0, 17);
        Console.Write($"{playerNames[i]}: {new string(' ', lengthUserGuess)}"); // spaces to cover incorrect answer
        Console.SetCursorPosition(playerNames[i].Length + 2, 17); // sets cursor after player name:
        string guess = Console.ReadLine() ?? "";
        lengthUserGuess = guess.Length;
        if (!int.TryParse(guess, out guesses[i])) continue;
        if (guesses[i] > 0 && guesses[i] <= 100) break;
      }
    }
    if (numPlayers == 1) guesses[1] = rnd.Next(1, 101); // assign the computer a guess
    return guesses;
  }

  /// <summary>
  /// Prints the game graphics to console
  /// </summary>
  static void PrintGame(string[] playerNames, int healthPlayerOne, int healthPlayerTwo)
  {
    Console.Clear();
    // robot ascii art from https://www.asciiart.eu/electronics/robots
    // alien ascii art from https://www.asciiart.eu/space/aliens
    // combined using claude.ai
    string[] robotVsAlien = [
"                                                    o   o    ",
            "                                                     )-(     ",
            "                                                    (O O)    ",
            "       __                                            \\=/     ",
            "   _  |@@|                                          .-\"-.    ",
            "  / \\ \\--/ __                                      //\\ /\\\\  ",
            "  ) O|----|  |   __                              _// / \\ \\\\_",
            " / / \\ }{ /\\ )_ / _\\                            =./ {,-.} \\.=",
            " )/  /\\__/\\ \\__O (__)                               || ||   ",
            "|/  (--/\\--)    \\__/                               || ||   ",
            "/   _)(  )(_                                      __|| ||__ ",
            "   `---''---`                                    `---\" \"---'"
        ];
    foreach (string line in robotVsAlien)
    {
      Console.WriteLine(line);
    }
    Console.WriteLine($"     HP [{new string('#', healthPlayerOne / 10),-10}]                            HP [{new string('#', healthPlayerTwo / 10),-10}]");
    Console.WriteLine($"             {playerNames[0]}                                   {playerNames[1]}");
    Console.WriteLine();
  }

  /// <summary>
  /// Calculate distance between answer and guesses
  /// Decrements player health by random integer between 10 and 20 for player who was furthest from answer
  /// </summary>
  static void ApplyDamage(string[] playerNames, int[] guesses, ref int healthPlayerOne, ref int healthPlayerTwo)
  {
    int answer = rnd.Next(1, 101);
    Console.WriteLine($"The answer was {answer}");

    int damage = rnd.Next(10, 20);
    // calculate guess distance from answer
    int player1Distance = Math.Abs(guesses[0] - answer);
    int player2Distance = Math.Abs(guesses[1] - answer);
    if (player1Distance < player2Distance)
    {
      Console.WriteLine($"{playerNames[0]} was closer and does {damage} damage!");
      healthPlayerTwo -= damage;
    }
    else if (player1Distance > player2Distance)
    {
      Console.WriteLine($"{playerNames[1]} was closer and does {damage} damage!");
      healthPlayerOne -= damage;
    }
    else
    {
      Console.WriteLine("Tie!");
    }
  }

  static void NumberGameIntro()
  {
    Console.Clear();
    Console.WriteLine("╔════════════════════════════════╗");
    Console.WriteLine("║        NUMBER BATTLE!          ║");
    Console.WriteLine("║                                ║");
    Console.WriteLine("║ Guess closest to the random    ║");
    Console.WriteLine("║ number to deal damage!         ║");
    Console.WriteLine("╚════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
  }

  // ==================================================
  // Conway's Game of Life
  // Af: Nicolaj
  // ==================================================

  static void PlayGameOfLife()
  {
    GameOfLifeIntro();
    while (true)
    {
      GameOfLife();
      if (PlayAgain() == 0) break; // user selected N
    }
    ShowMainMenu();
  }

  static void GameOfLife()
  {
    Console.Clear();
    Console.CursorVisible = false;
    int arrayHeight = 20;
    int arrayWidth = 40;

    string[,] cellArray = SelectPattern(arrayHeight, arrayWidth);
    while (!Console.KeyAvailable)
    {
      PrintCellArray(cellArray);
      string[,] newArray = InitializeArray(arrayHeight, arrayWidth);
      for (int i = 0; i < cellArray.GetLength(0); i++)
      {
        for (int j = 0; j < cellArray.GetLength(1); j++)
        {
          int neighbors = CountNeighbors(i, j, cellArray);
          newArray[i, j] = ApplyGameOfLifeRules(cellArray[i, j], neighbors);
        }
      }
      cellArray = newArray;
      Console.WriteLine("\nPress any key to stop");
      Thread.Sleep(200);
    }
    if (Console.KeyAvailable)
    {
      Console.ReadKey(true); // consume key
      Console.CursorVisible = true;
    }
  }

  static void GameOfLifeIntro()
  {
    Console.Clear();
    Console.WriteLine("╔════════════════════════════════╗");
    Console.WriteLine("║          GAME OF LIFE!         ║");
    Console.WriteLine("║                                ║");
    Console.WriteLine("║ Zero-player evolutionary game! ║");
    Console.WriteLine("║ Watch cells live, die, and     ║");
    Console.WriteLine("║ evolve across generations.     ║");
    Console.WriteLine("╚════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
  }

  /// <summary>
  /// Prints the cell array to console starting at Position 0, 0
  /// </summary>
  static void PrintCellArray(string[,] cellArray)
  {
    Console.SetCursorPosition(0, 0);
    for (int i = 0; i < cellArray.GetLength(0); i++)
    {
      for (int j = 0; j < cellArray.GetLength(1); j++)
      {
        Console.Write(cellArray[i, j]);
      }
      Console.WriteLine();
    }
  }

  /// <summary>
  /// Counts number of live neighbors for cell[row, col]
  /// </summary>
  static int CountNeighbors(int row, int col, string[,] cellArray)
  {
    int liveNeighbors = 0;
    for (int i = -1; i <= 1; i++)
    {
      for (int j = -1; j <= 1; j++)
      {
        if (i == 0 && j == 0) continue;
        int newRow = row + i;
        int newCol = col + j;
        if (newRow < 0 || newRow >= cellArray.GetLength(0) ||
            newCol < 0 || newCol >= cellArray.GetLength(1)) continue;
        if (cellArray[newRow, newCol] == "██")
        {
          liveNeighbors++;
        }
      }
    }
    return liveNeighbors;
  }

  /// <summary>
  /// Creates array of size arrayHeight, arrayWidth and fills with "  "
  /// </summary>
  /// <returns> returns array[arrayHeight, arrayWidth] </returns>
  static string[,] InitializeArray(int arrayHeight, int arrayWidth)
  {
    string[,] cellArray = new string[arrayHeight, arrayWidth];
    for (int i = 0; i < arrayHeight; i++)
    {
      for (int j = 0; j < arrayWidth; j++)
      {
        cellArray[i, j] = "  ";
      }
    }
    return cellArray;
  }

  /// <summary>
  /// Let's user select a pattern from a predefined list of 4 common patterns
  /// Prints the pattern to the console so user can preview it before selecting
  /// </summary>
  /// <returns> An array with the selected pattern </returns>
  static string[,] SelectPattern(int arrayHeight, int arrayWidth)
  {
    string[,] cellArray = InitializeArray(arrayHeight, arrayWidth);
    string[] patterns = ["Toad", "Glider", "Die Hard", "Lightweight Spaceship"];
    int currentIndex = 0;
    while (true)
    {
      switch (currentIndex)
      {
        case 0: // toad pattern
          cellArray[9, 19] = "██";
          cellArray[9, 20] = "██";
          cellArray[9, 21] = "██";
          cellArray[10, 18] = "██";
          cellArray[10, 19] = "██";
          cellArray[10, 20] = "██";
          break;
        case 1: // glider pattern
          cellArray[2, 4] = "██";
          cellArray[3, 5] = "██";
          cellArray[4, 3] = "██";
          cellArray[4, 4] = "██";
          cellArray[4, 5] = "██";
          break;
        case 2: // Die hard pattern
          cellArray[10, 21] = "██";
          cellArray[11, 15] = "██";
          cellArray[11, 16] = "██";
          cellArray[12, 16] = "██";
          cellArray[12, 20] = "██";
          cellArray[12, 21] = "██";
          cellArray[12, 22] = "██";
          break;
        case 3: // Lightweight Spaceship pattern
          cellArray[8, 11] = "██";
          cellArray[8, 12] = "██";
          cellArray[8, 13] = "██";
          cellArray[8, 14] = "██";
          cellArray[9, 10] = "██";
          cellArray[9, 14] = "██";
          cellArray[10, 14] = "██";
          cellArray[11, 10] = "██";
          cellArray[11, 13] = "██";
          break;
        default:
          break;
      }
      PrintCellArray(cellArray);
      PrintMenu(patterns, currentIndex);
      int result = MenuSelect(patterns, currentIndex);
      if (result == -1) break; // user made selection, exit menu loop
      currentIndex = result;
      cellArray = InitializeArray(arrayHeight, arrayWidth);
    }
    Console.Clear();
    return cellArray;
  }

  static string ApplyGameOfLifeRules(string currentCell, int neighbors)
  {
    if (currentCell == "  " && neighbors == 3) return "██"; // becomes live if dead and 3 neighbors
    if (currentCell == "██")
      return neighbors switch
      {
        < 2 => "  ", // dies if less than 2 neighbors
        > 3 => "  ", // dies if more than 3 neighbors
        _ => "██", // lives to next generation if 2-3 neighbors
      };
    return currentCell;
  }
}
