using Deploy.Tasks;

namespace Deploy.Soldier.Application
{
    public interface ISoldierService
    {
        IResponse Execute(ITask task);
    }
}