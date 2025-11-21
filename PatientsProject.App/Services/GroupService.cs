using Microsoft.EntityFrameworkCore;
using PatientsProject.App.Domain;
using PatientsProject.App.Models;
using PatientsProject.Core.App.Models;
using PatientsProject.Core.App.Services;
using PatientsProject.Core.App.Services.MVC;

namespace PatientsProject.App.Services;

public class GroupService : Service<Group>, IService<GroupRequest, GroupResponse>
{
    public GroupService(DbContext db) : base(db) { }

    public CommandResponse Create(GroupRequest request)
    {
        if (Query().Any(g => g.Title.ToLower() == request.Title.Trim().ToLower()))
            return Error("Group with the same title exists!");

        var entity = new Group { Title = request.Title.Trim() };
        Create(entity);
        return Success("Group created successfully.", entity.Id);
    }

    public CommandResponse Update(GroupRequest request)
    {
        if (Query().Any(g => g.Id != request.Id && g.Title.ToLower() == request.Title.Trim().ToLower()))
            return Error("Group with the same title exists!");

        var entity = Query(false).SingleOrDefault(g => g.Id == request.Id);
        if (entity is null) return Error("Group not found!");

        entity.Title = request.Title.Trim();
        Update(entity);
        return Success("Group updated successfully.", entity.Id);
    }

    public CommandResponse Delete(int id)
    {
        var entity = Query(false).Include(g => g.Users).SingleOrDefault(g => g.Id == id);
        if (entity is null) return Error("Group not found!");

        if (entity.Users.Any())
            return Error("Group cannot be deleted because it has related users!");

        Delete(entity);
        return Success("Group deleted successfully.", entity.Id);
    }

    public GroupRequest Edit(int id)
    {
        var entity = Query().SingleOrDefault(g => g.Id == id);
        return entity == null ? null : new GroupRequest { Id = entity.Id, Title = entity.Title };
    }

    public GroupResponse Item(int id)
    {
        var entity = Query().SingleOrDefault(g => g.Id == id);
        return entity == null ? null : new GroupResponse { Id = entity.Id, Title = entity.Title };
    }

    public List<GroupResponse> List()
    {
        return Query().Select(g => new GroupResponse { Id = g.Id, Title = g.Title }).ToList();
    }
}