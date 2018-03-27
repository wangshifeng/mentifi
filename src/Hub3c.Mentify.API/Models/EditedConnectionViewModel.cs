using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.API.Models
{
    /// <inheritdoc />
    /// <summary>
    ///   Model for Editing Connection
    /// </summary>
    public class EditedConnectionViewModel : CreatedConnectionViewModel
    {
        /// <summary>
        /// Approved = 1,Removed = 2,Rejected = 3
        /// </summary>
        public ConnectionStatus Status { get; set; }
    }
}
