/******************************************************************************
 *
 * File: MainViewModel.cs
 *
 * Description: MainViewModel.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieData.Dtos;
using MovieData.Entitys;
using MovieData.Helpers;
using MovieData.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MovieLibrary.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Private

        private readonly IDataSource _movieService;

        [ObservableProperty]
        private Category _category;

        private ObservableCollection<MovieLightDto> _filteredMovies = new ObservableCollection<MovieLightDto>();

        private string _orderBy;

        [ObservableProperty]
        private string _searchTerm;

        [ObservableProperty]
        private MovieDto _selectedMovie;

        [ObservableProperty]
        private MovieLightDto _selectedRow;

        #endregion Private

        #region Collections

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        /// <summary>
        /// Gets or sets the filtered movies.
        /// </summary>
        public ObservableCollection<MovieLightDto> FilteredMovies
        {
            get => _filteredMovies;
            set
            {
                _filteredMovies = value;
                OnPropertyChanged(nameof(FilteredMovies));
            }
        }
        /// <summary>
        /// Gets or sets the selected categories.
        /// </summary>
        public ObservableCollection<Category> SelectedCategories { get; set; } = new ObservableCollection<Category>();

        #endregion Collections

        #region Commands

        /// <summary>
        /// Gets the filter clean command.
        /// </summary>
        public IAsyncRelayCommand CleanFiltersCommand { get; }

        /// <summary>
        /// Gets the order changed command.
        /// </summary>
        public IAsyncRelayCommand OrderChangedCommand { get; }

        #endregion Commands

        #region Public Constructors

        public MainViewModel(IDataSource movieService)
        {
            _movieService = movieService;

            CleanFiltersCommand = new AsyncRelayCommand(CleanFiltersAsync);
            OrderChangedCommand = new AsyncRelayCommand<DataGridColumnHeader>(OrderByChangedAsync);
            SelectedCategories.CollectionChanged += SelectedCategories_CollectionChanged;
            Init();

            this.PropertyChanged += MainViewModel_PropertyChanged;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Shows the exception.
        /// </summary>
        /// <param name="toex">The toex.</param>
        private static void ShowException(TimeoutException toex)
        {
            MessageBox.Show(toex.Message, "Movie Library", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Filters the clean async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task CleanFiltersAsync()
        {
            this.Category = null;
            SearchTerm = "";
        }

        /// <summary>
        /// Filters the movies.
        /// </summary>
        private async void FilterMovies()
        {
            var action = () =>
            {
                FilteredMovies.Clear();
            };

            DispatcherInvoke(action);

            var select = new SelectAndOrder()
            {
                SearchTerm = this.SearchTerm,
                SelectedCategories = this.SelectedCategories.Select(cat => cat.Id).ToList(),
                Order_by = _orderBy
            };
            try
            {
                var filteredMovies = await _movieService.GetMovieListAsync(select);

                if (filteredMovies != null)
                {
                    action = () =>
                    {
                        foreach (var item in filteredMovies)
                        {
                            FilteredMovies.Add(item);
                        }
                    };

                    DispatcherInvoke(action);
                }
            }
            catch (TimeoutException toex)
            {
                ShowException(toex);
            }
        }

        /// <summary>
        /// Gets the movie by id async.
        /// </summary>
        private async void GetMovieByIdAsync()
        {
            var _movieId = SelectedRow?.Id;

            if (_movieId == null || _movieId <= 0)
            {
                return;
            }

            try
            {
                var movie = await _movieService.GetMovieDtoByIdAsync(_movieId.Value);
                if (movie == null)
                {
                    return;
                }
                SelectedMovie = movie;
            }
            catch (TimeoutException toex)
            {
                ShowException(toex);
            }
        }

        /// <summary>
        /// Gets the sort direction.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>A bool.</returns>
        private bool GetSortDirection(DataGridColumn column)
        {
            // Check if the column is sorted
            if (column != null && column.SortDirection.HasValue)
            {
                return column.SortDirection.Value != ListSortDirection.Ascending;
            }

            // If not sorted, assume ascending by default
            return true;
        }

        /// <summary>
        /// Inits the.
        /// </summary>
        private async void Init()
        {
            try
            {
                var _categories = await _movieService.GetCategoriesAsync();
                if (_categories != null)
                {
                    var action = () =>
                    {
                        foreach (var item in _categories)
                        {
                            Categories.Add(item);
                        }
                    };

                    DispatcherInvoke(action);
                }

                // Initially load all movies
                FilterMovies();
            }
            catch (TimeoutException toex)
            {
                ShowException(toex);
                // throw;
            }
        }

        /// <summary>
        /// Dispatchers the invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        private void DispatcherInvoke(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                // You are on a different thread, use Dispatcher.Invoke
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action();
                });
            }
        }

        /// <summary>
        /// Mains the view model_ property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchTerm))
            {
                FilterMovies();
            }
            if (e.PropertyName == nameof(SelectedRow) && SelectedRow?.Id > 0)
            {
                GetMovieByIdAsync();
            }
        }

        /// <summary>
        /// Orders the by changed async.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>A Task.</returns>
        private async Task OrderByChangedAsync(DataGridColumnHeader header)
        {
            if (header == null || header.Column == null)
            {
                return;
            }
            var column = header.Column;
            bool sortDirection = GetSortDirection(column);
            _orderBy = $"{column.SortMemberPath}_{(sortDirection ? "asc" : "desc")}";
            FilterMovies();
        }

        /// <summary>
        /// Selecteds the categories_ collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectedCategories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterMovies();
        }

        #endregion Private Methods
    }
}