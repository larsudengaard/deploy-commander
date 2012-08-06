using Deploy.Tasks;

namespace Deploy.Soldier.Application
{
    public interface IPawnService
    {
        IResponse Execute(ITask task);
    }
}