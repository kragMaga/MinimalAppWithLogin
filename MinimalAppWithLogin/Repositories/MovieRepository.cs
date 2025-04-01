using Minimal_JWT.Models;

namespace Minimal_JWT.Repositories
{
    public class MovieRepository
    {
        public static List<Movie> Movies = new()
        {
            new() { Id = 1, Title = "Vikings", Description = "Story of Ragnar Lothbrok and his sons.", Rating = 7.4},
            new() { Id = 2, Title = "Money Heist", Description = "The greatest heist in history.", Rating = 8.0},
            new() { Id = 3, Title = "Peaky Blinders", Description = "The rise of the Shelbys.", Rating = 8.1},
            new() { Id = 4, Title = "Ori Oka", Description = "A betrayal story", Rating = 7.8},
            new() { Id = 5, Title = "Jumong", Description = "The story of King Jumong: The founder of Goguryeo", Rating = 7.4},
        };
    }
}