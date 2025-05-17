namespace Krizium.KidsReadingApp.Api.Models
{
    /// <summary>
    /// Represents a single word in a paragraph
    /// </summary>
    public class WordDto
    {
        /// <summary>
        /// Unique identifier for the word
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the paragraph this word belongs to
        /// </summary>
        public int ParagraphId { get; set; }
        
        /// <summary>
        /// The actual text of the word
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Order/position of this word in the paragraph
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Optional URL to pre-recorded audio pronunciation of the word
        /// </summary>
        public string? AudioUrl { get; set; }
        
        /// <summary>
        /// Difficulty level of the word (1-5, where 1 is easiest)
        /// </summary>
        public int? DifficultyLevel { get; set; }
        
        /// <summary>
        /// Whether this word should be highlighted as a vocabulary word
        /// </summary>
        public bool IsVocabularyWord { get; set; }
    }
}
