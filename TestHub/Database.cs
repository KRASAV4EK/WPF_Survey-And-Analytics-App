using Microsoft.Data.Sqlite;
using System.IO;

public static class Database
{
    private static readonly float TestProbability = 0.7f;
    private static readonly string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");
    public static readonly string ConnectionString = "Data Source=" + DatabasePath;
    private static readonly SqliteConnection connection = new(ConnectionString);
    private static readonly Random _random = new();

    public static void Initialize()
    {
        if (!File.Exists(DatabasePath))
        {
            Console.WriteLine("Database file not found. Creating a new one...\n");
            connection.Open();

            CreateTables();
            AddUsersToDatabase();
            AddTestsToDatabase();
            AddResultsToDatabase();

            Console.WriteLine("Database and tables created successfully.\n");
        }
        else
        {
            Console.WriteLine("Database file found.\n");
        }
    }
    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection(ConnectionString);
    }
    public static void TestConnection()
    {
        using var connection = GetConnection();
        Console.WriteLine("Database connection successful!\n");
    }

    public static void CreateTables()
    {
        string createTestsTable = @"
                CREATE TABLE IF NOT EXISTS tests (
                    TestId INTEGER PRIMARY KEY AUTOINCREMENT,
                    TestName TEXT NOT NULL
                )";

        string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Surname TEXT NOT NULL,
                    Email TEXT NOT NULL
                )";

        string createQuestionsTable = @"
                CREATE TABLE IF NOT EXISTS Questions (
                    QuestionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    TestId INTEGER NOT NULL,
                    QuestionText TEXT NOT NULL,
                    FOREIGN KEY (TestId) REFERENCES tests(TestId) ON DELETE CASCADE
                )";

        string createAnswersTable = @"
                CREATE TABLE IF NOT EXISTS Answers (
                    AnswerId INTEGER PRIMARY KEY AUTOINCREMENT,
                    QuestionId INTEGER NOT NULL,
                    AnswerText TEXT NOT NULL,
                    FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId) ON DELETE CASCADE
                )";

        string createResultsTable = @"
                CREATE TABLE IF NOT EXISTS Results (
                    ResultId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    QuestionId INTEGER NOT NULL,
                    AnswerId INTEGER NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,                    
                    FOREIGN KEY (AnswerId) REFERENCES Answers(AnswerId) ON DELETE CASCADE,
                    FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId) ON DELETE CASCADE
                )";

        using var command = new SqliteCommand(createUsersTable, connection);
        command.ExecuteNonQuery();

        command.CommandText = createTestsTable;
        command.ExecuteNonQuery();

        command.CommandText = createResultsTable;
        command.ExecuteNonQuery();

        command.CommandText = createQuestionsTable;
        command.ExecuteNonQuery();

        command.CommandText = createAnswersTable;
        command.ExecuteNonQuery();

    }

    private static void AddUsersToDatabase()
    {
        string insertUserQuery = @"
            INSERT INTO Users (Login, Name, Surname, Email) 
            VALUES (@Login, @Name, @Surname, @Email)";

        using var command = new SqliteCommand(insertUserQuery, connection);

        // Add users
        {
            AddUser(command, "timi", "Ilia", "Timofeev", "timi@gmail.com");
            AddUser(command, "anni", "Anna", "Ivanova", "anna@gmail.com");
            AddUser(command, "johnsmith", "John", "Smith", "john.smith@gmail.com");
            AddUser(command, "kate123", "Kate", "Johnson", "kate.johnson@gmail.com");
            AddUser(command, "mike77", "Michael", "Brown", "mike.brown@gmail.com");
            AddUser(command, "sara01", "Sarah", "Davis", "sara.davis@gmail.com");
            AddUser(command, "alex99", "Alex", "Wilson", "alex.wilson@gmail.com");
            AddUser(command, "linda45", "Linda", "Garcia", "linda.garcia@gmail.com");
            AddUser(command, "robertx", "Robert", "Martinez", "robert.martinez@gmail.com");
            AddUser(command, "emma22", "Emma", "Hernandez", "emma.hernandez@gmail.com");
            AddUser(command, "chris007", "Chris", "Moore", "chris.moore@gmail.com");
            AddUser(command, "olivia56", "Olivia", "Taylor", "olivia.taylor@gmail.com");
            AddUser(command, "daniel89", "Daniel", "Anderson", "daniel.anderson@gmail.com");
            AddUser(command, "amelia77", "Amelia", "Thomas", "amelia.thomas@gmail.com");
        }

        Console.WriteLine("14 Users were added to the database successfully.\n");
    }
    private static void AddUser(SqliteCommand command, string login, string name, string surname, string email)
    {
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Login", login);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Surname", surname);
        command.Parameters.AddWithValue("@Email", email);

        command.ExecuteNonQuery();
    }

    private static void AddTestsToDatabase()
    {
        // Add tests
        {

            AddTest(
                "Mood test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("How are you?", new List<string> { "Good", "Bad" }),
            ("Is it your best day today?", new List<string> { "Yes", "No", "Of course" }),
            ("Are you an optimist or pessimist?", new List<string> { ":D", ":(" })
                }
            );

            AddTest(
                "Happiness test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("Are you happy?", new List<string> { "Yes", "No", "Of course" })
                }
            );

            AddTest(
                "Day test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("What day is it?", new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
                }
            );

            AddTest(
                "Personality test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("Do you enjoy meeting new people?", new List<string> { "Yes", "No", "Sometimes" }),
            ("How do you prefer to spend your free time?", new List<string> { "Alone", "With friends", "With family" }),
            ("What motivates you the most?", new List<string> { "Success", "Recognition", "Personal growth" }),
            ("Are you more logical or emotional?", new List<string> { "Logical", "Emotional", "A mix of both" }),
            ("Do you often plan your day ahead?", new List<string> { "Always", "Rarely", "Never" })
                }
            );

            AddTest(
                "Work-life balance test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("How many hours a day do you work?", new List<string> { "Less than 6", "6-8", "More than 8" }),
            ("Do you often take breaks during work?", new List<string> { "Yes", "No", "Sometimes" }),
            ("Do you feel satisfied with your work-life balance?", new List<string> { "Yes", "No", "Partially" }),
            ("How do you usually spend your weekends?", new List<string> { "Resting", "Working", "Socializing" }),
            ("Do you have time for hobbies?", new List<string> { "Always", "Sometimes", "Never" })
                }
            );

            AddTest(
                "Learning preferences test",
                new List<(string QuestionText, List<string> Answers)>
                {
            ("How do you prefer to learn new skills?", new List<string> { "Reading", "Watching videos", "Hands-on practice" }),
            ("Do you enjoy working in groups?", new List<string> { "Yes", "No", "Depends on the task" }),
            ("What type of content is easier for you to understand?", new List<string> { "Visual", "Text", "Audio" }),
            ("How do you organize your learning materials?", new List<string> { "Neatly categorized", "Randomly saved", "Don't organize" }),
            ("How often do you review what you've learned?", new List<string> { "Regularly", "Rarely", "Never" }),
            ("What motivates you to learn?", new List<string> { "Personal interest", "Career growth", "External pressure" })
                }
            );
        }

        Console.WriteLine("6 Tests were added to the database successfully.\n");
    }
    public static void AddTest(string testName, List<(string QuestionText, List<string> Answers)> questions)
    {
        string insertTestQuery = @"
    INSERT INTO tests (TestName)
    VALUES (@TestName);
    SELECT last_insert_rowid();";

        using var testCommand = new SqliteCommand(insertTestQuery, connection);
        testCommand.Parameters.AddWithValue("@TestName", testName);
        long testId = (testCommand.ExecuteScalar() as long?) ?? throw new InvalidOperationException("Failed to retrieve TestId.");

        Console.WriteLine($"Test \"{testName}\" added.");

        string insertQuestionQuery = @"
        INSERT INTO Questions (TestId, QuestionText)
        VALUES (@TestId, @QuestionText);
        SELECT last_insert_rowid();";

        string insertAnswerQuery = @"
        INSERT INTO Answers (QuestionId, AnswerText)
        VALUES (@QuestionId, @AnswerText);";

        using var questionCommand = new SqliteCommand(insertQuestionQuery, connection);
        using var answerCommand = new SqliteCommand(insertAnswerQuery, connection);

        foreach (var (questionText, answers) in questions)
        {
            questionCommand.Parameters.Clear();
            questionCommand.Parameters.AddWithValue("@TestId", testId);
            questionCommand.Parameters.AddWithValue("@QuestionText", questionText);

            long questionId = (questionCommand.ExecuteScalar() as long?) ?? throw new InvalidOperationException("Failed to retrieve QuestionId.");

            Console.WriteLine($"Question \"{questionText}\" in \"{testName}\" added.");

            foreach (var answerText in answers)
            {
                answerCommand.Parameters.Clear();
                answerCommand.Parameters.AddWithValue("@QuestionId", questionId);
                answerCommand.Parameters.AddWithValue("@AnswerText", answerText);
                answerCommand.ExecuteNonQuery();

                Console.WriteLine($"Answer \"{answerText}\" in \"{questionText}\" added.");
            }
            Console.WriteLine($"All answers in \"{questionText}\" added.");
        }

        Console.WriteLine($"All questions in test \"{testName}\" added.\n");
    }

    public static void AddResultsToDatabase()
    {
        // Get list of all users
        var users = new List<long>();
        using (var userCommand = new SqliteCommand("SELECT UserId FROM Users", connection))
        using (var reader = userCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                users.Add(reader.GetInt64(0));
            }
        }

        // Get list of all tests
        var tests = new Dictionary<long, Dictionary<long, List<long>>>();
        using (var testCommand = new SqliteCommand("SELECT TestId FROM Tests", connection))
        using (var testReader = testCommand.ExecuteReader())
        {
            while (testReader.Read())
            {
                long testId = testReader.GetInt64(0);
                var questionsAndAnswers = new Dictionary<long, List<long>>();

                // Get all questions for the current test
                using (var questionCommand = new SqliteCommand("SELECT QuestionId FROM Questions WHERE TestId = @TestId", connection))
                {
                    questionCommand.Parameters.AddWithValue("@TestId", testId);
                    using (var questionReader = questionCommand.ExecuteReader())
                    {
                        while (questionReader.Read())
                        {
                            long questionId = questionReader.GetInt64(0);
                            var answers = new List<long>();

                            // Get all answers for the current question
                            using (var answerCommand = new SqliteCommand("SELECT AnswerId FROM Answers WHERE QuestionId = @QuestionId", connection))
                            {
                                answerCommand.Parameters.AddWithValue("@QuestionId", questionId);
                                using (var answerReader = answerCommand.ExecuteReader())
                                {
                                    while (answerReader.Read())
                                    {
                                        answers.Add(answerReader.GetInt64(0));
                                    }
                                }
                            }

                            questionsAndAnswers[questionId] = answers;
                        }
                    }
                }

                tests[testId] = questionsAndAnswers;
            }
        }

        var random = new Random();

        // Add results for each user
        foreach (var userId in users)
        {
            foreach (var test in tests)
            {
                long testId = test.Key;
                var questionsAndAnswers = test.Value;

                // Decide if user will complete the test
                if (random.NextDouble() > TestProbability) continue; // Skip this test entirely

                // Add answers for all questions in the test
                foreach (var question in questionsAndAnswers)
                {
                    long questionId = question.Key;
                    var answers = question.Value;

                    if (answers.Count > 0)
                    {
                        long randomAnswerId = answers[random.Next(answers.Count)];

                        // Insert result into database
                        string insertResultQuery = @"
                        INSERT INTO Results (UserId, QuestionId, AnswerId)
                        VALUES (@UserId, @QuestionId, @AnswerId);";

                        using var insertCommand = new SqliteCommand(insertResultQuery, connection);
                        insertCommand.Parameters.AddWithValue("@UserId", userId);
                        insertCommand.Parameters.AddWithValue("@QuestionId", questionId);
                        insertCommand.Parameters.AddWithValue("@AnswerId", randomAnswerId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        Console.WriteLine($"Random answers added to the database with TestProbability {TestProbability}.\n");
    }
}