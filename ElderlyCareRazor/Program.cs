using System.Globalization;
using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var connectionString = builder.Configuration.GetConnectionString("MyStockDB");

builder.Services.AddDbContext<ElderCareContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<AccountDAO, AccountDAO>();
builder.Services.AddScoped<ServiceDAO, ServiceDAO>();
builder.Services.AddScoped<RoleDAO>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingTimeSlotRepository, BookingTimeSlotRepository>();
builder.Services.AddScoped<ICaregiverAvailabilityRepository, CaregiverAvailabilityRepository>();
builder.Services.AddScoped<ICaregiverRepository, CaregiverRepository>();
builder.Services.AddScoped<IElderRepository, ElderRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IRecordRepository, RecordRepository>();
builder.Services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
builder.Services.AddScoped<ITrackingRepository, TrackingRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingTimeSlotService, BookingTimeSlotService>();
builder.Services.AddScoped<ICaregiverAvailabilityService, CaregiverAvailabilityService>();
builder.Services.AddScoped<ICaregiverService, CaregiverService>();
builder.Services.AddScoped<IElderService, ElderService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();


builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "855024682610-7me9k0ap16evk6iero8s8293i4sp7o7o.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("https://www.googleapis.com/auth/user.phonenumbers.read");
    options.Events.OnRemoteFailure = context =>
    {
        if (context.Failure is AuthenticationFailureException &&
            context.Failure.Message.Contains("access_denied"))
        {
            context.Response.Redirect("/Auth/Login?errorMessage=Cancel.");
            context.HandleResponse();
        }

        return Task.CompletedTask;
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
