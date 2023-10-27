using Npgsql;
using System.Collections.Generic; 

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Kangan_Quiz_Project");
app.MapGet("/questions/{id}", (int id) => getQuestion(id)); //get question by id.
app.MapGet("/questions", () => getQuestions()); //get all questions.
app.MapGet("/questionsCorrectOption", () => getQuestionsWithCorrectOption()); //get all questions with correct option.


app.Run();

  
Questions getQuestion(int id) {
    using var connection = getDbConnection();
    connection.Open();
    using var command = new NpgsqlCommand($"SELECT * FROM \"Quizzes\" WHERE \"id\" = {id}", connection);
    using NpgsqlDataReader reader = command.ExecuteReader();
    reader.Read();
    var question = new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)});
    return question;
}

List<Questions> getQuestions() {
    using var connection = getDbConnection();
    connection.Open();
    using var command = new NpgsqlCommand("SELECT * FROM \"Quizzes\"", connection);
    using NpgsqlDataReader reader = command.ExecuteReader();
    var questions = new List<Questions>();
    while (reader.Read()) {
        questions.Add(new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5)}));
    }
    return questions;
}

List<Questions> getQuestionsWithCorrectOption() {
    using var connection = getDbConnection();
    connection.Open();
    using var command = new NpgsqlCommand("SELECT * FROM \"Quizzes\"", connection);
    using NpgsqlDataReader reader = command.ExecuteReader();
    var questions = new List<Questions>();
    while (reader.Read()) {
        questions.Add(new Questions(reader.GetInt32(0), reader.GetString(1), new List<string> {reader.GetString(5)}));
    }
    return questions;
}

NpgsqlConnection getDbConnection() {
    return new NpgsqlConnection("User Id=postgres;Password=Th8f9CuFtj_GgE6;Server=db.evcaibnrztyuudacvojx.supabase.co;Port=5432;Database=postgres");
}
