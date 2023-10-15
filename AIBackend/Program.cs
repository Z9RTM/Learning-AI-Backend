using AIBackend.AIPlugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.TemplateEngine.Basic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IPromptTemplateEngine, BasicPromptTemplateEngine>();

// IKernel は毎回インスタンス化したいので Transient
builder.Services.AddTransient(sp =>
{
    var section = builder.Configuration.GetSection("OpenAIChatCompletionService");
    return Kernel.Builder
        .WithOpenAIChatCompletionService("gpt-4", "Your API KEY", "Your Org ID")
        .WithPromptTemplateEngine(sp.GetRequiredService<IPromptTemplateEngine>())
        .Build();
});

// ページの情報を管理するクラスを登録
builder.Services.AddTransient<IPageInfoProvider, PageInfoProvider>(sp =>
{
    var pageNavigationPlugins = new PageInfoProvider();
    pageNavigationPlugins.AddPage("WeatherForecast", "showdata", "気温の予報の一覧を表示するページです。");
    pageNavigationPlugins.AddPage("StockForecast", "showdata2", "株価の予測の一覧を表示するページです。");
    pageNavigationPlugins.AddPage("Counter", "counter", "ボタンを押した回数をカウントするページ");
    return pageNavigationPlugins;
});

// ページのお勧めをしてくれるクラスを登録
builder.Services.AddScoped<PageSuggestionService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
