using SilentLogAgent.Models;

namespace SilentLogAgent.Services
{
    public interface ISpecLoader
    {
        IReadOnlyList<LogExpectation> GetExpectations();
    }
}
