﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fleet.Core.Entities
{
    /// <summary>
    /// Stałe lub tymczasowe przychody użytkownika
    /// </summary>
    [Table("przychody_użytkownika")]
    public class IncomeEntity : BaseEntity
    {
        #region Columns

        [Column("kwota_przychodu")]
        public double Income { get; set; }

        [Column("Źródło")]
        public string Source { get; set; }

        /// <summary>
        /// Co ile dni wpływają na konto środki pieniężne
        /// </summary>
        [Column("cykliczność_dni")]
        public int PeriodicityDay { get; set; }

        /// <summary>
        /// Data kolejnego wpływu danego środku pieniężnego
        /// </summary>
        [Column("kolejny_przychód")]
        public DateTime NextIncome { get; set; }

        #endregion
        
        #region Relations

        public AccountEntity Account { get; set; }
        public int AccountId { get; set; }
        
        #endregion
    }
}