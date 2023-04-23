using CurrieTechnologies.Razor.SweetAlert2;

namespace Sales.WEB.Services
{
    public class ToastService : IToastService
    {
        private readonly SweetAlertService sweetAlertService;

        public ToastService(SweetAlertService sweetAlertService)
        {
            this.sweetAlertService = sweetAlertService;
        }
        public async Task Success(string message)
        {
            var toast = sweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: message);
        }
    }
}
