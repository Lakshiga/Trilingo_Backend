using System;

namespace TES_Learning_App.Application_Layer.DTOs.Exercise.Response
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string JsonData { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
