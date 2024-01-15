using MovieData.Helpers;
using MovieData.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMqHelper.Interfaces;

namespace MovieLibraryWorkerService.Data
{
    public class RabbitMqReplyHandlerFiller : IRabbitMqReplyHandlerFiller
    {
        private readonly IDataSource _dataSource;
        public RabbitMqReplyHandlerFiller(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Gets the functions.
        /// </summary>
        /// <returns>An IDictionary.</returns>
        public IDictionary<string, Func<JArray, Task<string>>> GetFunctions()
        {
            var functions = new Dictionary<string, Func<JArray, Task<string>>>();

            functions.Add("GetCategoriesAsync", async (JArray parameters) =>
            {
                var categories = await _dataSource.GetCategoriesAsync();
                return JsonConvert.SerializeObject(categories);
            });

            functions.Add("GetMovieListAsync", async (JArray parameters) =>
            {
                var selectAndOrder = parameters.Count > 0 ? parameters[0].ToObject<SelectAndOrder>() : null;
                var movieList = await _dataSource.GetMovieListAsync(selectAndOrder);
                return JsonConvert.SerializeObject(movieList);
            });

            functions.Add("GetMovieDtoByIdAsync", async (JArray parameters) =>
            {
                int movieId = parameters[0].ToObject<int>();
                var movieDto = await _dataSource.GetMovieDtoByIdAsync(movieId);
                return JsonConvert.SerializeObject(movieDto);
            });


            return functions;
        }
    }
}
