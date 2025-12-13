using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Student.Request
{
    public class SubmitExerciseResultDto
    {
        [Required]
        public int ExerciseId { get; set; }
        
        [Required]
        public int Score { get; set; }
        
        [Required]
        public int TimeTakenInSeconds { get; set; }
    }
}