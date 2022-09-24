﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Fleet.Core.Entities
{
    public class TransactionEntity : BaseEntity
    {
        #region Columns

        [Column("ilość")]
        [Required]
        public double Quantity { get; set; }

        [Column("data_transakcji")]
        [Required]
        public DateTime TransactionDate { get; set; }

        [Column("waluta")]
        public string Currency { get; set; }

        [Column("zapłacono")]
        public double Paid { get; set; }
        
        #endregion
        
        #region Relations

        public int AccountId { get; set; }
        public AccountEntity Account { get; set; }

        [AllowNull]
        public int? ProductId { get; set; }

        public ProductEntity Product { get; set; }
        
        #endregion
    }
}