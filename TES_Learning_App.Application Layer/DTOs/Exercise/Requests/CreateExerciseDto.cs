namespace TES_Learning_App.Application_Layer.DTOs.Exercise.Requests
{
    public class CreateExerciseDto
    {
        public int ActivityId { get; set; }
        public string JsonData { get; set; } = string.Empty;
        public int SequenceOrder { get; set; } = 1;
    }
}
