namespace AnimalCatalog.API.DTOs.Responses
{
    public class AnimalResponse
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; } // Enviaremos como texto para o frontend
        public string PetSize { get; set; } // Enviaremos como texto
        public DateTime BirthDate { get; set; }
        public DateTime RescueDate { get; set; }
        public string Breed { get; set; }
        public string Species { get; set; } // Enviaremos como texto
        public string Description { get; set; }
        public byte[]? AnimalPic { get; set; }
    }
}