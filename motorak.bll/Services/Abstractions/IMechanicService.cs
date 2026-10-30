using Motorak.BLL.ModelVM.Mechanic;

namespace Motorak.BLL.Services.Abstractions
{
    public interface IMechanicService
    {
        Task<(bool status, string message)> CreateMechanicAsync(CreateMechanicModel model);
        Task<(bool status, string message, List<MechanicListModel>? mechanics)> GetAllMechanicsAsync();
        Task<(bool status, string message, MechanicDetailsModel? mechanic)> GetMechanicByIdAsync(int id);
        Task<(bool status, string message)> EditMechanicAsync(EditMechanicModel model);
        Task<(bool status, string message)> DeleteMechanicAsync(int id);
    }
}