﻿using System;
using System.Threading.Tasks;
using Fleet.Core.ApiModels;
using Fleet.Core.Dtos;
using Fleet.Core.Entities;
using Fleet.Core.Interfaces.Repositories;
using Fleet.Core.Interfaces.Services;
using Fleet.Core.Specifications;

namespace Fleet.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;

        public TransactionService( IUnitOfWork unitOfWork,
            IProductService productService )
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }
        
        public async Task<ApiResponse<TransactionDto>> CreateTransaction( TransactionDto transactionDto )
        {
            var transactionToCreate = new TransactionEntity();
         
            var placeSpec = new ProductPlaceSpecification ( transactionDto.Place );
            var place = await _unitOfWork.Repository<ProductPlaceEntity>().GetEntityWithSpecAsync ( placeSpec );

            if( place == null)
                return new ApiResponse<TransactionDto> ( 200, "Utwórz najpierw miejse transakcji", null );
            
            
            var amountTransactions = await AmountTransactionNames ( transactionDto.TransactionDate,
                transactionDto.TransactionName,
                transactionDto.AccountId );
            transactionToCreate.TransactionName =  $"{transactionDto.TransactionName}_{++amountTransactions}";
            transactionToCreate.TransactionDate = transactionDto.TransactionDate;
            transactionToCreate.Currency = transactionDto.Currency;
            transactionToCreate.TotalPaid = transactionDto.TotalPaid;
            transactionToCreate.TransactionDirectionId = transactionDto.TransactionDirectionId;
            transactionToCreate.AccountId = transactionDto.AccountId;

            _unitOfWork.Repository<TransactionEntity>().Add ( transactionToCreate );
            await _unitOfWork.CompleteAsync();
            
            foreach ( var position in transactionDto.Positions )
            {
                var res = await CreateTransactionPosition ( position, place.Id, transactionDto.AccountId );
                if( string.IsNullOrEmpty ( res.Message ) )
                {
                    var transactionPosition = (TransactionPositionsEntity) res.Response;
                    transactionPosition.TransactionId = transactionToCreate.Id;
                    _unitOfWork.Repository<TransactionPositionsEntity>().Add ( transactionPosition );
                }
            }
            
            await _unitOfWork.CompleteAsync();
            return new ApiResponse<TransactionDto> ( 200, "", transactionDto );
        }

        /// <summary>
        /// Zlicza ilość transakcji o danej nazwie wykonanej w danym dniu
        /// </summary>
        /// <param name="transactionDate">Data wykonanej transakcji</param>
        /// <param name="transactionName">Nazwa transakcji</param>
        /// <param name="accountId">Id konta użytkownika</param>
        /// <returns>Ilość transakcji</returns>
        private async Task<int> AmountTransactionNames( DateTime transactionDate, string transactionName, int accountId )
        {
            var transactionSpec = new TransactionSpecification ( transactionDate, transactionName, accountId );
            var transaction = await _unitOfWork.Repository<TransactionEntity>()
                .ListAsync ( transactionSpec );

            return transaction.Count;
 ;       }

        private async Task<ApiResponse<TransactionPositionsEntity>> CreateTransactionPosition( TransactionPositionsDto position, int placeId, int accountId )
        {
            var positionToCreate = new TransactionPositionsEntity();
            positionToCreate.Paid = position.Paid;
            positionToCreate.Quantity = position.Quantity;

            var response = new ApiResponse<TransactionPositionsEntity> ( 200, "", null );

            // pobieram produkt po nazwie dla danego sklepu
            var product = await _productService.GetProduct ( position.ProductName, placeId, accountId );
            
            // jesli istnieje to z niego korzystam
            if( product != null )
            {
                positionToCreate.ProductId = product.Id;
                positionToCreate.Product = product;
                
            }
            // jeśli nie to tworzę nowy
            else
            {
                var catalogSpec = new CatalogSpecification ( position.CatalogName, accountId );
                var catalogEntity = await _unitOfWork.Repository<CatalogEntity>().GetEntityWithSpecAsync ( catalogSpec );


                // tworzenie produktu
                var productEntity = new ProductEntity
                {
                    AccountId = accountId,
                    CatalogId = catalogEntity.Id,
                    Price = position.Paid / position.Quantity,
                    ProductName = position.ProductName,
                    Unit = position.Unit,
                    productPlaceId = placeId
                };
                await _productService.CreateProduct ( productEntity );
                
                positionToCreate.ProductId = productEntity.Id;
            }

            response.Response = positionToCreate;


            return new ApiResponse<TransactionPositionsEntity> ( 200, "", response.Response );
        }
        
        
    }
}