using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);

    }
    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    internal Todo.Models.Todo GetById(int id)
    {
        Todo.Models.Todo todo = new();
        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"SELECT * FROM todo Where Id = '{id}'";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                    }
                    else
                    {
                        return todo;
                    }
                };
            }
        }
        return todo;
    }

    public RedirectResult Update(Todo.Models.Todo todo)
    {
        using (var connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return Redirect("https://localhost:7287/");
        }
    }
    internal TodoViewModel GetAllTodos()
    {
        List<Todo.Models.Todo> todoList = new();

        using (SqliteConnection connection =
        new SqliteConnection("Data Source = db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                new Todo.Models.Todo
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    }
                    else
                    {
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                };
            }
        }
        return new TodoViewModel
        {
            TodoList = todoList
        };
    }

    public RedirectResult Insert(Todo.Models.Todo todo)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return Redirect("https://localhost:7287/");
        }
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = connection.CreateCommand())
            {
                connection.Open();
                tableCmd.CommandText = $"DELETE FROM todo WHERE id={id}";
                tableCmd.ExecuteNonQuery();

            }
        }
        return Json(new { });
    }
}
