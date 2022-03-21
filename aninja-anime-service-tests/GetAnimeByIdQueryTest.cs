using aninja_anime_service.Enums;
using aninja_anime_service.Handlers;
using aninja_anime_service.Models;
using aninja_anime_service.Profiles;
using aninja_anime_service.Queries;
using aninja_anime_service.Repositories;
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
    public class GetAnimeByIdQueryTest
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
                Status = Status.CurrentlyAiring,
                Demographic = Demographic.Shoujo,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            }
        };

        [Fact]
        public async Task GetById_GetWithExistingId_ReturnsElementWithId()
        {
            //Arrange
            var mockRepo = new Mock<IAnimeRepository>();

            var anime1 = _data.FirstOrDefault(x => x.Id == 1);
            var anime2 = _data.FirstOrDefault(x => x.Id == 2);
            var anime3 = _data.FirstOrDefault(x => x.Id == 3);

            mockRepo.Setup(x => x.GetById(1)).Returns(Task.FromResult(anime1));
            mockRepo.Setup(x => x.GetById(2)).Returns(Task.FromResult(anime2));
            mockRepo.Setup(x => x.GetById(3)).Returns(Task.FromResult(anime3));
            mockRepo.Setup(x => x.GetById(-1)).Returns(Task.FromResult<Anime?>(null));
            mockRepo.Setup(x => x.GetById(15)).Returns(Task.FromResult<Anime?>(null));

            var handler = new GetAnimeByIdQueryHandler(mockRepo.Object);

            var query1 = new GetAnimeByIdQuery() { Id = 1 };
            var query2 = new GetAnimeByIdQuery() { Id = 2 };
            var query3 = new GetAnimeByIdQuery() { Id = 3 };

            var queryMinus = new GetAnimeByIdQuery() { Id = -1 };
            var queryInexistent = new GetAnimeByIdQuery() { Id = 15 };

            //Act

            var result1 = await handler.Handle(query1, CancellationToken.None);
            var result2 = await handler.Handle(query2, CancellationToken.None);
            var result3 = await handler.Handle(query3, CancellationToken.None);

            var resultMinus = await handler.Handle(queryMinus, CancellationToken.None);
            var resultInexistent = await handler.Handle(queryInexistent, CancellationToken.None);

            //Assert

            result1.Should().BeSameAs(anime1);
            result2.Should().BeSameAs(anime2);
            result3.Should().BeSameAs(anime3);

            resultMinus.Should().BeNull();
            resultInexistent.Should().BeNull();
        }
    }
}
