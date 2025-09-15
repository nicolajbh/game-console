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
      Console.ReadKey();
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

  static int GetRandomNumber(int i, int j)
  {
    Random rnd = new();
    return rnd.Next(i, j);
  }

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

  static void PrintWelcomeScreen()
  {
    Console.Clear();
    Console.WriteLine("Velkommen til Matias og Nicolaj's spillekonsol");
    Console.ReadKey();
  }

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

  static void PrintGame(string[] playerNames, int healthPlayerOne, int healthPlayerTwo)
  {
    Console.Clear();
    Console.WriteLine($"Health {playerNames[0]}: {healthPlayerOne}");
    Console.WriteLine($"Health {playerNames[1]}: {healthPlayerTwo}");
  }

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
