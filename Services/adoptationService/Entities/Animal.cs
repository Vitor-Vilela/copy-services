using AnimalCatalog.API.Entities.Base;
using AnimalCatalog.API.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace AnimalCatalog.API.Entities
{
    public class Animal : ModelBase
    {
        [Required]
        public int InstitutionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public ESexoAnimal Sex { get; set; }

        [Required]
        public EPorteAnimal PetSize { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime RescueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Breed { get; set; }

        [Required]
        public EEspecieAnimal Species { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public byte[]? AnimalPic { get; set; }

        public bool Status { get; set; }
    }
}