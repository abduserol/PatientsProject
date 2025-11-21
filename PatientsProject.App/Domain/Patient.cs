using PatientsProject.Core.App.Domain;

namespace PatientsProject.App.Domain
{
    public class Patient : Entity
    {
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public List<DoctorPatient> DoctorPatients { get; set; }
    }
}