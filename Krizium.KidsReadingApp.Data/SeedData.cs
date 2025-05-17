using Krizium.KidsReadingApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

namespace Krizium.KidsReadingApp.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context, SeedDataJson seedData)
        {
            // Check if there's already data in the database
            if (context.Books.Any())
            {
                return; // DB has been seeded
            }

            // Seed books and content
            //SeedBooks(context, seedData);
            SeedSomeBooks(context);
        }

        private static async void SeedBooks(AppDbContext context, SeedDataJson seedData)
        {
            //var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed_data.json");

            //if (!File.Exists(jsonPath))
            //{
            //    Console.WriteLine("Seed data file not found.");
            //    return;
            //}

            //var json = File.ReadAllText(jsonPath);

            //var seedData = JsonSerializer.Deserialize<SeedDataJson>(
            //    json,
            //    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            //);

            if (seedData?.Books == null) return;

            foreach (var book in seedData.Books)
            {
                var bookEntity = new BookEntity
                {
                    Title = book.Title,
                    Author = book.Author,
                    CoverImageUrl = book.CoverImageUrl,
                    AgeRangeMin = book.AgeRangeMin,
                    AgeRangeMax = book.AgeRangeMax,
                    IsAvailableOffline = book.IsAvailableOffline
                };

                context.Books.Add(bookEntity);
                context.SaveChanges();

                foreach (var page in book.Pages)
                {
                    var pageEntity = new PageEntity
                    {
                        BookId = bookEntity.Id,
                        PageNumber = page.PageNumber,
                        ImageUrl = page.ImageUrl
                    };

                    context.Pages.Add(pageEntity);
                    context.SaveChanges();

                    int count = 0;
                    foreach (var paragraph in page.Paragraphs)
                    {
                        var paragraphEntity = new ParagraphEntity
                        {
                            PageId = pageEntity.Id,
                            Order = count++
                        };

                        context.Paragraphs.Add(paragraphEntity);
                        context.SaveChanges();

                        for (int i = 0; i < paragraph.Words.Count; i++)
                        {
                            context.Words.Add(new WordEntity
                            {
                                ParagraphId = paragraphEntity.Id,
                                Text = paragraph.Words[i],
                                Order = i + 1,
                                IsAvailableOffline = false
                            });
                        }

                        context.SaveChanges();
                    }
                }
            }
        }

        private static void SeedSomeBooks(AppDbContext context)
        {
            // Sample book 1: "Max's Adventure"
            var maxAdventure = new BookEntity
            {
                Title = "Max's Adventure in the Forest",
                Author = "Sarah Johnson",
                CoverImageUrl = "images/books/max_adventure.jpg",
                AgeRangeMin = 3,
                AgeRangeMax = 7,
                IsAvailableOffline = false,
            };

            context.Books.Add(maxAdventure);
            context.SaveChanges();

            // Add pages to the book
            AddMaxAdventurePages(context, maxAdventure.Id);

            // Sample book 2: "The Counting Game"
            var countingGame = new BookEntity
            {
                Title = "The Counting Game",
                Author = "Michael Roberts",
                CoverImageUrl = "images/books/counting_game.jpg",
                AgeRangeMin = 2,
                AgeRangeMax = 5,
                IsAvailableOffline = false
            };

            context.Books.Add(countingGame);
            context.SaveChanges();

            // Add pages to the book
            AddCountingGamePages(context, countingGame.Id);

            // Sample book 3: "Colors All Around"
            var colorsBook = new BookEntity
            {
                Title = "Colors All Around",
                Author = "Emily Chen",
                CoverImageUrl = "images/books/colors.jpg",
                AgeRangeMin = 2,
                AgeRangeMax = 6,
                IsAvailableOffline = false
            };

            context.Books.Add(colorsBook);
            context.SaveChanges();

            // Add pages to the book
            AddColorsBookPages(context, colorsBook.Id);
        }

        private static void AddMaxAdventurePages(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "images/books/max_adventure_1.jpg"
            };

            context.Pages.Add(page1);
            context.SaveChanges();

            // Add paragraph to page 1
            var paragraph1 = new ParagraphEntity
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            context.SaveChanges();

            // Add words to paragraph
            string[] words = "One sunny morning, Max the curious little fox went exploring in the big green forest.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    IsAvailableOffline = false,
                    AudioCacheKey = $"audio_{words[i].ToLower()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                });
            }

            context.SaveChanges();

            // Page 2
            var page2 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "images/books/max_adventure_2.jpg"
            };

            context.Pages.Add(page2);
            context.SaveChanges();

            // Add paragraph to page 2
            var paragraph2 = new ParagraphEntity
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            context.SaveChanges();

            // Add words to paragraph
            string[] words2 = "He found a shiny blue feather on the path, made friends with a happy squirrel, and shared his lunch with a hungry rabbit.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 3
            var page3 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 3,
                ImageUrl = "images/books/max_adventure_3.jpg"
            };

            context.Pages.Add(page3);
            context.SaveChanges();

            // Add paragraph to page 3
            var paragraph3 = new ParagraphEntity
            {
                PageId = page3.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph3);
            context.SaveChanges();

            // Add words to paragraph
            string[] words3 = "It was the best adventure ever!".Split(' ');

            for (int i = 0; i < words3.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph3.Id,
                    Text = words3[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();
        }

        private static void AddCountingGamePages(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "images/books/counting_1.jpg"
            };

            context.Pages.Add(page1);
            context.SaveChanges();

            // Add paragraph to page 1
            var paragraph1 = new ParagraphEntity
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            context.SaveChanges();

            // Add words to paragraph
            string[] words = "I can count to ten! Let's count together.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 2
            var page2 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "images/books/counting_2.jpg"
            };

            context.Pages.Add(page2);
            context.SaveChanges();

            // Add paragraph to page 2
            var paragraph2 = new ParagraphEntity
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            context.SaveChanges();

            // Add words to paragraph
            string[] words2 = "One apple. Two bananas. Three oranges.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 3
            var page3 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 3,
                ImageUrl = "images/books/counting_3.jpg"
            };

            context.Pages.Add(page3);
            context.SaveChanges();

            // Add paragraph to page 3
            var paragraph3 = new ParagraphEntity
            {
                PageId = page3.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph3);
            context.SaveChanges();

            // Add words to paragraph
            string[] words3 = "Four cars. Five trains. Six boats.".Split(' ');

            for (int i = 0; i < words3.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph3.Id,
                    Text = words3[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 4
            var page4 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 4,
                ImageUrl = "images/books/counting_4.jpg"
            };

            context.Pages.Add(page4);
            context.SaveChanges();

            // Add paragraph to page 4
            var paragraph4 = new ParagraphEntity
            {
                PageId = page4.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph4);
            context.SaveChanges();

            // Add words to paragraph
            string[] words4 = "Seven stars. Eight flowers. Nine trees. Ten butterflies.".Split(' ');

            for (int i = 0; i < words4.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph4.Id,
                    Text = words4[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();
        }

        private static void AddColorsBookPages(AppDbContext context, int bookId)
        {
            // Page 1
            var page1 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 1,
                ImageUrl = "images/books/colors_1.jpg"
            };

            context.Pages.Add(page1);
            context.SaveChanges();

            // Add paragraph to page 1
            var paragraph1 = new ParagraphEntity
            {
                PageId = page1.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph1);
            context.SaveChanges();

            // Add words to paragraph
            string[] words = "Colors are everywhere! Let's learn about colors.".Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph1.Id,
                    Text = words[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 2
            var page2 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 2,
                ImageUrl = "images/books/colors_2.jpg"
            };

            context.Pages.Add(page2);
            context.SaveChanges();

            // Add paragraph to page 2
            var paragraph2 = new ParagraphEntity
            {
                PageId = page2.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph2);
            context.SaveChanges();

            // Add words to paragraph
            string[] words2 = "Red is the color of apples and strawberries.".Split(' ');

            for (int i = 0; i < words2.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph2.Id,
                    Text = words2[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 3
            var page3 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 3,
                ImageUrl = "images/books/colors_3.jpg"
            };

            context.Pages.Add(page3);
            context.SaveChanges();

            // Add paragraph to page 3
            var paragraph3 = new ParagraphEntity
            {
                PageId = page3.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph3);
            context.SaveChanges();

            // Add words to paragraph
            string[] words3 = "Blue is the color of the sky and ocean.".Split(' ');

            for (int i = 0; i < words3.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph3.Id,
                    Text = words3[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 4
            var page4 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 4,
                ImageUrl = "images/books/colors_4.jpg"
            };

            context.Pages.Add(page4);
            context.SaveChanges();

            // Add paragraph to page 4
            var paragraph4 = new ParagraphEntity
            {
                PageId = page4.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph4);
            context.SaveChanges();

            // Add words to paragraph
            string[] words4 = "Yellow is the color of bananas and the sun.".Split(' ');

            for (int i = 0; i < words4.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph4.Id,
                    Text = words4[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();

            // Page 5
            var page5 = new PageEntity
            {
                BookId = bookId,
                PageNumber = 5,
                ImageUrl = "images/books/colors_5.jpg"
            };

            context.Pages.Add(page5);
            context.SaveChanges();

            // Add paragraph to page 5
            var paragraph5 = new ParagraphEntity
            {
                PageId = page5.Id,
                Order = 1
            };

            context.Paragraphs.Add(paragraph5);
            context.SaveChanges();

            // Add words to paragraph
            string[] words5 = "Green is the color of grass and leaves.".Split(' ');

            for (int i = 0; i < words5.Length; i++)
            {
                context.Words.Add(new WordEntity
                {
                    ParagraphId = paragraph5.Id,
                    Text = words5[i],
                    Order = i + 1,
                    IsAvailableOffline = false
                });
            }

            context.SaveChanges();
        }
    }

    public class SeedDataJson
    {
        public List<BookJson> Books { get; set; }
    }

    public class BookJson
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public int AgeRangeMin { get; set; }
        public int AgeRangeMax { get; set; }
        public bool IsAvailableOffline { get; set; }
        public List<PageJson> Pages { get; set; }
    }

    public class PageJson
    {
        public int PageNumber { get; set; }
        public string ImageUrl { get; set; }
        public List<ParagraphJson> Paragraphs { get; set; }
    }

    public class ParagraphJson
    {
        public int Order { get; set; }
        public List<string> Words { get; set; }
    }

}