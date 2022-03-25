using System;
using System.Linq;
using System.Threading.Tasks;
using aninja_rating_service.Models;
using aninja_rating_service.Repositories;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace aninja_rating_service_tests;

public class RatingRepositoryTest
{
    [Fact]
    public async Task GetRatingsForAnime_RatingsExist_ReturnsRatings()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);

        var ratings = new Rating[]
        {
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 2.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 3.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)}
        };
        var ids = ratings.Select(x => x.Id);
        var collection = mongoClient.GetDatabase("ratingDB").GetCollection<Rating>("ratings");
        await collection.InsertManyAsync(ratings);
        
        //Act
        var ratingsReceived = await repository.GetRatingsForAnime(1);

        //Assert
        ratingsReceived.Should().NotBeNull();
        ratingsReceived.Should().NotBeEmpty();
        ratingsReceived.Should().NotContain(ratings[3]);
        ratingsReceived.Should().BeEquivalentTo(ratings.Where(x => x.AnimeId == 1));

        //Teardown
        await collection.DeleteManyAsync(x => ids.Contains(x.Id));
    }
    [Fact]
    public async Task GetRatingsForAnime_RatingsDoNotExist_ReturnsEmptyArray()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);
        
        //Act
        var ratingsReceived = await repository.GetRatingsForAnime(1);
        
        //Assert
        ratingsReceived.Should().NotBeNull();
        ratingsReceived.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetRatingsByUser_RatingsExist_ReturnsRatings()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);

        var someSubmitter = Guid.NewGuid();

        var ratings = new Rating[]
        {
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 2.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = someSubmitter, Comment = "test", Mark = 3.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = someSubmitter, Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)}
        };
        var ids = ratings.Select(x => x.Id);
        var collection = mongoClient.GetDatabase("ratingDB").GetCollection<Rating>("ratings");
        await collection.InsertManyAsync(ratings);
        
        //Act
        var ratingsReceived = await repository.GetRatingsByUser(someSubmitter);

        //Assert
        ratingsReceived.Should().NotBeNull();
        ratingsReceived.Should().NotBeEmpty();
        ratingsReceived.Should().BeEquivalentTo(ratings.Where(x => x.SubmitterId == someSubmitter));

        //Teardown
        await collection.DeleteManyAsync(x => ids.Contains(x.Id));
    }
    
    [Fact]
    public async Task GetRatingsByUser_RatingsDoNotExist_ReturnsEmptyArray()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);

        var someSubmitter = Guid.NewGuid();

        var ratings = new Rating[]
        {
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 2.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 3.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)}
        };
        var ids = ratings.Select(x => x.Id);
        var collection = mongoClient.GetDatabase("ratingDB").GetCollection<Rating>("ratings");
        await collection.InsertManyAsync(ratings);
        
        //Act
        var ratingsReceived = await repository.GetRatingsByUser(someSubmitter);

        //Assert
        ratingsReceived.Should().NotBeNull();
        ratingsReceived.Should().BeEmpty();

        //Teardown
        await collection.DeleteManyAsync(x => ids.Contains(x.Id));
    }

    [Fact]
    public async Task GetAverageRatingForAnime_RatingsExist_ReturnsAvg()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);

        var ratings = new Rating[]
        {
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 2.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 3.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)}
        };
        var ids = ratings.Select(x => x.Id);
        var collection = mongoClient.GetDatabase("ratingDB").GetCollection<Rating>("ratings");
        await collection.InsertManyAsync(ratings);
        
        //Act
        var avg = await repository.GetAverageRatingForAnime(1);
        
        //Assert
        avg.Should().BeApproximately(ratings.Where(x => x.AnimeId == 1).Average(x => x.Mark), 0.01m);
        
        //Teardown
        await collection.DeleteManyAsync(x => ids.Contains(x.Id));

    }
    
    [Fact]
    public async Task GetAverageRatingForAnime_RatingsDoNotExist_ReturnsZero()
    {
        //Arrange
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var repository = new RatingRepository(mongoClient);

        var ratings = new Rating[]
        {
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 2.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 3.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 1, SubmissionDate = new DateTime(2015, 12, 12)},
            new Rating() {Id = Guid.NewGuid(), SubmitterId = Guid.NewGuid(), Comment = "test", Mark = 4.5m, AnimeId = 2, SubmissionDate = new DateTime(2015, 12, 12)}
        };
        var ids = ratings.Select(x => x.Id);
        var collection = mongoClient.GetDatabase("ratingDB").GetCollection<Rating>("ratings");
        await collection.InsertManyAsync(ratings);
        
        //Act
        var avg = await repository.GetAverageRatingForAnime(3);
        
        //Assert
        avg.Should().BeApproximately(0m, 0.01m);
        
        //Teardown
        await collection.DeleteManyAsync(x => ids.Contains(x.Id));
    }
}