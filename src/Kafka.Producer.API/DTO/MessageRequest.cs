namespace Kafka.Producer.API.DTO
{
    public class MessageRequest
    {
        public string LicencePlate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
