using Microsoft.EntityFrameworkCore;
using OrdersApi.PaymentMethods.Interfaces;
using OrdersApi.PaymentMethods;
using OrdersApi.Repository;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddTransient<IPaymentStrategy, PixPaymentStrategy>();
builder.Services.AddTransient<IPaymentStrategy, CreditCardPaymentStrategy>();


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
