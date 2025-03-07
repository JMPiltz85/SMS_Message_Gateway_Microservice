using SMS_Message_Gateway_Microservice.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// registers the "MessageTrackingService" service with the container
builder.Services.Configure<MaximumSettings>(builder.Configuration.GetSection("MaximumValues")); // gets Maximum values from appsettings file
builder.Services.AddScoped<SMS_Service.Services.IMessageTrackingService, SMS_Service.Services.MessageTrackingService>();
builder.Services.AddControllers(); // addes controller services to the container

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting(); // configures HTTP request pipeline
app.MapControllers();

app.Run();
