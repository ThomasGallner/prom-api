using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PromContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
var app = builder.Build();

app.MapGet("/ticket-types", async (PromContext context) =>
{
    var ticketTypes = await context.TicketTypes.ToListAsync();

    return Results.Ok(ticketTypes.Select(tt => new TicketTypeResult(tt.ID, tt.Name, tt.Price)));
});

app.MapPost("/students", async (HttpRequest request, PromContext context) =>
{
    if (!request.ContentType!.StartsWith("text/plain"))
    {
        return Results.BadRequest();
    }

    using var stream = new StreamReader(request.Body);

    // ignore first line (header)
    await stream.ReadLineAsync();
    string? line;

    while ((line = await stream.ReadLineAsync()) != null)
    {
        var cols = line.Split(',');
        var invitationCode = cols[5];

        if (!await context.Students.AnyAsync(s => s.InvitationCode == invitationCode))
        {
            var student = new Student
            {
                InvitationCode = invitationCode,
            };

            await context.Students.AddAsync(student);
        }
    }

    await context.SaveChangesAsync();

    return Results.Ok();
});

#region Advanced Solution
// (/teachers missing) (writer)

// app.MapPost("/students", async (HttpRequest request, PromContext context) =>
//     await ImportFromCsv(request, context, async (cols, context) => {
//         var invitationCode = cols[5];

//         if (!await context.Students.AnyAsync(s => s.InvitationCode == invitationCode))
//         {
//             var student = new Student
//             {
//                 InvitationCode = invitationCode,
//             };

//             await context.Students.AddAsync(student);
//         }
//     }));

// async Task<IResult> ImportFromCsv(HttpRequest request, PromContext context, Func<string[], PromContext, Task> writer)
// {
//     if (!request.ContentType!.StartsWith("text/plain"))
//     {
//         return Results.BadRequest();
//     }

//     using var stream = new StreamReader(request.Body);

//     // ignore first line (header)
//     await stream.ReadLineAsync();
//     string? line;

//     while ((line = await stream.ReadLineAsync()) != null)
//     {
//         var cols = line.Split(',');
//         await writer(cols, context);
//     }

//     await context.SaveChangesAsync();

//     return Results.Ok();
// }
#endregion

app.MapPost("/teachers", async (HttpRequest request, PromContext context) =>
{
    if (!request.ContentType!.StartsWith("text/plain"))
    {
        return Results.BadRequest();
    }

    using var stream = new StreamReader(request.Body);

    // ignore first line (header)
    await stream.ReadLineAsync();
    string? line;

    while ((line = await stream.ReadLineAsync()) != null)
    {
        var cols = line.Split(',');
        var invitationCode = cols[5];

        if (!Boolean.TryParse(cols[4], out var teacherOf5thGrade))
        {
            continue;
        }

        if (!await context.Teachers.AnyAsync(t => t.InvitationCode == invitationCode))
        {
            var teacher = new Teacher
            {
                InvitationCode = invitationCode,
                TeacherOf5thGrade = teacherOf5thGrade,
            };

            await context.Teachers.AddAsync(teacher);
        }
    }

    await context.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("/purchases", async (List<AddTicketRequestDto> ticketRequests, PromContext context) =>
{
    var purchase = new Purchase
    {
        TotalPrice = 0,
    };

    foreach (var ticketRequest in ticketRequests)
    {
        var ticket = new Ticket
        {
            FirstName = ticketRequest.FirstName,
            LastName = ticketRequest.LastName,
            TicketTypeID = ticketRequest.TicketTypeID,
        };

        var ticketType = await context.TicketTypes.FirstAsync(tt => tt.ID == ticket.TicketTypeID);
        purchase.TotalPrice += ticketType.Price;

        // invitation code needed
        if (ticketType.ID == (int)TicketTypeIDs.Student)
        {
            var student = await context.Students.FirstAsync(s => s.InvitationCode == ticketRequest.InvitationCode);

            if (student == null)
            {
                return Results.BadRequest();
            }

            ticket.Student = student;

            // TODO check amount of invitation code usage
            // code below won't work
            // student.Tickets.Count
            /*  Latest objective that was worked on
                The API must support buying tickets. It must be possible to buy multiple tickets in a single API request. The following data is required when tickets are bought:
                    First name of guest
                    Last name of guest
                    Ticket type
                    Invitation code (if required for the ticket type)
            */
        }

        purchase.Tickets.Add(ticket);
    }

    await context.Purchases.AddAsync(purchase);
    await context.SaveChangesAsync();

    // temp return -> so that the code is compilable
    return Results.Ok();
});

app.MapGet("/purchases/{id}", (string lastName, int id) => { throw new NotImplementedException(); });

app.MapDelete("/purchases/{id}", (string lastName, int id) => { throw new NotImplementedException(); });

app.MapGet("/purchases/statistics", () => { throw new NotImplementedException(); });

app.Run();


record TicketTypeResult(int ID, string Name, int Price);
record AddTicketRequestDto(string FirstName, string LastName, int TicketTypeID, string? InvitationCode);

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

enum TicketTypeIDs
{
    Student = 3,
    Teacher = 4,
    TeacherOf5thGrade = 5,
}

enum MaxAllowedTicketsPerPerson
{
    Student = 3,
    Teacher = 2,
    TeacherOf5thGrade = 2,
}