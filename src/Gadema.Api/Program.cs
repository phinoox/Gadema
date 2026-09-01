// =============================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos;
using Gadema.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using Gadema.Data.Database;
using Gadema.Core.Services;

namespace Gadema.Api;

// Gadema.Api - ASP.NET Core Web API Entry Point
// =============================================================================

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Console.WriteLine("Ficker");
        // Add services to the container.
        builder.Services.AddControllers();
        string allargs = "Penis args:";
        foreach(var arg in args)
        {
            allargs += "," + arg;
        }
        //Console.WriteLine(allargs);

       // if(builder.Environment.IsProduction() || builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<GameDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            
        }

        // Register service implementations
        builder.Services.AddScoped<IContentService, ContentItemService>();
        builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IDialogueService, DialogueService>();
        builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IExternalReferenceService, ExternalReferenceService>();
        builder.Services.AddScoped<IStoryOutlineService, StoryOutlineService>();
        builder.Services.AddScoped<IExportService, ExportService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IReviewStatusService, ReviewStatusService>();
        builder.Services.AddScoped<IUserContext, UserContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { Status = "healthy" }));
        app.Run();

    }
}