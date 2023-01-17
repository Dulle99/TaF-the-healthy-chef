using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Neo4jClient;
using StackExchange.Redis;
using System.Text;
using Quartz;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_API.ScheduledJob;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaF_WebAPI", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

    c.AddSecurityRequirement(securityRequirement);

});


#region ConnectingToDatabase's


var graphClient = new GraphClient(new Uri(
                builder.Configuration.GetSection("Neo4jConnectionSettings:Server").Value),
                builder.Configuration.GetSection("Neo4jConnectionSettings:User").Value,
                builder.Configuration.GetSection("Neo4jConnectionSettings:Password").Value)
{
    DefaultDatabase = "taf"
};

graphClient.ConnectAsync();

var multiplexer = ConnectionMultiplexer.Connect("localhost");

builder.Services.AddSingleton<IGraphClient, GraphClient>(_ => graphClient);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

#endregion ConnectingToDatabase's

#region ScheduledJobs

IDictionary<string, object> databasesClients = new Dictionary<string, object>
{
    { "neo4jClient", graphClient },
    { "redisMultiplexer", multiplexer }
};

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var CacheRecomendedContentJobKey = new JobKey("CacheRecomendedContent");
    var CacheBadWordsJobKey = new JobKey("CacheBadWords");

    q.AddJob<CacheRecomendedContent>(opts => {
        opts.WithIdentity(CacheRecomendedContentJobKey);
        opts.SetJobData(new JobDataMap(databasesClients));
        });

    q.AddTrigger(opts => opts
        .ForJob(CacheRecomendedContentJobKey)
        .WithIdentity("CacheRecomendedContent-Trigger")
        .StartAt(DateTimeOffset.Now));


q.AddTrigger(opts => opts
    .ForJob(CacheRecomendedContentJobKey)
    .WithIdentity("CacheRecomendedContent-ScheduledTrigger")
    //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 20))
    .WithCronSchedule("0 0 */3 ? * *")); //occurs on every 3hr

    q.AddJob<CacheBadWordList>(opts => {
        opts.WithIdentity(CacheBadWordsJobKey);
        opts.SetJobData(new JobDataMap(databasesClients));
    });

    q.AddTrigger(opts => opts
        .ForJob(CacheBadWordsJobKey)
        .WithIdentity("CacheBadWords-Trigger")
        .StartAt(DateTimeOffset.Now));

});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

#endregion ScheduledJobs

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Jwt";
    options.DefaultChallengeScheme = "Jwt";
}).AddJwtBearer("Jwt", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit")),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date
    };

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaF_WebAPI v1"));
}

app.UseCors(configurePolicy =>
                configurePolicy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

