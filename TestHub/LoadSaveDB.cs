using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace TestHub
{
    public static class LoadSaveDB
    {
        // User
        public static void SaveUserToDatabase(User user)
        {
            if (user.UserId == 0)
            {
                // New user
                AddUserToDatabase(user);
            }
            else
            {
                // Existing user
                UpdateUserInDatabase(user);
            }
        }
        public static void AddUserToDatabase(User user)
        {
            string insertQuery = @"
            INSERT INTO Users (Login, Name, Surname, Email) 
            VALUES (@Login, @Name, @Surname, @Email);
            SELECT last_insert_rowid();";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Login", user.Login);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Surname", user.Surname);
                    command.Parameters.AddWithValue("@Email", user.Email);

                    // Get new id
                    user.UserId = (long)(command.ExecuteScalar() ?? 0);

                }
            }
        }
        public static void UpdateUserInDatabase(User user)
        {
            string query = @"
            UPDATE Users
            SET Login = @Login,
                Name = @Name,
                Surname = @Surname,
                Email = @Email
            WHERE UserId = @UserId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@Login", user.Login);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Surname", user.Surname);
                    command.Parameters.AddWithValue("@Email", user.Email);

                    command.ExecuteNonQuery();
                }
            }
        }
        public static ObservableCollection<User> LoadUsersFromDatabase()
        {
            var users = new ObservableCollection<User>();

            string query = "SELECT UserId, Login, Name, Surname, Email FROM Users";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserId = reader.GetInt64(0),
                                Login = reader.GetString(1),
                                Name = reader.GetString(2),
                                Surname = reader.GetString(3),
                                Email = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return users;
        }
        public static void DeleteUserFromDatabase(User user)
        {
            string deleteQuery = @"
            DELETE FROM Users
            WHERE UserId = @UserId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Project
        public static void SaveProjToDatabase(Project project)
        {
            if (project.TestId == 0)
            {
                // New test
                AddProjToDatabase(project);
            }
            else
            {
                // Existing test
                UpdateProjInDatabase(project);
            }
        }
        public static void AddProjToDatabase(Project project)
        {
            string insertQuery = @"
            INSERT INTO Tests (TestName) 
            VALUES (@TestName);
            SELECT last_insert_rowid();";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@TestName", project.ProjTitle);

                    // Get new test ID
                    project.TestId = (long)(command.ExecuteScalar() ?? 0);
                }
            }

            // Save questions and answers for the test
            SaveQuestionsAndAnswersToDatabase(project);
        }
        public static void UpdateProjInDatabase(Project project)
        {
            string updateQuery = @"
            UPDATE Tests
            SET TestName = @TestName
            WHERE TestId = @TestId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@TestId", project.TestId);
                    command.Parameters.AddWithValue("@TestName", project.ProjTitle);

                    command.ExecuteNonQuery();
                }
            }

            // Update questions and answers for the test
            SaveQuestionsAndAnswersToDatabase(project);
        }
        public static ObservableCollection<Project> LoadProjectsFromDatabase()
        {
            var projects = new ObservableCollection<Project>();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            // Query to fetch all tests
            string fetchTestsQuery = @"
            SELECT TestId, TestName
            FROM Tests";

            using var testCommand = new SqliteCommand(fetchTestsQuery, connection);
            using var testReader = testCommand.ExecuteReader();

            while (testReader.Read())
            {
                var testId = testReader.GetInt64(0);
                var testName = testReader.GetString(1);

                var project = new Project
                {
                    TestId = testId,
                    ProjTitle = testName,
                    ProjQuestions = LoadQuestionsForTest(testId, connection)
                };

                projects.Add(project);
            }

            return projects;
        }
        public static void DeleteProjFromDatabase(Project project)
        {
            string deleteQuery = @"
            DELETE FROM Tests
            WHERE TestId = @TestId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@TestId", project.TestId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Question
        public static void SaveQuestionsAndAnswersToDatabase(Project project)
        {
            foreach (var question in project.ProjQuestions)
            {
                SaveQuestionToDatabase(question, project.TestId);
            }
        }

        private static void SaveQuestionToDatabase(Question question, long testId)
        {
            if (question.QuestionId == 0)
            {
                // New question
                AddQuestionToDatabase(question, testId);
            }
            else
            {
                // Update existing question
                UpdateQuestionInDatabase(question);
            }

            foreach (var answer in question.Answers)
            {
                SaveAnswerToDatabase(answer, question.QuestionId);
            }
        }
        private static void AddQuestionToDatabase(Question question, long testId)
        {
            string insertQuestionQuery = @"
            INSERT INTO Questions (TestId, QuestionText) 
            VALUES (@TestId, @QuestionText);
            SELECT last_insert_rowid();";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(insertQuestionQuery, connection))
                {
                    command.Parameters.AddWithValue("@TestId", testId);
                    command.Parameters.AddWithValue("@QuestionText", question.QuestionText);

                    question.QuestionId = (long)(command.ExecuteScalar() ?? 0);
                }
            }
        }
        private static void UpdateQuestionInDatabase(Question question)
        {
            string updateQuestionQuery = @"
            UPDATE Questions
            SET QuestionText = @QuestionText
            WHERE QuestionId = @QuestionId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(updateQuestionQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                    command.Parameters.AddWithValue("@QuestionText", question.QuestionText);

                    command.ExecuteNonQuery();
                }
            }
        }
        public static ObservableCollection<Question> LoadQuestionsForTest(long testId, SqliteConnection connection)
        {
            var questions = new ObservableCollection<Question>();

            // Query to fetch all questions for a specific test
            string fetchQuestionsQuery = @"
            SELECT QuestionId, QuestionText
            FROM Questions
            WHERE TestId = @TestId";

            using var questionCommand = new SqliteCommand(fetchQuestionsQuery, connection);
            questionCommand.Parameters.AddWithValue("@TestId", testId);

            using var questionReader = questionCommand.ExecuteReader();

            while (questionReader.Read())
            {
                var questionId = questionReader.GetInt64(0);
                var questionText = questionReader.GetString(1);

                var question = new Question
                {
                    QuestionId = questionId,
                    QuestionText = questionText,
                    Answers = LoadAnswersForQuestion(questionId, connection)
                };

                questions.Add(question);
            }

            return questions;
        }
        public static void DeleteQuestionFromDatabase(Question question)
        {
            string deleteQuery = @"
            DELETE FROM Questions
            WHERE QuestionId = @QuestionId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        // Answer
        private static void SaveAnswerToDatabase(Answer answer, long questionId)
        {
            ArgumentNullException.ThrowIfNull(answer);
            ArgumentNullException.ThrowIfNull(questionId);

            if (answer.AnswerId == 0)
            {
                // New answer
                AddAnswerToDatabase(answer, questionId);
            }
            else
            {
                // Update existing answer
                UpdateAnswerInDatabase(answer);
            }
        }
        private static void AddAnswerToDatabase(Answer answer, long questionId)
        {
            string insertAnswerQuery = @"
            INSERT INTO Answers (QuestionId, AnswerText) 
            VALUES (@QuestionId, @AnswerText);
            SELECT last_insert_rowid();";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(insertAnswerQuery, connection))
                {
                    command.Parameters.AddWithValue("@QuestionId", questionId);
                    command.Parameters.AddWithValue("@AnswerText", answer.AnswerText);

                    answer.AnswerId = (long)(command.ExecuteScalar() ?? 0);
                }
            }
        }
        private static void UpdateAnswerInDatabase(Answer answer)
        {
            string updateAnswerQuery = @"
            UPDATE Answers
            SET AnswerText = @AnswerText
            WHERE AnswerId = @AnswerId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(updateAnswerQuery, connection))
                {
                    command.Parameters.AddWithValue("@AnswerId", answer.AnswerId);
                    command.Parameters.AddWithValue("@AnswerText", answer.AnswerText);

                    command.ExecuteNonQuery();
                }
            }
        }
        public static ObservableCollection<Answer> LoadAnswersForQuestion(long questionId, SqliteConnection connection)
        {
            var answers = new ObservableCollection<Answer>();

            // Query to fetch all answers for a specific question
            string fetchAnswersQuery = @"
            SELECT AnswerId, AnswerText
            FROM Answers
            WHERE QuestionId = @QuestionId";

            using var answerCommand = new SqliteCommand(fetchAnswersQuery, connection);
            answerCommand.Parameters.AddWithValue("@QuestionId", questionId);

            using var answerReader = answerCommand.ExecuteReader();

            while (answerReader.Read())
            {
                var answerId = answerReader.GetInt64(0);
                var answerText = answerReader.GetString(1);
                answers.Add(new Answer() { AnswerId = answerId, AnswerText = answerText });
            }

            return answers;
        }
        public static void DeleteAnswerFromDatabase(Answer answer)
        {
            string deleteQuery = @"
            DELETE FROM Answers
            WHERE AnswerId = @AnswerId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@AnswerId", answer.AnswerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Result
        public static void SaveResultToDatabase(User user, Project project)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(project);

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();

                if (user.HasCompletedTest)
                {
                    UpdateResultToDatabase(connection, user, project);
                }
                else
                {
                    AddResultToDatabase(connection, user, project);
                }
            }

            Console.WriteLine($"Results for user \"{user.Login}\" and project \"{project.ProjTitle}\" have been saved.\n");
        }
        private static void AddResultToDatabase(SqliteConnection connection, User user, Project project)
        {
            string insertResultQuery = @"
            INSERT INTO Results (UserId, QuestionId, AnswerId)
            VALUES (@UserId, @QuestionId, @AnswerId);";

            foreach (var question in project.ProjQuestions)
            {
                if (question.SelectedAnswer == null)
                {
                    throw new InvalidOperationException("All questions must have a selected answer before saving results.");
                }

                using (var command = new SqliteCommand(insertResultQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                    command.Parameters.AddWithValue("@AnswerId", question.SelectedAnswer.AnswerId);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"New results added for user \"{user.Login}\".");
        }
        private static void UpdateResultToDatabase(SqliteConnection connection, User user, Project project)
        {
            string updateResultQuery = @"
            UPDATE Results
            SET AnswerId = @AnswerId
            WHERE UserId = @UserId AND QuestionId = @QuestionId;";

            foreach (var question in project.ProjQuestions)
            {
                if (question.SelectedAnswer == null)
                {
                    throw new InvalidOperationException("All questions must have a selected answer before updating results.");
                }

                using (var command = new SqliteCommand(updateResultQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
                    command.Parameters.AddWithValue("@AnswerId", question.SelectedAnswer.AnswerId);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Existing results updated for user \"{user.Login}\".");
        }
    }
}
