using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Exercise.Requests
{
    public class UpdateExerciseDto
    {
        public string? JsonData { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SequenceOrder must be greater than 0")]
        public int? SequenceOrder { get; set; }
    }
}
