using Microsoft.EntityFrameworkCore;
using PatientsProject.App.Domain;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Models;
using PatientsProject.Core.App.Services;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.App.Services;

public class DoctorService : Service<Doctor>, IService<DoctorRequest, DoctorResponse>
{
    public DoctorService(DbContext db) : base(db)
    {
    }

    protected override IQueryable<Doctor> Query(bool isNoTracking = true)
    {
        return base.Query(isNoTracking)
            .Include(d => d.Branch)
            .Include(d => d.User)
            .Include(d => d.DoctorPatients)
            .OrderBy(d => d.Id);
    }
    public CommandResponse Create(DoctorRequest request)
    {
        var entity = new Doctor
        {
            BranchId = request.BranchId ?? 0,
        };

        Create(entity);

        return Success("Doctor created successfully.", entity.Id);
    }

    public CommandResponse Update(DoctorRequest request)
    {
        var entity = Query(false).SingleOrDefault(d => d.Id == request.Id);
        if (entity is null)
            return Error("Doctor not found!");

        entity.BranchId = request.BranchId ?? 0;

        Update(entity);

        return Success("Doctor updated successfully.", entity.Id);
    }

    public CommandResponse Delete(int id)
    {
        var entity = Query(false).SingleOrDefault(d => d.Id == id);
        if (entity is null)
            return Error("Doctor not found!");


        Delete(entity.DoctorPatients.ToList());

        Delete(entity);

        return Success("Doctor deleted successfully.", entity.Id);
    }

    public DoctorRequest Edit(int id)
    {
        var entity = Query().SingleOrDefault(d => d.Id == id);
        if (entity is null) return null;

        return new DoctorRequest
        {
            Id = entity.Id,
            BranchId = entity.BranchId,
        };
    }

    public DoctorResponse Item(int id)
    {
        var entity = Query().SingleOrDefault(d => d.Id == id);
        if (entity is null) return null;

        return new DoctorResponse
        {
            Id = entity.Id,
            Guid = entity.Guid,
            BranchId = entity.BranchId,

            Branch = entity.Branch.Title,
            PatientCount = entity.DoctorPatients.Count
        };
    }

    public List<DoctorResponse> List()
    {
        return Query().Select(d => new DoctorResponse
        {
            Id = d.Id,
            Guid = d.Guid,
            BranchId = d.BranchId,
            UserId = d.UserId,
            GroupId = d.GroupId,
        
            Branch = d.Branch.Title,
            PatientCount = d.DoctorPatients.Count,

            DoctorName = d.User != null ? d.User.FirstName + " " + d.User.LastName : "No User Info" 

        }).ToList();
    }
}