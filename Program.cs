using Npgsql;
using System.Collections.Generic; 

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

 // Enable CORS
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });

app.MapGet("/", () => "Kangan_Quiz_Project");
app.MapGet("/questions/{id}", (int id) => getQuestion(id)); //get question by id.
app.MapGet("/questions", () => getQuestions()); //get all questions.
app.MapGet("/questionsCorrectOption", () => getQuestionsWithCorrectOption()); //get all questions with correct option.

app.MapGet("/users/{username}", (string username) => checkUser(username)); //check if user exists.

app.MapPost("/questions", (Questions question) => addQuestion(question)); //add question.

app.Run();

Questions getQuestion(int id) {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand($"SELECT * FROM \"Quizzes\" WHERE \"id\" = {id}", connection);
        using NpgsqlDataReader reader = command.ExecuteReader();
        reader.Read();
        var question = new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)});
        return question;
    } catch (Exception ex) {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return null;
    }
}

List<Questions> getQuestions() {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT * FROM \"Quizzes\"", connection);
        using NpgsqlDataReader reader = command.ExecuteReader();
        var questions = new List<Questions>();
        while (reader.Read()) {
            questions.Add(new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)}));
        }
        return questions;
    } catch (Exception ex) {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return null;
    }
}

List<Questions> getQuestionsWithCorrectOption() {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT * FROM \"Quizzes\"", connection);
        using NpgsqlDataReader reader = command.ExecuteReader();
        var questions = new List<Questions>();
        while (reader.Read()) {
            questions.Add(new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(5)}));
        }
        return questions;
    } catch (Exception ex) {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return null;
    }
}

bool checkUser(string username) {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand($"SELECT * FROM \"Users\" WHERE \"userName\" = '{username}'", connection);
        using NpgsqlDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.HasRows) {
            return true;
        } else {
            return false;
        }
    } catch (Exception ex) {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return false;
    }
}

bool addQuestion(Questions question) {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand($"INSERT INTO \"Quizzes\" (\"id\", \"question\", \"correctOption1\", \"option2\", \"option3\", \"option4\") VALUES ({question.Id}, '{question.Question}', '{question.Options[0]}', '{question.Options[1]}', '{question.Options[2]}', '{question.Options[3]}')", connection);
        command.ExecuteNonQuery();
        return true;
    } catch (Exception ex) {
        Console.WriteLine($"An error occurred: {ex.Message}");
        return false;
    }
}

NpgsqlConnection getDbConnection() {
    return new NpgsqlConnection("User Id=postgres;Password=Th8f9CuFtj_GgE6;Server=db.evcaibnrztyuudacvojx.supabase.co;Port=5432;Database=postgres");
}
