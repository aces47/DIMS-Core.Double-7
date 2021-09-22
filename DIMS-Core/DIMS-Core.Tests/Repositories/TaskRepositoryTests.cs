﻿using System;
using DIMS_Core.Common.Exceptions;
using DIMS_Core.DataAccessLayer.Models;
using DIMS_Core.Tests.Repositories.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace DIMS_Core.Tests.Repositories
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly TaskRepositoryFixture _repositoryFixture;

        public TaskRepositoryTests()
        {
            _repositoryFixture = new TaskRepositoryFixture();
        }

        [Fact]
        // Note: GetAll will work always in our case. It can be thrown into EF Core.
        // But it is implementation details of EF Core so we mustn't test these cases.
        public async Task GetAll_OK()
        {
            // Act
            var result = await _repositoryFixture.Repository
                                                 .GetAll()
                                                 .ToArrayAsync();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public async Task GetById_OK()
        {
            // Act
            var result = await _repositoryFixture.Repository.GetById(_repositoryFixture.TaskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_repositoryFixture.TaskId, result.TaskId);
            Assert.Equal("Test Name", result.Name);
            Assert.Equal("Test Description", result.Description);
        }
        
        [Fact]
        public async Task GetById_EmptyId_Fail()
        {
            // Arrange
            const int id = 0;

            // Act, Assert
            await Assert.ThrowsAsync<ObjectNotFoundException>(() => _repositoryFixture.Repository.GetById(id));
        }
        
        [Fact]
        public async Task GetById_NotExistTask_Fail()
        {
            // Arrange
            const int id = int.MaxValue;

            // Act, Assert
            await Assert.ThrowsAsync<ObjectNotFoundException>(() => _repositoryFixture.Repository.GetById(id));
        }

        [Fact]
        public async Task Create_OK()
        {
            // Arrange
            var entity = new DataAccessLayer.Models.Task
                         {
                             Name = "Create",
                             Description = "Description"
                         };

            // Act
            var result = await _repositoryFixture.Repository.Create(entity);
            await _repositoryFixture.Context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(default, result.TaskId);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Description, result.Description);
        }

        [Fact]
        public async Task Create_EmptyEntity_Fail()
        {
            // Arrange Act, Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repositoryFixture.Repository.Create(null));
        }

        [Fact]
        public async Task Update_OK()
        {
            // Arrange
            var entity = new DataAccessLayer.Models.Task
                         {
                             TaskId = _repositoryFixture.TaskId,
                             Name = "TaskName",
                             Description = "Description"
                         };

            // Act
            var result = _repositoryFixture.Repository.Update(entity);
            await _repositoryFixture.Context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(default, result.TaskId);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Description, result.Description);
        }
        
        [Fact]
        public void Update_EmptyEntity_Fail()
        {
            // Arrange Act, Assert
            Assert.Throws<ArgumentNullException>(() => _repositoryFixture.Repository.Update(null));
        }

        [Fact]
        public async Task Delete_OK()
        {
            // Act
            await _repositoryFixture.Repository.Delete(_repositoryFixture.TaskId);
            await _repositoryFixture.Context.SaveChangesAsync();

            // Assert
            var deletedEntity = await _repositoryFixture.Context.Tasks.FindAsync(_repositoryFixture.TaskId);
            Assert.Null(deletedEntity);
        }
        
        [Fact]
        public async Task Delete_EmptyId_Fail()
        {
            // Arrange
            const int id = 0;

            // Act, Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repositoryFixture.Repository.Delete(id));
        }
        public void Dispose() => _repositoryFixture.Dispose();
    }
}