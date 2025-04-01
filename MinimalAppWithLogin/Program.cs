using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_JWT.Models;
using Minimal_JWT.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
// JWT added
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();
//Injections added: WHY?????????
builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddSingleton<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//added::: EXPLAIN!!!!!!!!
app.UseAuthorization();
app.UseAuthentication();


app.MapPost("/login",
    (UserLogin user, IUserService service) => Login(user, service))
    .Accepts<UserLogin>("application/json")
    .Produces<string>();

app.MapPost("/create",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Administrator")]
    (Movie movie, IMovieService service) => Create(movie, service))
    .Accepts<Movie>("application/json")
    .Produces<Movie>(statusCode: 200, contentType: "application/json");

app.MapGet("/get",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Administrator")]
    (int id, IMovieService service) => Get(id, service))
    .Produces<Movie>();

app.MapGet("/list",
    (IMovieService service) => List(service))
    .Accepts<Movie>("application/json");

app.MapPut("/update",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    (Movie newMovie, IMovieService service) => Update(newMovie, service))
    .Accepts<Movie>("application/json")
    .Produces<Movie>(statusCode: 200, contentType: "application/json");

app.MapDelete("/delete",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    (int id, IMovieService service) => Delete(id, service));



IResult Login(UserLogin user, IUserService service)
{
    if (!string.IsNullOrEmpty(user.Username) &&
        !string.IsNullOrEmpty(user.Password))
    {
        var loggedInUser = service.Get(user);
        if (loggedInUser == null) return Results.NotFound("User not found");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, loggedInUser.Username),
            new Claim(ClaimTypes.Email, loggedInUser.EmailAddress),
            new Claim(ClaimTypes.GivenName, loggedInUser.GivenName),
            new Claim(ClaimTypes.Surname, loggedInUser.Surname),
            new Claim(ClaimTypes.Role, loggedInUser.Role)
        };

        var token = new JwtSecurityToken
        (
            //yesss
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
               new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
               SecurityAlgorithms.HmacSha256)
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(tokenString);
    }
    return Results.Ok(user);
}
IResult Create (Movie movie, IMovieService service)
{
    var result = service.Create(movie);
    return Results.Ok(result);
}

IResult Get (int id, IMovieService service)
{
    var movie = service.Get(id);

    if (movie == null) return Results.NotFound("Movie not found");

    return Results.Ok(movie);
}

IResult List (IMovieService service)
{
    var movies = service.List();

    return Results.Ok(movies);
}

IResult Update(Movie newMovie , IMovieService service)
{
    var updatedMovie = service.Update(newMovie);

    if (updatedMovie is null) Results.NotFound("Movie not found");

    return Results.Ok(updatedMovie);
}

IResult Delete(int id, IMovieService service)
{
    var result = service.Delete(id);

    if (!result) Results.BadRequest("Something went wrong");

    return Results.Ok(result);
}




app.UseHttpsRedirection();

app.Run();
