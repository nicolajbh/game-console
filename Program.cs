using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;

internal class Program
{
  static Random rnd = new();

  public static void Main(string[] args)
  {
    var gameMenu = new Dictionary<string, Action>
    {
      { "Number Battle", PlayNumberGame},
      { "Kryds & Bolle", XO },
      { "Exit", () => Environment.Exit(0) }
    };
    var menuTitles = gameMenu.Keys.ToArray();
    int selectedIndex = 0;
    PrintWelcomeScreen();
    while (true)
    {
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

    // ==================================================
    // Kryds & Bolle
    // Af: Matias
    // ==================================================
    public static void XO()
    { 
        string playerToken; //Gemmer menneskespillerens type af spilbrik.
        string userInput = "";
        string[,] gameBoard = {{ " ", " ", " " },{ " ", " ", " " },{ " ", " ", " " }}; //Opretter spilbrættet som en matrix.
        string tokenX = "X";
        string tokenO = "O";
        string msgWelcome = "Din modstander {randomBotName} har udfordret dig. Tryk ENTER for at få dine brikker...\n";
        string msgTokenX = "Du har fået kryds (X) derfor begynder du. Tryk ENTER for at begynde spillet...";
        string msgTokenO = "Du har fået bolle (O) derfor begynder {randomBotName}. Tryk ENTER for at begynde spillet...";
        int x;
        int y;

        // Velkomstbesked
        gameHeader();
        Console.WriteLine(msgWelcome);

        // Tildeling af spilbrik 
        Console.ReadKey();
        gameHeader();
        Console.WriteLine(
            (playerToken = rnd.Next(0, 2) == 0 ? "X" : "O") switch
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

        while (userInput != "q")
        {
            userInput = Console.ReadLine();

            if (userInput == "q")
            {
                break;
            }
            else
            {
                x = userInput[0] - 'a';
                y = int.Parse(userInput[1].ToString()) - 1;
                gameBoard[x, y] = playerToken;
                updateBoard();
            }
        }

        void updateBoard() // Renderer spilbrættets data i en indrammet spilbræt.
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
            };
        }

        void gameHeader() //Renderer spiltitlen
        {
            Console.Clear();
            Console.WriteLine("""
            XOXOXOXOXOXOXOXOXOXO

               KRYDS & BOLLE!

            XOXOXOXOXOXOXOXOXOXO

            """);
        };
    }


  // ==================================================
  // Number guessing battle??
  // Af: Nicolaj
  // ==================================================

  static void PlayNumberGame()
  {
    // TODO add ascii art
    // TODO alternate who starts each turn
    // TODO powerups? critical hits? damage multipliers?
    PrintIntroScreen();
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
    Console.Clear();
    if (healthPlayerOne > healthPlayerTwo)
    {
      Console.WriteLine($"{playerNames[0]} won!");
    }
    else
    {
      Console.WriteLine($"{playerNames[1]} won!");
    }
    // TODO Add play again
    // PlayAgain();
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
      while (true)
      {
        Console.Write($"{playerNames[i]}: ");
        string guess = Console.ReadLine() ?? "";
        if (int.TryParse(guess, out guesses[i]))
        {
          break;
        }
      }
    }
    if (numPlayers == 1) guesses[1] = rnd.Next(1, 101); // assign the computer a guess
    return guesses;
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
    Console.Clear();
    for (int i = 0; i < menuTitles.Length; i++)
    {
      if (i == currentIndex)
      {
        Console.WriteLine($"> {menuTitles[i]}");
      }
      else
      {
        Console.WriteLine(menuTitles[i]);
      }
    }
    Console.WriteLine("\nUse ↑/↓ arrows to navigate, Enter to select");
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

  /// <summary>
  /// Prints the game graphics to console
  /// </summary>
  static void PrintGame(string[] playerNames, int healthPlayerOne, int healthPlayerTwo)
  {
    Console.Clear();
    // robot ascii art from https://www.asciiart.eu/electronics/robots
    // alien ascii art from https://www.asciiart.eu/space/aliens
    // combined using claude.ai
    string[] robotVsAlien = {
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
        };
    foreach (string line in robotVsAlien)
    {
      Console.WriteLine(line);
    }
    Console.WriteLine($"     HP [{new string('#', (int)healthPlayerOne / 10).PadRight(10)}]                            HP [{new string('#', (int)healthPlayerTwo / 10).PadRight(10)}]");
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

  static void PrintIntroScreen()
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
  // static void PlayAgain()
  // {
  //
  // }
  //
}
