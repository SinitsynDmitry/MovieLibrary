using RabbitMqHelper.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieData.Dtos;
using Newtonsoft.Json;

namespace MovieLibrary.Data.Tests
{
    [TestClass()]
    public class MovieServiceTests
    {
        /// <summary>
        /// Gets the movie dto by id async test.
        /// </summary>
        [TestMethod()]
        public void GetMovieDtoByIdAsyncTest()
        {
            int movieId = 1;
            var expectedMovieDto = new MovieDto(1, "The Matrix", "Computer programmer", 4, 1999, "Action");

            var rabbitMqMock = new Mock<IRabbitMqRequestSender>();
            rabbitMqMock.Setup(service => service.SendAndWaitAsync(It.IsAny<object>()))
                .ReturnsAsync(JsonConvert.SerializeObject(expectedMovieDto));

            var movieService = new MovieService(rabbitMqMock.Object);

            // Act
            var resultMovieDto = movieService.GetMovieDtoByIdAsync(movieId).Result;

            Assert.IsNotNull(resultMovieDto);
            Assert.AreEqual(expectedMovieDto.Id, resultMovieDto.Id);
            Assert.AreEqual(expectedMovieDto.Title, resultMovieDto.Title);
            Assert.AreEqual(expectedMovieDto.Description, resultMovieDto.Description);
            Assert.AreEqual(expectedMovieDto.Rating, resultMovieDto.Rating);
            Assert.AreEqual(expectedMovieDto.Category, resultMovieDto.Category);
        }

        /// <summary>
        /// Gets the movie list async test.
        /// </summary>
        [TestMethod()]
        public void GetMovieListAsyncTest()
        {
            var expectedMovieList = new List<MovieLightDto>
            {
                new MovieLightDto(5, "John Wick", 4, 2014, "Action"),
                new MovieLightDto(15, "The Green Mile",  5, 1999,"Drama"),
            };

            // Mocking RabbitMQ behavior
            var rabbitMqMock = new Mock<IRabbitMqRequestSender>();
            rabbitMqMock.Setup(service => service.SendAndWaitAsync(It.IsAny<object>()))
                .ReturnsAsync(JsonConvert.SerializeObject(expectedMovieList));

            // Create an instance of the service with the mocked repository
            var movieService = new MovieService(rabbitMqMock.Object);

            // Act
            var resultMovieList = movieService.GetMovieListAsync().Result;

            // Assert
            Assert.IsNotNull(resultMovieList);
            CollectionAssert.AreEqual(expectedMovieList, resultMovieList);
        }
    }
}