using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PromContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
var app = builder.Build();

app.MapGet("/ticket-types", async (PromContext context) =>
{
    var ticketTypes = await context.TicketTypes.ToListAsync();

    return Results.Ok(ticketTypes.Select(tt => new TicketTypeResult(tt.ID, tt.Name, tt.Price)));
});

app.MapPost("/students", ([FromBody] string csv) =>
{
    return Results.Ok(csv);
});

app.MapPost("/teachers", () => { throw new NotImplementedException(); });

app.MapPost("/purchases", () => { throw new NotImplementedException(); });

app.MapGet("/purchases/{id}", (string lastName, int id) => { throw new NotImplementedException(); });

app.MapDelete("/purchases/{id}", (string lastName, int id) => { throw new NotImplementedException(); });

app.MapGet("/purchases/statistics", () => { throw new NotImplementedException(); });

app.Run();


record AddStudentDto(int StudentID, string InvitationCode);
record AddTeacherDto(int TeacherID, string InvitationCode, bool Teaches5thGrade);

record TicketTypeResult(int ID, string Name, int Price);

class Student
{
    public int ID { get; set; }
    [MaxLength(15)]
    public string InvitationCode { get; set; } = "";
    public List<Ticket> Tickets { get; set; } = new();
}

class Teacher
{
    public int ID { get; set; }
    [MaxLength(15)]
    public string InvitationCode { get; set; } = "";
    public bool TeacherOf5thGrade { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}

class Purchase
{
    public int ID { get; set; }
    public int TotalPrice { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}

class TicketType
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public int Price { get; set; }
    public List<Ticket> Tickets { get; set; } = new();
}

class Ticket
{
    public int ID { get; set; }
    public TicketType? TicketType { get; set; }
    public int TicketTypeID { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public Student? Student { get; set; }
    public int? StudentID { get; set; }
    public Teacher? Teacher { get; set; }
    public int? TeacherID { get; set; }
    public Purchase? Purchase { get; set; }
    public int PurchaseID { get; set; }
}

class PromContext : DbContext
{
    public PromContext(DbContextOptions<PromContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
}
