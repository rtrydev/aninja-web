using aninja_anime_service.Dtos;
using aninja_anime_service.Enums;
using aninja_anime_service.Handlers;
using aninja_anime_service.Models;
using aninja_anime_service.Profiles;
using aninja_anime_service.Queries;
using aninja_anime_service.Repositories;
using aninja_anime_service.SyncDataServices;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace aninja_anime_service_tests
{
    public class GetAllAnimesQueryTest
    {
        private readonly IEnumerable<Anime> _data = new List<Anime>
        {
            new()
            {
                Id = 1,
                OriginalTitle = "あいうえお",
                TranslatedTitle = "Title1",
                Status = Status.CurrentlyAiring,
                Demographic = Demographic.Josei,
                Description = "test desc1",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2020, 12, 1),
                EpisodeCount = 12
            },
            new()
            {
                Id = 2,
                OriginalTitle = "かきくけこ",
                TranslatedTitle = "Title2",
                Status = Status.FinishedAiring,
                Demographic = Demographic.Seinen,
                Description = "test desc2",
                ImgUrl = "https://google.com",
                EndDate = new DateTime(2021, 4, 5),
                StartDate = new DateTime(2020, 12, 1),
                EpisodeCount = 10
            },
            new()
            {
                Id = 3,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = Status.NotYetAired,
                Demographic = Demographic.Shoujo,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            }
        };

        private readonly IEnumerable<int> tagIds = new List<int> { 1, 2 };

        [Fact]
        public async Task GetAll_WithoutQueries_ReturnsBaseSet()
        {
            //Arrange
            var mockRepo = new Mock<IAnimeRepository>();
            mockRepo.Setup(x => x.GetAll()).Returns(Task.FromResult(_data));

            IEnumerable<Anime> animeWithTag = new List<Anime>() { _data.First() };

            var mockTagDataService = new Mock<IAnimeTagDataClient>();
            mockTagDataService.Setup(x => x.ReturnAllAnimeWithTags(It.Is<IEnumerable<int>>(x => x.Equals(tagIds)))).Returns(animeWithTag);

            var animeProfile = new AnimeProfile();
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(animeProfile));
            var mapper = new Mapper(cfg);

            var handler = new GetAllAnimesQueryHandler(mockRepo.Object, mockTagDataService.Object);

            var query = new GetAllAnimesQuery() { };

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().BeSameAs(_data);


        }

        [Fact]
        public async Task GetAll_WithGivenDemographic_ReturnsWithDemographic()
        {
            //Arrange
            var mockRepo = new Mock<IAnimeRepository>();
            mockRepo.Setup(x => x.GetAll()).Returns(Task.FromResult(_data));

            IEnumerable<Anime> animeWithTag = new List<Anime>() { _data.First() };

            var mockTagDataService = new Mock<IAnimeTagDataClient>();
            mockTagDataService.Setup(x => x.ReturnAllAnimeWithTags(It.Is<IEnumerable<int>>(x => x.Equals(tagIds)))).Returns(animeWithTag);

            var animeProfile = new AnimeProfile();
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(animeProfile));
            var mapper = new Mapper(cfg);

            var handler = new GetAllAnimesQueryHandler(mockRepo.Object, mockTagDataService.Object);

            var queryOne = new GetAllAnimesQuery() { Demographics = new[] { "Josei" } };
            var queryOneResult = _data.Where(x => x.Demographic == Demographic.Josei);

            var queryMulti = new GetAllAnimesQuery() { Demographics = new[] { "Josei", "Shoujo" } };
            var queryMultiResult = _data.Where(x => x.Demographic == Demographic.Josei || x.Demographic == Demographic.Shoujo);

            var queryMultiWithExistentAndInexistent = new GetAllAnimesQuery() { Demographics = new[] { "Josei", "Shoujo", "Shounen" } };
            var queryMultiWithExistentAndInexistentResult = _data.Where(x => x.Demographic == Demographic.Josei || x.Demographic == Demographic.Shoujo);

            var queryWithOnlyInexistent = new GetAllAnimesQuery() { Demographics = new[] { "Shounen" } };

            var queryWithManyInexistent = new GetAllAnimesQuery() { Demographics = new[] { "Shounen", "Shounen" } };


            //Act
            var resultOne = await handler.Handle(queryOne, CancellationToken.None);
            var resultMulti = await handler.Handle(queryMulti, CancellationToken.None);
            var resultMultiWithExistentAndInexistent = await handler.Handle(queryMultiWithExistentAndInexistent, CancellationToken.None);
            var resultWithOnlyInexistent = await handler.Handle(queryWithOnlyInexistent, CancellationToken.None);
            var resultWithManyInexistent = await handler.Handle(queryWithManyInexistent, CancellationToken.None);

            //Assert
            resultOne.Should().BeEquivalentTo(queryOneResult);
            resultMulti.Should().BeEquivalentTo(queryMultiResult);
            resultMultiWithExistentAndInexistent.Should().BeEquivalentTo(queryMultiWithExistentAndInexistentResult);
            resultWithOnlyInexistent.Should().BeEmpty();
            resultWithManyInexistent.Should().BeEmpty();

        }
    }
}
