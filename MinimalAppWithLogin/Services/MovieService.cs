﻿using Minimal_JWT.Models;
using Minimal_JWT.Repositories;

namespace Minimal_JWT.Services
{
    public class MovieService : IMovieService
    {
        public Movie Create(Movie movie)
        {
            movie.Id = MovieRepository.Movies.Count + 1;
            MovieRepository.Movies.Add(movie);

            return movie;
        }

        public Movie Get(int id)
        {
            var movie = MovieRepository.Movies.FirstOrDefault(e => e.Id == id);
            if (movie is null) return null;

            return movie;
        }

        public List<Movie> List()
        {
            var movies = MovieRepository.Movies;
            return movies;
        }

        public Movie Update(Movie newMovie)
        {
            var oldMovie = MovieRepository.Movies.FirstOrDefault(e =>e.Id == newMovie.Id);

            if (oldMovie is null) return null;

            oldMovie.Title = newMovie.Title;
            oldMovie.Description = newMovie.Description;
            oldMovie.Rating = newMovie.Rating;

            return newMovie;
        }

        public bool Delete(int Id)
        {
            var oldMovie = MovieRepository.Movies.FirstOrDefault(e => e.Id == Id);

            if (oldMovie is null) return false;

            MovieRepository.Movies.Remove(oldMovie);

            return true;
        }
    }
}
