using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using aninja_anime_service.Data;
using aninja_anime_service.Enums;
using aninja_anime_service.Models;
using aninja_anime_service.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace aninja_anime_service_tests;

public class AnimeRepoTest
{
    //methodName_testedScenario_expectedBehaviour
    
    // arrange -> act -> assert
    
    private readonly List<Anime> _data = new ()
    {
        new ()
        {
            Id = 1, OriginalTitle = "あいうえお", TranslatedTitle = "Title1", Status = Status.CurrentlyAiring,
            Demographic = Demographic.Josei, Description = "test desc1", ImgUrl = null, EndDate = new DateTime(),
            StartDate = new DateTime(2020, 12, 1), EpisodeCount = 12
        },
        new ()
        {
            Id = 2, OriginalTitle = "かきくけこ", TranslatedTitle = "Title2", Status = Status.FinishedAiring,
            Demographic = Demographic.Seinen, Description = "test desc2", ImgUrl = "https://google.com",
            EndDate = new DateTime(2021, 4, 5), StartDate = new DateTime(2020, 12, 1), EpisodeCount = 10
        },
        new ()
        {
            Id = 3, OriginalTitle = "たちつてと", TranslatedTitle = "Title3", Status = Status.NotYetAired,
            Demographic = Demographic.Shoujo, Description = "test desc3", ImgUrl = null, EndDate = new DateTime(),
            StartDate = new DateTime(2023, 12, 1), EpisodeCount = 0
        }
    };
    
    [Fact]
    public async Task GetAll_GetWithoutQuery_ReturnsAll()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AnimeRepoTest")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(_data);
        await context.SaveChangesAsync();
        
        var repo = new AnimeRepository(context);

        //Act
        var anime = await repo.GetAll();
        
        //Assert
        anime.Should().Equal(_data);

    }

    [Fact]
    public async Task GetById_GetById_ReturnsEntityWithGivenIdOrNull()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AnimeRepoTest")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(_data);
        await context.SaveChangesAsync();
        
        var repo = new AnimeRepository(context);

        //Act
        var anime0 = await repo.GetById(0);

        var animeMinus = await repo.GetById(-1);
        
        var anime1 = await repo.GetById(1);
        var anime2 = await repo.GetById(2);
        var anime3 = await repo.GetById(3);
        
        //Assert
        anime0.Should().BeNull();
        animeMinus.Should().BeNull();
        
        anime1.Should().BeSameAs(_data[0]);
        anime2.Should().BeSameAs(_data[1]);
        anime3.Should().BeSameAs(_data[2]);
    }

    [Fact]
    public async Task Create_AddNewEntry_AddsNewEntry()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AnimeRepoTest")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(_data);
        await context.SaveChangesAsync();
        
        var repo = new AnimeRepository(context);

        //Act
        var animeToAdd = new Anime()
        {
            Id = 4, OriginalTitle = "さしすせそ", TranslatedTitle = "Some title", Demographic = Demographic.Josei,
            Description = "Desc", Status = Status.CurrentlyAiring, StartDate = new DateTime(2021, 11, 11),
            EndDate = new DateTime(), EpisodeCount = 12, ImgUrl = null
        };

        var result = await repo.Create(animeToAdd);
        await repo.SaveChangesAsync();

        //Assert
        var dataAfterAdd = await repo.GetAll();
        dataAfterAdd.Should().HaveCount(4);
        dataAfterAdd.Last().Should().BeSameAs(animeToAdd);
        result.Should().BeSameAs(animeToAdd);

    }

    [Fact]
    public async Task Update_ChangeField_AppliesChange()
    {
        //Arrange
        
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AnimeRepoTest")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(_data);
        await context.SaveChangesAsync();
        
        var repo = new AnimeRepository(context);

        var dataBeforeUpdate = await repo.GetById(3);
        var updateData = await repo.GetById(3);
        updateData!.TranslatedTitle = "Some different title";
        
        //Act
        var result = await repo.Update(updateData);
        await repo.SaveChangesAsync();
        var dataAfterUpdate = await repo.GetAll();
        var entryAfterUpdate = await repo.GetById(3);

        //Assert
        dataAfterUpdate.Should().Contain(entryAfterUpdate);

        result.Should().BeSameAs(updateData);
        entryAfterUpdate.Should().BeSameAs(updateData);

    }

    [Fact]
    public async Task Delete_DeleteExisting_RemovesEntry()
    {
        //Arrange
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AnimeRepoTest")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(contextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.AddRange(_data);
        await context.SaveChangesAsync();

        var repo = new AnimeRepository(context);

        var dataToDelete = await repo.GetById(3);

        //Act
        var result = await repo.Delete(dataToDelete);
        await repo.SaveChangesAsync();
        var dataAfterDelete = await repo.GetAll();
        var entryAfterDelete = await repo.GetById(3);


        //Assert
        result.Should().BeSameAs(dataToDelete);
        dataAfterDelete.Should().HaveCount(2);
        dataAfterDelete.Should().NotContain(dataToDelete);
        entryAfterDelete.Should().BeNull();

    }
}