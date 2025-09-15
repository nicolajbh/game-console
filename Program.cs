internal class Program
{
  public static void Main(string[] args)
  {
    // Menu for selecting games
    var gameMenu = new Dictionary<string, Action>
    {
      { "Number battle", PlayNumberGame},
      { "Exit", () => Environment.Exit(0) }
    };
    var menuTitles = gameMenu.Keys.ToArray();
    int index = 0;
    PrintWelcomeScreen();
    while (true)
    {
      PrintMenu(menuTitles, index);
      int result = MenuSelect(menuTitles, index);
      if (result == -1) break;
      index = result;
    }
    gameMenu[menuTitles[index]]();
  }


  static void PlayNumberGame()
  {
    // TODO add ascii art
    // TODO add difficulty - tighten rnd params
    Console.Clear();
    Console.WriteLine("Starting number game...");
    Thread.Sleep(2000);
    Console.Clear();

    // game set up variables
    int numPlayers = GetNumPlayers();
    string[] playerNames = GetPlayerNames(numPlayers);

    int healthPlayerOne = 100;
    int healthPlayerTwo = 100;

    // gameplay loop
    while (healthPlayerOne <= 0 || healthPlayerTwo <= 0)
    {
      PrintGame(playerNames, healthPlayerOne, healthPlayerTwo);
      int[] guesses = GetUserGuess(playerNames, numPlayers);
      if (playerNames[1] == "Computer")
      {
        Console.WriteLine($"{playerNames[1]} guesses {guesses[1]}");
      }
      Thread.Sleep(2000);
      ApplyDamage(playerNames, guesses, ref healthPlayerOne, ref healthPlayerTwo);
      Console.ReadKey(); // wait for user input to start next round
    }
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
      if (int.TryParse(input, out int numPlayers))
      {
        if (numPlayers == 1 || numPlayers == 2) return numPlayers;
      }
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
      while (input == "");
      players[i] = input;
    }
    if (numPlayers == 1) players[1] = "Computer";
    return players;
  }

  /// <summary>
  /// Generates random integer 
  /// </summary>
  /// <returns> int between i and j </returns>
  static int GetRandomNumber(int i, int j)
  {
    Random rnd = new();
    return rnd.Next(i, j);
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
    if (numPlayers == 1) guesses[1] = GetRandomNumber(1, 100);
    return guesses;
  }

  /// <summary>
  /// Outputs welcome graphics to console
  /// </summary>
  static void PrintWelcomeScreen()
  {
    Console.Clear();
    Console.WriteLine("Velkommen til Matias og Nicolaj's spillekonsol");
    Console.ReadKey();
  }

  /// <summary>
  /// Loops through menu titles and prints to console
  /// Adds arrow to item currently selected from menu
  /// </summary>
  static void PrintMenu(string[] menuTitles, int currentIndex)
  {
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
    }
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
    Console.WriteLine($"Health {playerNames[0]}: {healthPlayerOne}");
    Console.WriteLine($"Health {playerNames[1]}: {healthPlayerTwo}");
  }
  /// <summary>
  /// Calculate distance between answer and guesses
  /// Decrements player health by random integer between 10 and 20 for player who was furthest from answer
  /// </summary>
  static void ApplyDamage(string[] playerNames, int[] guesses, ref int healthPlayerOne, ref int healthPlayerTwo)
  {
    int answer = GetRandomNumber(1, 100);
    Console.WriteLine($"The answer was {answer}");

    int damage = GetRandomNumber(10, 20);
    if (Math.Abs(guesses[0] - answer) < Math.Abs(guesses[1] - answer))
    {
      Console.WriteLine($"{playerNames[0]} was closer and does {damage} damage!");
      healthPlayerTwo -= damage;
    }
    else if (Math.Abs(guesses[0] - answer) > Math.Abs(guesses[1] - answer))
    {
      Console.WriteLine($"{playerNames[1]} was closer and does {damage} damage!");
      healthPlayerOne -= damage;
    }
    else
    {
      Console.WriteLine("Tie!");
    }
  }

  // static void PlayAgain()
  // {
  //
  // }
}
