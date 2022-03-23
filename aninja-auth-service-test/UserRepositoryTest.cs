using aninja_auth_service.Models;
using aninja_auth_service.Repositories;
using FluentAssertions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace aninja_auth_service_test
{
    public class UserRepositoryTest
    {
        [Fact]
        public async Task AddUser_AddNewUser_CreatesUser()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();

            var user = new User() { Id = guid, Name = "test", Email = "a@a.com", Password = "asd" };

            //Act
            await repository.CreateUser(user);
            var userFromDb = (await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindAsync(x => x.Id == user.Id)).First();

            //Assert
            userFromDb.Should().NotBeNull();
            userFromDb.Should().BeEquivalentTo(user);

            //Teardown
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindOneAndDeleteAsync(x => x.Id == guid);

        }


    }
}
