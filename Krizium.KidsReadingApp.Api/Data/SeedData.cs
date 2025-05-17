using Krizium.KidsReadingApp.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Check if there's already data in the database
            if (context.Books.Any())
            {
                return; // DB has been seeded
            }

            // Create sample books
            await SeedBooksAsync(context);
        }

        private static async Task SeedBooksAsync(AppDbContext context)
        {
            // Book 1: Max's Adventure
            var maxAdventure = new Book
            {
                Title = "Max's Adventure in the Forest",
                Author = "Sarah Johnson",
                CoverImageUrl = "https://api.kidsreadingapp.com/images/max_adventure.jpg",
                AgeRangeMin = 3,
                AgeRangeMax = 7,
                Description = "Join Max the fox as he explores the forest and makes new friends.",
                Categories = "Adventure,Animals,Nature",
                DateAdded = DateTime.UtcNow,
                IsActive = true
            };

            context.Books.Add(maxAdventure);
            await context.SaveChangesAsync();

            // Add pages to Max's Adventure
            await AddMaxAdventurePagesAsync(context, maxAdventure.Id);

            // Book 2: The Counting Game
            var countingGame = new Book
            {
                Title = "The Counting Game",
                Author = "Michael Roberts",
                CoverImageUrl = "https://api.kidsreadingapp.com/images/counting_game.jpg",
                AgeRangeMin = 2,
                AgeRangeMax = 5,
                Description = "Learn to count from one to ten with colorful pictures and simple text.",
                Categories = "Educational,Numbers,Learning",
                DateAdded = DateTime.UtcNow,
                IsActive = true
            };

            context.Books.Add(countingGame);
            await context.SaveChangesAsync();

            // Add pages to The Counting Game
            await AddCountingGamePagesAsync(context, countingGame.Id);

            // Book 3: Colors All Around
            var colorsBook = new Book
            {
                Title = "Colors All Around",
                Author = "Emily Chen",
                CoverImageUrl = "https://api.kidsreadingapp.com/images/colors.jpg",
                AgeRangeMin = 2,
                AgeRangeMax = 6,
                Description = "Explore the wonderful world of colors in everyday objects.",
                Categories = "Educational,Colors,Learning",
                DateAdded = DateTime.UtcNow,
                IsActive = true
            };

            context.Books.Add(colorsBook);
            await context.SaveChangesAsync();

            // Add pages to Colors All Around
            await AddColorsBookPagesAsync(context, colorsBook.Id);
        }

        private static async Task AddMaxAdventurePagesAsync(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new Page
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "https://api.kidsreadingapp.com/images/max_adventure_1.jpg"
            };

            context.Pages.Add(page1);
            await context.SaveChangesAsync();

            // Add paragraph to page 1
            var paragraph1 = new Paragraph
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words = "One sunny morning Max the curious little fox went exploring in the big green forest.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words[i])
                });
            }

            await context.SaveChangesAsync();

            // Page 2
            var page2 = new Page
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "https://api.kidsreadingapp.com/images/max_adventure_2.jpg"
            };

            context.Pages.Add(page2);
            await context.SaveChangesAsync();

            // Add paragraph to page 2
            var paragraph2 = new Paragraph
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words2 = "He found a shiny blue feather on the path, made friends with a happy squirrel, and shared his lunch with a hungry rabbit.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words2[i]),
                    IsVocabularyWord = IsVocabularyWord(words2[i]),
                    Definition = GetWordDefinition(words2[i])
                });
            }

            await context.SaveChangesAsync();

            // Page 3
            var page3 = new Page
            {
                BookId = bookId,
                PageNumber = 3,
                ImageUrl = "https://api.kidsreadingapp.com/images/max_adventure_3.jpg"
            };

            context.Pages.Add(page3);
            await context.SaveChangesAsync();

            // Add paragraph to page 3
            var paragraph3 = new Paragraph
            {
                PageId = page3.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph3);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words3 = "It was the best adventure ever!".Split(' ');

            for (int i = 0; i < words3.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph3.Id,
                    Text = words3[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words3[i])
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task AddCountingGamePagesAsync(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new Page
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "https://api.kidsreadingapp.com/images/counting_1.jpg"
            };

            context.Pages.Add(page1);
            await context.SaveChangesAsync();

            // Add paragraph to page 1
            var paragraph1 = new Paragraph
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words = "I can count to ten! Let's count together.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words[i])
                });
            }

            await context.SaveChangesAsync();

            // Page 2
            var page2 = new Page
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "https://api.kidsreadingapp.com/images/counting_2.jpg"
            };

            context.Pages.Add(page2);
            await context.SaveChangesAsync();

            // Add paragraph to page 2
            var paragraph2 = new Paragraph
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words2 = "One apple. Two bananas. Three oranges.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words2[i]),
                    IsVocabularyWord = IsNumberWord(words2[i]),
                    Definition = GetNumberDefinition(words2[i])
                });
            }

            await context.SaveChangesAsync();

            // Add more pages...
        }

        private static async Task AddColorsBookPagesAsync(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new Page
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "https://api.kidsreadingapp.com/images/colors_1.jpg"
            };

            context.Pages.Add(page1);
            await context.SaveChangesAsync();

            // Add paragraph to page 1
            var paragraph1 = new Paragraph
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words = "Colors are everywhere! Let's learn about colors.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words[i])
                });
            }

            await context.SaveChangesAsync();

            // Page 2
            var page2 = new Page
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "https://api.kidsreadingapp.com/images/colors_2.jpg",
                BackgroundColor = "#FFCCCC" // Light red background
            };

            context.Pages.Add(page2);
            await context.SaveChangesAsync();

            // Add paragraph to page 2
            var paragraph2 = new Paragraph
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            await context.SaveChangesAsync();

            // Add words to paragraph
            string[] words2 = "Red is the color of apples and strawberries.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new Word
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    DifficultyLevel = GetWordDifficulty(words2[i]),
                    IsVocabularyWord = IsColorWord(words2[i]),
                    Definition = GetColorDefinition(words2[i])
                });
            }

            await context.SaveChangesAsync();

            // Add more pages for other colors...
        }

        #region Helper Methods

        private static int GetWordDifficulty(string word)
        {
            // Simple algorithm to determine word difficulty
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            // Very easy words (3 letters or less, common words)
            if (word.Length <= 3 || word == "the" || word == "and" || word == "was" || word == "is" || word == "are" || word == "to")
            {
                return 1;
            }
            
            // Easy words (4-5 letters, common words)
            if (word.Length <= 5 || word == "about" || word == "with" || word == "that" || word == "have" || word == "this")
            {
                return 2;
            }
            
            // Medium words (6-7 letters)
            if (word.Length <= 7)
            {
                return 3;
            }
            
            // Hard words (8-9 letters)
            if (word.Length <= 9)
            {
                return 4;
            }
            
            // Very hard words (10+ letters)
            return 5;
        }

        private static bool IsVocabularyWord(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            // Common vocabulary words for children's stories
            string[] vocabularyWords = 
            {
                "curious", "exploring", "adventure", "forest", "feather", "shiny", "shared", "squirrel", "rabbit"
            };
            
            return vocabularyWords.Contains(word);
        }

        private static string? GetWordDefinition(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            return word switch
            {
                "curious" => "Wanting to learn or know more about something",
                "exploring" => "Traveling through a place to find out about it",
                "adventure" => "An exciting or unusual experience",
                "forest" => "A large area covered with trees",
                "feather" => "One of the light, flat growths forming the outer covering of a bird's body",
                "shiny" => "Reflecting light; bright and glossy",
                "shared" => "Given a portion of something to someone else",
                "squirrel" => "A small animal with a long bushy tail that lives in trees",
                "rabbit" => "A small animal with long ears and a short tail",
                _ => null
            };
        }

        private static bool IsNumberWord(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            string[] numberWords = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
            
            return numberWords.Contains(word);
        }
        
        private static string? GetNumberDefinition(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            return word switch
            {
                "one" => "The number 1",
                "two" => "The number 2",
                "three" => "The number 3",
                "four" => "The number 4",
                "five" => "The number 5",
                "six" => "The number 6",
                "seven" => "The number 7",
                "eight" => "The number 8",
                "nine" => "The number 9",
                "ten" => "The number 10",
                _ => null
            };
        }
        
        private static bool IsColorWord(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            string[] colorWords = { "red", "blue", "green", "yellow", "orange", "purple", "pink", "brown", "black", "white" };
            
            return colorWords.Contains(word);
        }
        
        private static string? GetColorDefinition(string word)
        {
            word = word.ToLower().Trim('.', ',', '!', '?');
            
            return word switch
            {
                "red" => "The color of blood or a ruby",
                "blue" => "The color of the sky or the ocean",
                "green" => "The color of grass or leaves",
                "yellow" => "The color of the sun or a banana",
                "orange" => "The color of a carrot or a pumpkin",
                "purple" => "The color of grapes or lavender",
                "pink" => "A light reddish color like a flamingo",
                "brown" => "The color of chocolate or tree bark",
                "black" => "The darkest color, like the night sky without stars",
                "white" => "The color of snow or milk",
                _ => null
            };
        }

        #endregion
    }
}
        