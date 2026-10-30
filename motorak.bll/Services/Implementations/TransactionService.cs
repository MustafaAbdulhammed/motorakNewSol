

using AutoMapper;
using motorak.dal.Entities;
using Motorak.BLL.ModelVM.Transactions;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAL.Entities;
using Motorak.DAL.Repo.Abstractions;

namespace Motorak.BLL.Services.Implementations
{
    
    public class TransactionService:ITransactionService
    {
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepo transactionRepo, IMapper mapper) {
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(TransactionCreateDto dto)
        {

            var transaction = new Transactions(
                dto.PaymentMethod,
                dto.TotalPrice,
                dto.CustomerId,  // Make sure your DTO has this
                dto.CarId,       // Make sure your DTO has this
                "System"
            );
            await _transactionRepo.AddAsync(transaction);

            return transaction.Id;
        }

        public async Task DeleteAsync(int id)
        {
            await _transactionRepo.DeleteAsync(id);
        }

        public async Task<List<TransactionReadDto>>  GetAllAsync()
        {
            var result = await _transactionRepo.GetAllAsync();
            return result.Select(x => _mapper.Map<TransactionReadDto>(x)).ToList();
        }

        public async Task<TransactionReadDto?> GetByIdAsync(int id)
        {
            var result = await _transactionRepo.GetByIdAsync(id);
            var final = _mapper.Map<TransactionReadDto>(result);

            return final;
            
        }

        public async Task UpdateAsync(TransactionUpdateDto dto)
        {
            var result = await _transactionRepo.GetByIdAsync(dto.Id);
            if (result == null)
                throw new InvalidOperationException("Transaction not found");
            result.UpdateTransaction(dto.PaymentMethod, dto.TotalPrice, dto.Status);

            await _transactionRepo.UpdateAsync(result);


        }
    }
}
