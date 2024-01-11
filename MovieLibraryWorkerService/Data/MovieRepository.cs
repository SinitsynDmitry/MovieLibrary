/******************************************************************************
 *
 * File: MovieRepository.cs
 *
 * Description: MovieRepository.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using MovieData.Dtos;
using MovieData.Entitys;
using MovieData.Helpers;
using MovieData.Interfaces;

namespace MovieLibraryWorkerService.Data
{
    public class MovieRepository : IDataSource
    {
        private readonly List<Movie> _movies = new List<Movie>();
        private readonly List<Category> _categories = new List<Category>();

        public MovieRepository()
        {
            // Action Category
            _categories.Add(new Category(1, "Action"));

            _movies.Add(new Movie(1, "The Matrix", "Computer programmer Thomas Anderson, known by his hacking alias 'Neo', is puzzled by repeated online encounters with the phrase 'the Matrix'.", 4, 1999, 1));
            _movies.Add(new Movie(2, "Die Hard", "NYPD officer John McClane tries to save hostages in a Los Angeles skyscraper occupied by terrorists.", 5, 1988, 1));
            _movies.Add(new Movie(3, "Mad Max: Fury Road", "In a post-apocalyptic wasteland, a woman rebels against a tyrannical ruler in search of her homeland.", 4, 2015, 1));
            _movies.Add(new Movie(4, "Gladiator", "A former Roman General seeks justice after being betrayed by a corrupt prince.", 5, 2000, 1));
            _movies.Add(new Movie(5, "John Wick", "An ex-hitman seeks vengeance for the killing of his dog, a gift from his late wife.", 4, 2014, 1));

            // Drama Category
            _categories.Add(new Category(2, "Drama"));

            _movies.Add(new Movie(11, "The Shawshank Redemption", "Two imprisoned men bond over several years, finding solace and eventual redemption through acts of common decency.", 5, 1994, 2));
            _movies.Add(new Movie(12, "Forrest Gump", "The life journey of Forrest Gump, a man with low intelligence but good intentions.", 4, 1994, 2));
            _movies.Add(new Movie(13, "The Godfather", "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", 5, 1972, 2));
            _movies.Add(new Movie(14, "Schindler's List", "In German-occupied Poland during World War II, industrialist Oskar Schindler gradually becomes concerned for his Jewish workforce after witnessing their persecution by the Nazis.", 5, 1993, 2));
            _movies.Add(new Movie(15, "The Green Mile", "The lives of guards on Death Row are affected by one of their charges: a black man accused of child murder and rape, yet who has a mysterious gift.", 4, 1999, 2));

            // Comedy Category
            _categories.Add(new Category(3, "Comedy"));

            _movies.Add(new Movie(21, "The Hangover", "A bachelor party in Las Vegas turns into a wild adventure as a group of friends tries to piece together what happened.", 3, 2009, 3));
            _movies.Add(new Movie(22, "Anchorman: The Legend of Ron Burgundy", "The 1970s action news team is shaken up when a female reporter arrives.", 4, 2004, 3));
            _movies.Add(new Movie(23, "Superbad", "Two co-dependent high school seniors are forced to deal with separation anxiety after their plan to stage a booze-soaked party goes awry.", 3, 2007, 3));
            _movies.Add(new Movie(24, "Dumb and Dumber", "The cross-country adventures of two good-hearted but incredibly stupid friends.", 3, 1994, 3));
            _movies.Add(new Movie(25, "Ferris Bueller's Day Off", "A high school wise guy is determined to have a day off from school, despite what the Principal thinks of that.", 4, 1986, 3));

            // Science Fiction Category
            _categories.Add(new Category(4, "Science Fiction"));

            _movies.Add(new Movie(31, "Blade Runner", "A blade runner must pursue and terminate four replicants who stole a ship in space and have returned to Earth to find their creator.", 4, 1982, 4));
            _movies.Add(new Movie(32, "Star Wars: Episode IV - A New Hope", "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a Wookiee, and two droids to save the galaxy from the Empire's world-destroying battle station.", 5, 1977, 4));
            _movies.Add(new Movie(33, "Interstellar", "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.", 4, 2014, 4));
            _movies.Add(new Movie(34, "The Fifth Element", "In the colorful future, a cab driver unwittingly becomes the central figure in the search for a legendary cosmic weapon to keep Evil and Mr. Zorg at bay.", 3, 1997, 4));
            _movies.Add(new Movie(35, "The Terminator", "A human soldier is sent from 2029 to 1984 to stop an almost indestructible cyborg killing machine, sent from the same year, which has been programmed to execute a young woman whose unborn son is the key to humanity's future salvation.", 5, 1984, 4));

            // Fantasy Category
            _categories.Add(new Category(5, "Fantasy"));

            _movies.Add(new Movie(41, "The Lord of the Rings: The Fellowship of the Ring", "A young hobbit, Frodo Baggins, embarks on a perilous journey to destroy the One Ring and save Middle-earth.", 5, 2001, 5));
            _movies.Add(new Movie(42, "Harry Potter and the Sorcerer's Stone", "An orphaned boy enrolls in a school of wizardry, where he learns the truth about himself, his family, and the terrible evil that haunts the magical world.", 4, 2001, 5));
            _movies.Add(new Movie(43, "The Chronicles of Narnia: The Lion, the Witch and the Wardrobe", "Four kids travel through a wardrobe to the land of Narnia and learn of their destiny to free it with the guidance of a mystical lion.", 3, 2005, 5));
            _movies.Add(new Movie(44, "Pan's Labyrinth", "In the falangist Spain of 1944, the bookish young stepdaughter of a sadistic army officer escapes into an eerie but captivating fantasy world.", 4, 2006, 5));
            _movies.Add(new Movie(45, "The Princess Bride", "While home sick in bed, a young boy's grandfather reads him the story of a farmboy-turned-pirate who encounters numerous obstacles, enemies, and allies in his quest to be reunited with his true love.", 4, 1987, 5));
        }

        /// <summary>
        /// Gets the categories async.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Task.FromResult(_categories);
        }

        /// <summary>
        /// Gets the movie dto by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A Task.</returns>
        public async Task<MovieDto> GetMovieDtoByIdAsync(int id)
        {

            var movie = _movies.FirstOrDefault(m => m.Id == id);

            if (movie != null)
            {
                var category = _categories.Where(cat => cat.Id == movie.CategoryId).FirstOrDefault();

                return new MovieDto(movie.Id, movie.Title, movie.Description, movie.Rating, movie.Year, category?.Name);
            }

            return null;
        }

        /// <summary>
        /// Gets the movie list async.
        /// </summary>
        /// <param name="selectAndOrder">The select and order.</param>
        /// <returns>A Task.</returns>
        public async Task<List<MovieLightDto>> GetMovieListAsync(SelectAndOrder? selectAndOrder = null)
        {
            if (selectAndOrder == null)
                return await Task.FromResult(_movies.Select(mv => new MovieLightDto(mv.Id, mv.Title, mv.Rating, mv.Year, (_categories.Where(cat => cat.Id == mv.CategoryId).FirstOrDefault())?.Name)).ToList());

            var searchTerm = "";
            if (!string.IsNullOrWhiteSpace(selectAndOrder.SearchTerm))
            {
                searchTerm = selectAndOrder.SearchTerm.ToLower();
            }

            var order_by = "";
            if (!string.IsNullOrWhiteSpace(selectAndOrder.Order_by))
            {
                order_by = selectAndOrder.Order_by.ToLower();
            }

            var selectedCategories = selectAndOrder.SelectedCategories;

            var filteredMovies = _movies
                .Where(movie => (string.IsNullOrWhiteSpace(searchTerm) || movie.Title.ToLower().Contains(searchTerm)) &&
                                (selectedCategories?.Count == 0 || selectedCategories.Contains(movie.CategoryId)))
                .Select(mv => new MovieLightDto(mv.Id, mv.Title, mv.Rating, mv.Year, (_categories.Where(cat => cat.Id == mv.CategoryId).FirstOrDefault())?.Name));

            if (filteredMovies == null)
            {
                return await Task.FromResult(new List<MovieLightDto>());
            }

            switch (order_by)
            {
                case "title_desc": filteredMovies = filteredMovies.OrderByDescending(movie => movie.Title); break;

                case "year_desc": filteredMovies = filteredMovies.OrderByDescending(movie => movie.Year); break;
                case "year_asc": filteredMovies = filteredMovies.OrderBy(movie => movie.Year); break;

                case "category_desc": filteredMovies = filteredMovies.OrderByDescending(movie => movie.Category); break;
                case "category_asc": filteredMovies = filteredMovies.OrderBy(movie => movie.Category); break;

                case "rating_desc": filteredMovies = filteredMovies.OrderByDescending(movie => movie.Rating); break;
                case "rating_asc": filteredMovies = filteredMovies.OrderBy(movie => movie.Rating); break;

                default: filteredMovies = filteredMovies.OrderBy(movie => movie.Title); break;
            }

            return await Task.FromResult(filteredMovies.ToList());
        }
    }
}
