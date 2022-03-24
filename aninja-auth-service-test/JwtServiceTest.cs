using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using aninja_auth_service.Authorization;
using aninja_auth_service.Models;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aninja_auth_service_test
{
    public class JwtServiceTest
    {
        [Fact]
        public async Task GetToken_GetTokenByUser_ReturnsNewToken()
        {
            //Arrange
            var loggerMock = new Mock<ILogger<JwtService>>();
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "myMessage" && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never
            );
            var jwtService = new JwtService(loggerMock.Object);
            var guid = Guid.NewGuid();

            var someUser = new User() { Id = guid, Name = "Tester", Email = "a@a.com", Password = "pass123" };
            Environment.SetEnvironmentVariable("JWT_SECRET", "someawesomejwtsecret");
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", "180");

            //Act
            var token = jwtService.GetJwtToken(someUser);
            Thread.Sleep(1000);
            var token2 = jwtService.GetJwtToken(someUser);

            //Assert
            token.Should().NotBeNull();
            token2.Should().NotBeNull();
            token.Should().NotBe(token2);

            //Teardown
            Environment.SetEnvironmentVariable("JWT_SECRET", null);
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", null);

        }

        [Fact]
        public async Task GetToken_SecretNotSet_CallsErrorLogReturnsNull()
        {
            //Arrange
            var loggerMock = new Mock<ILogger<JwtService>>();
            
            var jwtService = new JwtService(loggerMock.Object);
            var guid = Guid.NewGuid();
            var someUser = new User() { Id = guid, Name = "Tester", Email = "a@a.com", Password = "pass123" };
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", "180");

            //Act
            var token = jwtService.GetJwtToken(someUser);
            Thread.Sleep(1000);
            var token2 = jwtService.GetJwtToken(someUser);

            //Assert
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "The JWT_SECRET environmental variable has not been set" && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(2)
            );
            token.Should().BeNull();
            token2.Should().BeNull();

            //Teardown
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", null);

        }

        [Fact]
        public async Task ExpiredOrWillSoonExpire_NotExpired_ReturnsFalse()
        {
            //Arrange
            Environment.SetEnvironmentVariable("JWT_SECRET", "someawesomejwtsecret");
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", "180");
            var loggerMock = new Mock<ILogger<JwtService>>();

            var jwtService = new JwtService(loggerMock.Object);
            var guid = Guid.NewGuid();
            var someUser = new User() { Id = guid, Name = "Tester", Email = "a@a.com", Password = "pass123" };

            //Act
            var token = jwtService.GetJwtToken(someUser);

            //Assert
            jwtService.WillSoonExpire(token!).Should().BeFalse();

            //Teardown
            Environment.SetEnvironmentVariable("JWT_SECRET", null);

        }

        [Fact]
        public async Task ExpiredOrWillSoonExpire_CloseToExpirationOrExpired_ReturnsTrue()
        {
            //Arrange
            Environment.SetEnvironmentVariable("JWT_SECRET", "someawesomejwtsecret");
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", "29");
            var loggerMock = new Mock<ILogger<JwtService>>();

            var jwtService = new JwtService(loggerMock.Object);
            var guid = Guid.NewGuid();
            var someUser = new User() { Id = guid, Name = "Tester", Email = "a@a.com", Password = "pass123" };

            //Act
            var token = jwtService.GetJwtToken(someUser);

            //Assert
            jwtService.WillSoonExpire(token!).Should().BeTrue();

            //Teardown
            Environment.SetEnvironmentVariable("JWT_SECRET", null);
            Environment.SetEnvironmentVariable("JWT_EXPIREMINUTES", null);
        }
    }
}