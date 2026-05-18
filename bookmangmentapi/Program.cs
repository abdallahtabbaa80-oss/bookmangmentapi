using bookmangmentapi.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// register http client for service layer
builder.Services.AddHttpClient<Service>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BooksView}/{action=Index}/{id?}");

app.Run();
