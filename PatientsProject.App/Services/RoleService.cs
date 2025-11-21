using Microsoft.EntityFrameworkCore;
using PatientsProject.App.Domain;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Models;
using PatientsProject.Core.App.Services;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.App.Services;

public class RoleService : Service<Role>, IService<RoleRequest, RoleResponse>
{
    public RoleService(DbContext db) : base(db) { }

    public CommandResponse Create(RoleRequest request)
    {
        if (Query().Any(r => r.Name.ToLower() == request.Name.Trim().ToLower()))
            return Error("Role with the same name exists!");

        var entity = new Role { Name = request.Name.Trim() };
        Create(entity);
        return Success("Role created successfully.", entity.Id);
    }

    public CommandResponse Update(RoleRequest request)
    {
        if (Query().Any(r => r.Id != request.Id && r.Name.ToLower() == request.Name.Trim().ToLower()))
            return Error("Role with the same name exists!");

        var entity = Query(false).SingleOrDefault(r => r.Id == request.Id);
        if (entity is null) return Error("Role not found!");

        entity.Name = request.Name.Trim();
        Update(entity);
        return Success("Role updated successfully.", entity.Id);
    }

    public CommandResponse Delete(int id)
    {
        var entity = Query(false).Include(r => r.UserRoles).SingleOrDefault(r => r.Id == id);
        if (entity is null) return Error("Role not found!");

        if (entity.UserRoles.Any())
            return Error("Role cannot be deleted because it is assigned to users!");

        Delete(entity);
        return Success("Role deleted successfully.", entity.Id);
    }

    public RoleRequest Edit(int id)
    {
        var entity = Query().SingleOrDefault(r => r.Id == id);
        return entity == null ? null : new RoleRequest { Id = entity.Id, Name = entity.Name };
    }

    public RoleResponse Item(int id)
    {
        var entity = Query().SingleOrDefault(r => r.Id == id);
        return entity == null ? null : new RoleResponse { Id = entity.Id, Name = entity.Name };
    }

    public List<RoleResponse> List()
    {
        return Query().Select(r => new RoleResponse { Id = r.Id, Name = r.Name }).ToList();
    }
}