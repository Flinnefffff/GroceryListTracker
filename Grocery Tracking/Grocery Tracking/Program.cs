using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5002"); // specify your URL here
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // replace with your frontend URL
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class Purchase
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? Quantity { get; set; }
    public DateTime? Date { get; set; }
    public string StoreName { get; set; } = string.Empty;
}

public class Summary
{
    public int UserId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal TotalSpending { get; set; }
}

public class DataAccessLayer
{
    private MySqlConnection connection;

    public DataAccessLayer()
    {
        string connectionString = "server=mari.vamk.fi;user=e2101098;password=cqgYeaFEN6A;database=e2101098_Windows;charset=utf8mb4";
        connection = new MySqlConnection(connectionString);
    }

    public void AddGroceryItem(string itemName, decimal price, DateTime date)
    {
        string query = "INSERT INTO Purchase (ItemName, Price, Date) VALUES (@itemName, @price, @date)";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@itemName", itemName);
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@date", date);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public User CreateUser(User user)
    {
        string query = "INSERT INTO User (Name, Email, PasswordHash) VALUES (@Name, @Email, @PasswordHash)";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        // Return the newly created user
        return user;
    }

    public User? GetUserByUsernameAndPassword(string username, string password)
    {
        string query = "SELECT * FROM User WHERE Name = @Name AND PasswordHash = @PasswordHash";

        User? user = null;

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Name", username);
            command.Parameters.AddWithValue("@PasswordHash", password);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Email = reader.GetString("Email"),
                        PasswordHash = reader.GetString("PasswordHash"),
                    };
                }
            }

            connection.Close();
        }

        return user;
    }

    public List<Purchase> GetPriceHistory(string itemName)
    {
        List<Purchase> purchases = new List<Purchase>();

        string query = "SELECT * FROM Purchase WHERE ItemName = @itemName ORDER BY Date";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@itemName", itemName);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Purchase purchase = new Purchase
                    {
                        Id = reader.GetInt32("Id"),
                        ItemName = reader.GetString("ItemName"),
                        Price = reader.GetDecimal("Price"),
                        Date = reader.GetDateTime("Date"),
                        // Set other properties...
                    };
                    purchases.Add(purchase);
                }
            }

            connection.Close();
        }

        return purchases;
    }

    public List<Summary> GetMonthlySpending()
    {
        List<Summary> summaries = new List<Summary>();

        string query = @"
            SELECT DATE_FORMAT(Date, '%Y-%m') as Month, SUM(Price) as TotalSpending
            FROM Purchase
            GROUP BY Month
            ORDER BY Month";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Summary summary = new Summary
                    {
                        Month = reader.GetString("Month"),
                        TotalSpending = reader.GetDecimal("TotalSpending")
                        // Set other properties...
                    };
                    summaries.Add(summary);
                }
            }

            connection.Close();
        }

        return summaries;
    }

    // ... other methods ...
}


[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private DataAccessLayer dataAccessLayer;

    public ItemController()
    {
        dataAccessLayer = new DataAccessLayer();
    }

    [HttpPost]
    public ActionResult AddGroceryItem(string itemName, decimal price, DateTime date)
    {
        dataAccessLayer.AddGroceryItem(itemName, price, date);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult<User> Login(User user)
    {
        User loggedInUser = dataAccessLayer.GetUserByUsernameAndPassword(user.Name, user.PasswordHash);
        if (loggedInUser == null)
        {
            return NotFound();
        }
        return loggedInUser;
    }

    [HttpPost("signup")]
    public ActionResult<User> Signup(User user)
    {
        User createdUser = dataAccessLayer.CreateUser(user);
        if (createdUser == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Login), new { username = createdUser.Name, password = createdUser.PasswordHash }, createdUser);
    }

    [HttpGet("{itemName}")]
    public ActionResult<List<Purchase>> GetPriceHistory(string itemName)
    {
        List<Purchase> purchases = dataAccessLayer.GetPriceHistory(itemName);
        if (purchases == null)
        {
            return NotFound();
        }
        return purchases;
    }

    [HttpGet("summary")]
    public ActionResult<List<Summary>> GetMonthlySpending()
    {
        List<Summary> summaries = dataAccessLayer.GetMonthlySpending();
        if (summaries == null)
        {
            return NotFound();
        }
        return summaries;
    }

    // Implement other API endpoints (e.g., GET (all), POST, PUT, DELETE)
}
