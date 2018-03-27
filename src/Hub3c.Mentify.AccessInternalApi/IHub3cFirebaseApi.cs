using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi.Models;

namespace Hub3c.Mentify.AccessInternalApi
{
    // ReSharper disable once InconsistentNaming
    public interface IHub3cFirebaseApi
    {
        Task<object> Send(Hub3cFirebase model);
    }
}
