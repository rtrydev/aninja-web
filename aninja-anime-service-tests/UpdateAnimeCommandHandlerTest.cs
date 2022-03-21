using aninja_anime_service.Commands;
using aninja_anime_service.Enums;
using aninja_anime_service.Handlers;
using aninja_anime_service.Models;
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

namespace aninja_anime_service_tests
{
    public class UpdateAnimeCommandHandlerTest
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

        public async Task Update_UpdateWithValidData_ReturnsResponseFromRepo()
        {
            //Arrange
            var mockRepo = new Mock<IAnimeRepository>();

            var anime3 = _data.FirstOrDefault(x => x.Id == 3);

            var animeUpdateCommand = new UpdateAnimeCommand
            {
                Id = 3,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = "NotYetAired",
                Demographic = "Shounen",
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            var convertedAnime = new Anime
            {
                Id = 3,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = Status.NotYetAired,
                Demographic = Demographic.Shounen,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            var repoResponse = new Anime
            {
                Id = 3,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = Status.NotYetAired,
                Demographic = Demographic.Shounen,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            mockRepo.Setup(x => x.GetById(3)).Returns(Task.FromResult(anime3));
            mockRepo.Setup(x => x.Update(It.IsAny<Anime>())).Returns(Task.FromResult(repoResponse));
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<Anime>(It.IsAny<UpdateAnimeCommand>()))
                .Returns(convertedAnime);


            var handler = new UpdateAnimeCommandHandler(mockMapper.Object, mockRepo.Object);

            //Act
            var updatedEntry = await handler.Handle(animeUpdateCommand, CancellationToken.None);

            //Assert
            updatedEntry.Should().BeSameAs(repoResponse);

        }
    }
}
