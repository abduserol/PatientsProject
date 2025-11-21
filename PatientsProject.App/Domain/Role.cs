using System.ComponentModel.DataAnnotations;
using PatientsProject.Core.App.Domain;

namespace PatientsProject.App.Domain;

public class Role : Entity
{
    [Required, StringLength(25)]
    public string Name { get; set; }

    public List<UserRole> UserRoles { get; set; } = new List<UserRole>(); 
}