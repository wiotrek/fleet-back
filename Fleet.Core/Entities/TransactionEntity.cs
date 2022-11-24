﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fleet.Core.Entities
{
    [Table("transakcja")]
    public class TransactionEntity : BaseEntity
    {
        #region Columns

        [Column("data_transakcji")]
        [Required]
        public DateTime TransactionDate { get; set; }
        
        [Column("waluta")]
        public string Currency { get; set; }
        
        [Column("zapłacono_łącznie")]
        public double TotalPaid { get; set; }
        
        [Column("nazwa_transakcji")]
        public string TransactionName { get; set; }
        
        #endregion
        
        #region Relations

        public int AccountId { get; set; }
        public AccountEntity Account { get; set; }
        
        public int TransactionDirectionId { get; set; }
        public TransactionDirectionEntity TransactionDirection { get; set; }
        
        public List<TransactionPostionsEntity> TransactionPostions { get; set; }

        #endregion
    }
}