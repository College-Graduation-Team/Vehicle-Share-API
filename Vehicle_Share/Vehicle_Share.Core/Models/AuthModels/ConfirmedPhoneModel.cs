using System.ComponentModel.DataAnnotations;
using Vehicle_Share.Core.SharedResources;

namespace Vehicle_Share.Core.Models.AuthModels
{
    public class ConfirmPhoneModel
	{
		[Required(ErrorMessageResourceName = SharedResourcesKey.PhoneRequired, ErrorMessageResourceType = typeof(SharedResources.SharedResources))]
		[Phone(ErrorMessageResourceName = SharedResourcesKey.PhoneInvalid, ErrorMessageResourceType = typeof(SharedResources.SharedResources))]
		public string Phone { get; set; } = string.Empty;

		[Required(ErrorMessageResourceName = SharedResourcesKey.CodeRequired, ErrorMessageResourceType = typeof(SharedResources.SharedResources))]
		[Range(99999, 999999, ErrorMessageResourceName = SharedResourcesKey.CodeInvalid, ErrorMessageResourceType = typeof(SharedResources.SharedResources))]
		public string Code { get; set; } = string.Empty;
	}
}
