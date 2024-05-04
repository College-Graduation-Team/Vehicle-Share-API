using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.Core.Models.GeneralModels
{
    public class UpdateStatusRequestModel
    {
        public Status Status { get; set; }
        public string? Message { get; set; }
    }
}
