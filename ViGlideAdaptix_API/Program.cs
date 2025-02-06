using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ViGlideAdaptix_BLL.Helper;
using ViGlideAdaptix_BLL.Service.CartService;
using ViGlideAdaptix_BLL.Service.CustomerService;
using ViGlideAdaptix_BLL.Service.OrderService;
using ViGlideAdaptix_BLL.Service.PaymentServices;
using ViGlideAdaptix_BLL.Service.ProductService;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Momo Service
builder.Services.AddScoped<MomoService>();
//Add VnPay Service
builder.Services.AddScoped<VnPayService>();


//Add service for DBContext
builder.Services.AddDbContext<ViGlideAdaptixContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Register for Service
builder.Services.AddScoped<HashSha256>();
builder.Services.AddScoped<JWTToken>();
builder.Services.AddScoped<ImageHelper>();


builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//Register for Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));




// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "")),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ClockSkew = TimeSpan.Zero
                };
            });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

