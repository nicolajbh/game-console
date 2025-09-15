internal class Program
{
  private static void Main(string[] args)
  {
    // Menu for selecting games
    string[] games = ["Game 1", "Game 2", "Game 3"];
    Console.Clear();
    Console.WriteLine("Velkommen til Matias og Nicolaj's spillekonsol");
    Console.ReadKey();
    int index = 0;
    while (true)
    {
      Console.Clear();
      for (int i = 0; i < games.Length; i++)
      {
        if (i == index)
        {
          Console.WriteLine($"> {games[i]}");
        }
        else
        {
          Console.WriteLine(games[i]);
        }

      }
      switch (Console.ReadKey().Key)
      {
        case ConsoleKey.UpArrow: // up arrow
          index--;
          break;
        case ConsoleKey.DownArrow: // down arrow
          index++;
          break;
        case ConsoleKey.Enter:
          PlayNumberGame();
          break;
        default:
          Console.WriteLine("Invalid key");
          break;
      }
    }
  }

  static void PlayNumberGame()
  {
    // TODO add ascii art
    Console.Clear();
    Console.WriteLine("Starting number game...");
    Thread.Sleep(2000);
    Console.Clear();

    // TODO add second player functionality
    // TODO add difficulty
    string namePlayerOne = GetPlayerName(1);
    string namePlayerTwo = "Computer";

    int healthPlayerOne = 100;
    int healthPlayerTwo = 100;

    while (true)
    {
      if (healthPlayerOne <= 0 || healthPlayerTwo <= 0)
      {
        break;
      }
      Console.Clear();
      Console.WriteLine($"Health {namePlayerOne}: {healthPlayerOne}");
      Console.WriteLine($"Health {namePlayerTwo}: {healthPlayerTwo}");

      int answer = GetRandomNumber(1, 100);
      int damage = GetRandomNumber(10, 20);

      int guessPlayerOne = GetUserGuess();
      int guessPlayerTwo = GetRandomNumber(1, 100);
      Console.WriteLine($"Computer guesses {guessPlayerTwo}");
      Thread.Sleep(2000);
      Console.WriteLine($"The answer was {answer}.");

      if (Math.Abs(guessPlayerOne - answer) < Math.Abs(guessPlayerTwo - answer))
      {
        Console.WriteLine($"{namePlayerOne} was closer and does {damage} damage!");
        healthPlayerTwo -= damage;
      }
      else if (Math.Abs(guessPlayerOne - answer) > Math.Abs(guessPlayerTwo - answer))
      {
        Console.WriteLine($"{namePlayerTwo} was closer and does {damage} damage!");
        healthPlayerOne -= damage;
      }
      else
      {
        Console.WriteLine("Tie!");
      }
      Console.ReadKey();
      // TODO Add play again
    }
  }

  static string GetPlayerName(int i)
  {
    string playerName;
    do
    {
      Console.Write($"Name player {i}: ");
      playerName = Console.ReadLine() ?? "";
    }
    while (playerName == "");
    return playerName;
  }

  static int GetRandomNumber(int i, int j)
  {
    Random rnd = new();
    return rnd.Next(i, j);
  }

  static int GetUserGuess()
  {
    int userGuess;
    while (true)
    {
      Console.Write("Guess a number between 1-100: ");
      string userInput = Console.ReadLine() ?? "";
      if (int.TryParse(userInput, out userGuess))
      {
        break;
      }
    }
    return userGuess;
  }
}
