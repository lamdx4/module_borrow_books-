using LibraryManagement.Presentation.Service;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Repositories (Singleton for In-Memory Concurrent Collections to maintain state)
builder.Services.AddSingleton<IDocGiaRepository, DocGiaRepository>();
builder.Services.AddSingleton<ICuonSachRepository, CuonSachRepository>();
builder.Services.AddSingleton<IGiaoDichMuonTraRepository, GiaoDichMuonTraRepository>();
builder.Services.AddSingleton<IPhieuDatTruocRepository, PhieuDatTruocRepository>();
builder.Services.AddSingleton<IPhieuPhatRepository, PhieuPhatRepository>();
builder.Services.AddSingleton<IUnitOfWork, InMemUnitOfWork>();

// Register MediatR using the correct extension if MediatR is available, assuming typical CQRS pattern
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LibraryManagement.Application.Features.Borrowing.Commands.BorrowBookCommand).Assembly));

builder.Services.AddGrpc();
builder.Services.AddHostedService<ReminderWorker>(); // Register UC-06 Background Task

var app = builder.Build();

app.MapGrpcService<LibraryGrpcService>();

app.Run();