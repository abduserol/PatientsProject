using Microsoft.EntityFrameworkCore;
using PatientsProject.App.Domain;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Models;
using PatientsProject.Core.App.Services;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.App.Services;

public class PatientService : Service<Patient>, IService<PatientRequest, PatientResponse>
{
    public PatientService(DbContext db) : base(db)
    {
    }

    protected override IQueryable<Patient> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking)
            .Include(p => p.DoctorPatients).ThenInclude(dp => dp.Doctor)
            .OrderByDescending(p =>
                p.Id);
    }

    public List<PatientResponse> List()
    {
        return Query().Select(p => new PatientResponse
        {
            Id = p.Id,
        
            // Boy ve Kilo Formatlama
            HeightF = p.Height.HasValue ? p.Height.Value.ToString("N2") + " cm" : "-",
            WeightF = p.Weight.HasValue ? p.Weight.Value.ToString("N2") + " kg" : "-",
        
            UserName = p.User != null ? p.User.UserName : "-"

        }).ToList();
    }

    public PatientResponse Item(int id)
    {
        var entity = Query().SingleOrDefault(p => p.Id == id);
        if (entity is null)
            return null;

        return new PatientResponse
        {
            Id = entity.Id,
            Guid = entity.Guid,
            Height = entity.Height,
            Weight = entity.Weight,

            HeightF = entity.Height.HasValue ? entity.Height.Value.ToString("N2") + " cm" : "-",
            WeightF = entity.Weight.HasValue ? entity.Weight.Value.ToString("N2") + " kg" : "-",

            Doctors = entity.DoctorPatients.Select(dp => dp.Doctor.Id.ToString())
                .ToList(),
            DoctorIds = entity.DoctorPatients.Select(dp => dp.DoctorId).ToList()
        };
    }

    public PatientRequest Edit(int id)
    {
        var entity = Query().SingleOrDefault(p => p.Id == id);
        if (entity is null)
            return null;

        return new PatientRequest
        {
            Id = entity.Id,
            Height = entity.Height,
            Weight = entity.Weight,
            DoctorIds = entity.DoctorPatients.Select(dp => dp.DoctorId).ToList()
        };
    }

    public CommandResponse Create(PatientRequest request)
    {
        var entity = new Patient
        {
            Height = request.Height,
            Weight = request.Weight,

            DoctorPatients = request.DoctorIds.Select(doctorId => new DoctorPatient
            {
                DoctorId = doctorId
            }).ToList()
        };

        Create(entity);

        return new CommandResponse(true, "Patient created successfully.", entity.Id);
    }

    public CommandResponse Update(PatientRequest request)
    {
        var entity = Query(false).SingleOrDefault(p => p.Id == request.Id);

        if (entity is null)
            return new CommandResponse(false, "Patient not found!");

        Delete(entity.DoctorPatients);

        entity.Height = request.Height;
        entity.Weight = request.Weight;

        entity.DoctorPatients = request.DoctorIds.Select(doctorId => new DoctorPatient
        {
            PatientId = entity.Id,
            DoctorId = doctorId
        }).ToList();

        Update(entity);

        return new CommandResponse(true, "Patient updated successfully.", entity.Id);
    }

    public CommandResponse Delete(int id)
    {
        var entity = Query(false).SingleOrDefault(p => p.Id == id);
        if (entity is null)
            return new CommandResponse(false, "Patient not found!");

        Delete(entity.DoctorPatients);

        Delete(entity);

        return new CommandResponse(true, "Patient deleted successfully.", entity.Id);
    }
}