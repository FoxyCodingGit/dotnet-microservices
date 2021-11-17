using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos 
{
    public class PlatformCreateDto 
    {
        // LEARNING-NOTE: see how the id of the platform model is not set here. We are restricing the external user access to "id" on create as this is something we will do automaticallly on our side.

        [Required]
        public string Name { get; set; }

        [Required]
        public string Publisher { get; set; }

        [Required]
        public string Cost { get; set; }
    }
}