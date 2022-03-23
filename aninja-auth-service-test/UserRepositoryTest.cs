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

        [Fact]
        public async Task GetUserById_GetExistingUser_ReturnsUser()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();
            var user = new User() { Id = guid, Name = "test", Email = "a@a.com", Password = "asd" };
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").InsertOneAsync(user);

            //Act
            var userFromRepo = await repository.GetUserById(guid);

            //Assert
            userFromRepo.Should().NotBeNull();
            userFromRepo.Should().BeEquivalentTo(user);

            //Teardown
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindOneAndDeleteAsync(x => x.Id == guid);

        }

        [Fact]
        public async Task GetUserById_GetInexistentUser_ReturnsNull()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();

            //Act
            var userFromRepo = await repository.GetUserById(guid);

            //Assert
            userFromRepo.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByCredentials_Login_ReturnsUser()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();
            var user = new User() { Id = guid, Name = "test", Email = "a@a.com", Password = "asd" };
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").InsertOneAsync(user);

            //Act
            var userFromRepo = await repository.GetUserByCredentials(user.Name, user.Password);

            //Assert
            userFromRepo.Should().NotBeNull();
            userFromRepo.Should().BeEquivalentTo(user);

            //Teardown
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindOneAndDeleteAsync(x => x.Id == guid);

        }

        [Fact]
        public async Task GetUserByCredentials_LoginWrong_ReturnsNull()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();
            var user = new User() { Id = guid, Name = "test", Email = "a@a.com", Password = "asd" };
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").InsertOneAsync(user);

            //Act
            var userWrongPass = await repository.GetUserByCredentials(user.Name, "someotherpass");
            var userWrongName = await repository.GetUserByCredentials("somename", user.Password);
            var userBothWrong = await repository.GetUserByCredentials("somename", "somepass");

            //Assert
            userWrongPass.Should().BeNull();
            userWrongName.Should().BeNull();
            userBothWrong.Should().BeNull();

            //Teardown
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindOneAndDeleteAsync(x => x.Id == guid);
        }

        [Fact]
        public async Task UpdateUser_ChangeEmail_UpdatesUser()
        {
            //Arrange
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var repository = new UserRepository(mongoClient);

            var guid = Guid.NewGuid();
            var user = new User() { Id = guid, Name = "test", Email = "a@a.com", Password = "asd" };
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").InsertOneAsync(user);

            //Act
            user.Email = "b@b.com";
            await repository.UpdateUser(user);
            var userAfterUpdate = (await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindAsync(x => x.Id == guid)).First();

            //Assert
            userAfterUpdate.Should().NotBeNull();
            userAfterUpdate.Should().BeEquivalentTo(user);

            //Teardown
            await mongoClient.GetDatabase("usersDB").GetCollection<User>("users").FindOneAndDeleteAsync(x => x.Id == guid);
        }


    }
}
