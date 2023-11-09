using Npgsql;
using System.Collections.Generic; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

var app = builder.Build();

// GET endpoints
app.MapGet("/", () => "Kangan_Quiz_Project");
app.MapGet("/questions/{id}", (int id) => getQuestion(id)); //get question by id.
app.MapGet("/questions", () => getQuestions()); //get all questions.
app.MapGet("/questionsCorrectOption", () => getQuestionsWithCorrectOption()); //get all questions with correct option.
app.MapGet("/users/{username}", (string username) => checkUser(username)); //check if user exists.
app.MapGet("get5RandomQuestions", () => get5RandomQuestions()); //get 5 random questions.

    
List<Questions> get5RandomQuestions() {
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand("SELECT * FROM \"Quizzes\" ORDER BY random() LIMIT 5", connection);
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


//POST endpoints
app.MapPost("/questions", (Questions question) => addQuestion(question)); //add question.
app.MapPost("/updateQuestionOptions", (Questions question) => UpdateQuestionOptions(question)); // Update question options

//DELETE endpoints
app.MapDelete("/questions/{id}", (int id) => removeQuestionById(id)); // Delete a question.

app.UseCors();

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

// remove question by id
string removeQuestionById(int id) {
    // Check if the question exists
    Questions question = getQuestion(id);
    if (question == null){
        return $"No question found with ID {id}.";
    }

    // If the question exists then delete it
    try {
        using var connection = getDbConnection();
        connection.Open();
        using var command = new NpgsqlCommand($"DELETE FROM \"Quizzes\" WHERE \"id\" = {id}", connection);
        command.ExecuteNonQuery();
        return $"Question {id} deleted successfully.";
    } catch (Exception ex) { 
        Console.WriteLine($"An error occurred: {ex.Message}");
        return "An error occurred.";
    }
}

// Update question options
app.MapPost("/updateQuestionOptions", (Questions question) => UpdateQuestionOptions(question)); // Update question options

Questions UpdateQuestionOptions(Questions question){
    using var connection = getDbConnection();
    try{
        connection.Open();
        string query = $"UPDATE \"Quizzes\" SET \"correctOption1\" = '{question.Options[0]}', \"option2\" = '{question.Options[1]}', \"option3\" = '{question.Options[2]}', \"option4\" = '{question.Options[3]}' WHERE \"id\" = {question.Id}";

        using (var cmd = new NpgsqlCommand(query, connection)){
            cmd.Parameters.AddWithValue("correctOption1", question.Options[0]);
            cmd.Parameters.AddWithValue("option2", question.Options[1]);
            cmd.Parameters.AddWithValue("option3", question.Options[2]);
            cmd.Parameters.AddWithValue("option4", question.Options[3]);
            cmd.ExecuteNonQuery();
        }
    }
    catch (Exception ex){
        Console.WriteLine($"An error occurred: {ex.Message}");
        return null;
    }
    return question;
}


NpgsqlConnection getDbConnection() {
    return new NpgsqlConnection("User Id=postgres;Password=Th8f9CuFtj_GgE6;Server=db.evcaibnrztyuudacvojx.supabase.co;Port=5432;Database=postgres");
}

