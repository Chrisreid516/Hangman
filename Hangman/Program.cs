using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Hangman
{// *************************************************************
    // App: Hangman
    // Author: Reid, Chris
    // Date: 04/28/19
    // 
    class Program
    {
        // Used for selecting category of words to choose from
        public enum Category
        {
            INVALID,
            MOVIES,
            BOOKS,
            GAMES,
            E //used to exit selecting a category
        }

        static void Main(string[] args)
        {
            DisplayWelcomeScreen();
            DisplayMainMenu();
            DisplayClosingScreen();
        }

        static void DisplayClosingScreen()
        {
            DisplayHeader("Thanks for Playing!");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void DisplayMainMenu()
        {
            string menuChoice;
            bool exiting = false;

            while (!exiting)
            {
                menuChoice = DisplayMenuSelectionScreen();
                exiting = ProcessMenu(exiting, menuChoice);               
            }
        }

        static string DisplayMenuSelectionScreen()
        {
            string menuChoice;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Main Menu");
            Console.WriteLine();
            Console.WriteLine("\t1) Instructions");
            Console.WriteLine("\t2) Play Game");
            Console.WriteLine("\tE) Exit");
            Console.WriteLine();
            Console.Write("Enter Choice: ");

            menuChoice = Console.ReadKey().KeyChar.ToString();
            return menuChoice;
        }

        static bool ProcessMenu(bool exiting, string menuChoice)
        {
            string categoryAsString = "";
            Category category = Category.INVALID;

            switch (menuChoice)
            {
                case "1":
                    DisplayInstructions();
                    break;
                case "2":
                    category = SelectCategory(categoryAsString, category);

                    // if player selects e on the category screen then the PlayGame method doesn't execute
                    if (category == Category.E)
                    {
                        break;
                    }

                    PlayGame(category);
                    break;
                case "e":
                case "E":
                    exiting = true;
                    break;
                default:
                    Console.WriteLine("Please select a valid option.");
                    DisplayContinuePrompt();
                    break;
            }

            return exiting;
        }

        static Category SelectCategory(string categoryAsString, Category category)
        {
            bool validResponse = false;

            while (!validResponse)
            {
                DisplayCategorySelections();

                categoryAsString = Console.ReadKey().KeyChar.ToString().ToUpper();
                if (!Enum.TryParse(categoryAsString, out category))
                {
                    Console.WriteLine("please enter a valid category");
                }

                // This is so the default Enum selection isn't accidentally selected
                else if (category == Category.INVALID)
                {
                    Console.WriteLine("please enter a valid category");
                }

                else
                {
                    validResponse = true;
                }
            }
          
            return category;
        }

        static void DisplayCategorySelections()
        {
            DisplayHeader("Select Category");

            Console.WriteLine("Select a Category (movies, books, or games): ");
            Console.WriteLine("\t1) Movies");
            Console.WriteLine("\t2) Books");
            Console.WriteLine("\t3) Games");
            Console.WriteLine("\tE) Exit");
            Console.WriteLine();
            Console.Write("Enter Choice: ");
        }

        static void PlayGame(Category category)
        {
            // variables            
            bool gameOver = false;
            Shapes shape = new Shapes();
            List<string> availableLetters = null; 
            List<string> missedLetters = new List<string>();
            List<string> movies = null;
            List<string> books = null;
            List<string> games = null;
            string selectedLetter;
            string wordToGuess;
            string wordToGuessTemp = ""; //There is a temp because letters have to removed from it
            string hiddenWordToGuess; //Used to hide answer from the user

            //Fill lists with values
            availableLetters = FillAvailableLettersList(availableLetters);
            movies = FillMoviesList(movies);
            books = FillBooksList(books);
            games = FillGamesList(games);

            //Select word from selected category list 
            wordToGuessTemp = SelectWordToGuess(category, movies, books, games);
            wordToGuess = wordToGuessTemp;
            hiddenWordToGuess = Regex.Replace(wordToGuess, "[a-z]", "*");

            //Main game w/ loop
            while (!gameOver)
            {
                DisplayHeader("Main Game");            

                shape.DrawPole();
                DisplayWordToGuess(wordToGuess, availableLetters, hiddenWordToGuess);
                DisplayCategory(category);
                Console.SetCursorPosition(30, 9);
                Console.WriteLine("Select an available letter:");
                DisplayAvailableLettersColumn(availableLetters);

                gameOver = DisplayMissedLettersColumn(missedLetters, shape);

                if (hiddenWordToGuess == wordToGuess)
                {
                    gameOver = true;
                }

                if (gameOver)
                {
                    WinningOrLosingMessage(missedLetters, wordToGuess, hiddenWordToGuess);
                    break;
                }

                Console.SetCursorPosition(30, 10);
                selectedLetter = Console.ReadKey().KeyChar.ToString().ToLower();
                hiddenWordToGuess = UpdateLists(availableLetters, wordToGuessTemp, selectedLetter, missedLetters, hiddenWordToGuess);
            }         
        }

        // Selects which message should be displayed
        static void WinningOrLosingMessage(List<string> missedLetters, string wordToGuess, string hiddenWordToGuess)
        {
            if (missedLetters.Count() == 6)
            {
                LosingDisplayMessage(wordToGuess);
            }

            else if (hiddenWordToGuess == wordToGuess)
            {
                WinningDisplayMessage(wordToGuess);
            }
        }

        static void DisplayCategory(Category category)
        {
            Console.SetCursorPosition(30, 6);
            Console.Write($"Category: {category.ToString().ToLower()}");
        }

        // picks list based on which category was selected and then randomly selects a word from that list
        static string SelectWordToGuess(Category category, List<string> movies, List<string> books, List<string> games)
        {
            string wordToGuessTemp = "";
            Random rand = new Random();
            int index;

            switch (category)
            {
                case Category.MOVIES:
                    index = rand.Next(movies.Count());
                    wordToGuessTemp = movies[index];
                    break;
                case Category.BOOKS:
                    index = rand.Next(books.Count());
                    wordToGuessTemp = books[index];
                    break;
                case Category.GAMES:
                    index = rand.Next(games.Count());
                    wordToGuessTemp = games[index];
                    break;
            }

            return wordToGuessTemp;
        }

        static List<string> FillGamesList(List<string> games)
        {
            games = new List<string>()
            {
                "star wars: battlefront", "uncharted 4: a thiefs end", "call of duty: modern warfare", "portal", "rocket league"
            };

            return games;
        }

        static List<string> FillBooksList(List<string> books)
        {
            books = new List<string>()
            {
                "harry potter and the chamber of secrets", "the alchemist", "the fault in our stars",
                "the pilgrims progress", "the great gatsby"
            };

            return books;
        }

        static List<string> FillMoviesList(List<string> movies)
        {
            movies = new List<string>()
            {
                "star wars: a new hope", "avengers: endgame", "inception", "batman begins", "saving private ryan"
            };

            return movies;
        }

        static void WinningDisplayMessage(string wordToGuess)
        {
            DisplayHeader("You Win!!!");
            Console.WriteLine($"Correct Word: {wordToGuess}");
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        static void DisplayWordToGuess(string wordToGuess, List<string> availableLetters, string hiddenWordToGuess)
        {
            Console.SetCursorPosition(30, 3);
            Console.Write("Word to Guess:");
            Console.SetCursorPosition(30, 4);
            Console.Write(hiddenWordToGuess);
        }

        // Updates list to keep the game current
        static string UpdateLists(List<string> availableLetters, string wordToGuessTemp, string selectedLetter, List<string> missedLetters, string hiddenWordToGuess)
        {
            if (availableLetters.Contains(selectedLetter))
            {
                if (wordToGuessTemp.Contains(selectedLetter))
                {
                    bool lettersRemaining= true;

                    // checks to see if there is more than one instance of a letter and replaces a letter with a * if has
                    // been selected already
                    while (lettersRemaining)
                    {
                        lettersRemaining = false;

                        hiddenWordToGuess = hiddenWordToGuess.Remove(wordToGuessTemp.IndexOf(selectedLetter), 1)
                            .Insert(wordToGuessTemp.IndexOf(selectedLetter), selectedLetter);

                        wordToGuessTemp = wordToGuessTemp.Remove(wordToGuessTemp.IndexOf(selectedLetter), 1)
                            .Insert(wordToGuessTemp.IndexOf(selectedLetter), "*");

                        if (wordToGuessTemp.Contains(selectedLetter))
                        {
                            lettersRemaining = true;
                        }
                    }

                    availableLetters.Remove(selectedLetter);
                }

                else
                {
                    missedLetters.Add(selectedLetter);
                    availableLetters.Remove(selectedLetter);
                }
            }

            else
            {
                InvalidLetterDisplayMessage();
            }

            return hiddenWordToGuess;
        }

        // message displays if a letter from the alphabet is not chosen or if has already been used 
        static void InvalidLetterDisplayMessage()
        {
            Console.SetCursorPosition(30, 10);
            Console.WriteLine("Select a valid letter from a-z.");
            Console.SetCursorPosition(30, 11);
            Console.WriteLine("Press any key to continue.");
            Console.SetCursorPosition(30, 12);
            Console.ReadKey();
        }

        static void LosingDisplayMessage(string wordToGuess)
        {
            Console.SetCursorPosition(30, 10);
            Console.WriteLine("You Lose...");
            Console.SetCursorPosition(30, 11);
            Console.WriteLine($"Correct Word: {wordToGuess}");
            Console.SetCursorPosition(30, 12);
            Console.WriteLine("Press any key to exit.");
            Console.SetCursorPosition(30, 13);
            Console.ReadKey();
        }

        // this method and helper methods is lengthy because the cursor has to be reset for every letter
        static bool DisplayMissedLettersColumn(List<string> missedLetters, Shapes shape)
        {
            bool gameOver = false;
            Console.SetCursorPosition(25, 16);
            Console.WriteLine("Missed Letters:\n");

            switch (missedLetters.Count())
            {
                case 1:
                    DisplayOneWrongLetter(missedLetters, shape);
                    break;
                case 2:
                    DisplayTwoWrongLetters(missedLetters, shape);
                    break;
                case 3:
                    DisplayThreeWrongLetters(missedLetters, shape);
                    break;
                case 4:
                    DisplayFourWrongLetters(missedLetters, shape);               
                    break;
                case 5:
                    DisplayFiveWrongLetters(missedLetters, shape);
                    break;
                case 6:
                    DisplaySixWrongLetters(missedLetters, shape);
                    gameOver = true; // if 6 wrong letters happen, the player has lost
                    break;
            }

            return gameOver;
        }

        static void DisplaySixWrongLetters(List<string> missedLetters, Shapes shape)
        {
            DisplayFiveWrongLetters(missedLetters, shape);
            shape.DrawLegTwo();
            Console.SetCursorPosition(31, 19);
            Console.Write(missedLetters[5]);
        }

        static void DisplayFiveWrongLetters(List<string> missedLetters, Shapes shape)
        {
            DisplayFourWrongLetters(missedLetters, shape);
            shape.DrawLegOne();
            Console.SetCursorPosition(28, 19);
            Console.Write(missedLetters[4]);
        }

        static void DisplayFourWrongLetters(List<string> missedLetters, Shapes shape)
        {
            DisplayThreeWrongLetters(missedLetters, shape);
            shape.DrawArmTwo();
            Console.SetCursorPosition(25, 19);
            Console.Write(missedLetters[3]);
        }

        static void DisplayThreeWrongLetters(List<string> missedLetters, Shapes shape)
        {
            DisplayTwoWrongLetters(missedLetters, shape);
            shape.DrawArmOne();
            Console.SetCursorPosition(31, 18);
            Console.Write(missedLetters[2]);
        }

        static void DisplayTwoWrongLetters(List<string> missedLetters, Shapes shape)
        {
            DisplayOneWrongLetter(missedLetters, shape);
            shape.DrawBody();
            Console.SetCursorPosition(28, 18);
            Console.Write(missedLetters[1]);
        }

        static void DisplayOneWrongLetter(List<string> missedLetters, Shapes shape)
        {
            shape.DrawHead();
            Console.SetCursorPosition(25, 18);
            Console.Write(missedLetters[0]);
        }

        // Writes each letter to the console and goes to the next line after every 3 letter
        static void DisplayAvailableLettersColumn(List<string> availableLetters)
        {
            Console.SetCursorPosition(0, 16);
            Console.WriteLine("Available Letters:\n");
            int counter = 0;

            foreach (string letter in availableLetters)
            {
                Console.Write(letter + "  ");
                ++counter;

                if (counter == 3)
                {
                    Console.WriteLine();
                    counter = 0;
                }
            }
        }

        static List<string> FillAvailableLettersList(List<string> availableLetters)
        {
            availableLetters = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
            };

            return availableLetters;
        }

        static void DisplayInstructions()
        {
            DisplayHeader("Instructions");

            Console.WriteLine("This is a basic game of Hangman.");
            Console.WriteLine();
            Console.WriteLine("Try to guess the correct word. If you guess the word correctly, you win!");
            Console.WriteLine("If you incorrectly guess too many letters, you will lose...");
            DisplayContinuePrompt();
        }

        #region HELPER  METHODS

        static void DisplayWelcomeScreen()
        {
            DisplayHeader("Hangman: An Application Created by Chris Reid");
            DisplayContinuePrompt();
        }

        static void DisplayHeader(string header)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + header);
            Console.WriteLine();
        }

        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        #endregion
    }

    // Class used to draw all of the hangman shapes (arms, legs, body, head, and pole)
    class Shapes
    {
        #region METHODS
        public void DrawPole()
        {
            Console.WriteLine("\t    ************");
            Console.WriteLine("\t    *          *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t    *");
            Console.WriteLine("\t**********");
        }

        public void DrawHead()
        {
            Console.SetCursorPosition(0, 4);
            Console.WriteLine("\t    *          *");
            Console.WriteLine("\t    *         * *");
            Console.WriteLine("\t    *        *   *");
            Console.WriteLine("\t    *         * *");
        }

        public void DrawBody()
        {
            Console.SetCursorPosition(0, 8);
            Console.WriteLine("\t    *          *");
            Console.WriteLine("\t    *          *");
            Console.WriteLine("\t    *          *");
        }

        public void DrawArmOne()
        {
            Console.SetCursorPosition(0, 8);
            Console.WriteLine("\t    *       *  *");
            Console.WriteLine("\t    *       ****");
            Console.WriteLine("\t    *          *");
        }

        public void DrawArmTwo()
        {
            Console.SetCursorPosition(0, 8);
            Console.WriteLine("\t    *       *  *  *");
            Console.WriteLine("\t    *       *******");
            Console.WriteLine("\t    *          *");
        }

        public void DrawLegOne()
        {
            Console.SetCursorPosition(0, 11);
            Console.WriteLine("\t    *         *");
            Console.WriteLine("\t    *        *");
        }

        public void DrawLegTwo()
        {
            Console.SetCursorPosition(0, 11);
            Console.WriteLine("\t    *         * *");
            Console.WriteLine("\t    *        *   *");
        }

        #endregion
    }
}
