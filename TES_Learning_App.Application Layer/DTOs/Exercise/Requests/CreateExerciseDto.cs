using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Exercise.Requests
{
    public class CreateExerciseDto
    {
        [Required(ErrorMessage = "ActivityId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ActivityId must be greater than 0")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "JsonData is required")]
        public string JsonData { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "SequenceOrder must be greater than 0")]
        public int SequenceOrder { get; set; } = 1;
    }
}
