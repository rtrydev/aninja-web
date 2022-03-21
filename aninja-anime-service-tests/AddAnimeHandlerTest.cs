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
using Xunit;

namespace aninja_anime_service_tests
{
    public class AddAnimeHandlerTest
    {
        [Fact]
        public async Task AddAnimeCommandHandler_ProcessValidCommand_ReturnsReplyFromRepo()
        {
            //Arrange
            var animeAddCommand = new AddAnimeCommand
            {
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = "NotYetAired",
                Demographic = "Shoujo",
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            var convertedAnime = new Anime
            {
                Id = 0,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = Status.NotYetAired,
                Demographic = Demographic.Shoujo,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            var repoResponse = new Anime
            {
                Id = 4,
                OriginalTitle = "たちつてと",
                TranslatedTitle = "Title3",
                Status = Status.NotYetAired,
                Demographic = Demographic.Shoujo,
                Description = "test desc3",
                ImgUrl = null,
                EndDate = new DateTime(),
                StartDate = new DateTime(2023, 12, 1),
                EpisodeCount = 0
            };

            var mockRepo = new Mock<IAnimeRepository>();
            mockRepo.Setup(x => x.Create(It.IsAny<Anime>())).Returns(Task.FromResult(repoResponse));


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<Anime>(It.IsAny<AddAnimeCommand>()))
                .Returns(convertedAnime);

            var handler = new AddAnimeCommandHandler(mockMapper.Object, mockRepo.Object);

            //Act
            var result = await handler.Handle(animeAddCommand, CancellationToken.None);

            //Assert
            result.Should().BeSameAs(repoResponse);
        }
    }
}
