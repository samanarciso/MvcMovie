﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.Extensions.Logging;
using MvcMovie.Services;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movies;
        private readonly ILogger<MoviesController> _logger;
        public MoviesController(IMovieService movies, ILogger<MoviesController> logger)
        {
            _movies = movies;
            _logger = logger;

        }

        // GET: Movies
        // comment at 2:12

        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {

            IEnumerable<Movie> all = await _movies.GetAllAsync();
            IEnumerable<Movie> movies = all;
            IEnumerable<string?> genreQuery = all.Select(movie => movie.Genre).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(movie => movie.Title != null
                                        && movie.Title.ToUpper().Contains(searchString, StringComparison.OrdinalIgnoreCase));
                _logger.LogDebug("Searching by search string {searchString}", searchString);
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                _logger.LogDebug("Genre filter applied {movieGenre}", movieGenre);
                movies = movies.Where(movie => movie.Genre == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(genreQuery),
                Movies =  movies.ToList()
            };

            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            _logger.LogDebug("Displaying details for movie {id}", id);
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            _logger.LogDebug("CREATE GET");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("create POST model invalid");
                return View(movie);
            }

            await _movies.AddAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            _logger.LogDebug("Edit GET, movie id {id}", id);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit POST model is invalid");
                return View(movie);
            }

            await _movies.UpdateAsync(movie);
            return RedirectToAction(nameof(Index));

        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movies.GetByIdAsync(id);
            _logger.LogDebug("Delete GET, movie id {id}", id);
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movies.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
