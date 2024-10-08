﻿using System.ComponentModel.DataAnnotations;

namespace DSDD.Automations.Payments.Views.Payer;

public enum PaymentType
{
    [Display(Name = "Banka")]
    BANK,
    [Display(Name = "Banka - upraveno")]
    BANK_OVERRIDED,
    [Display(Name = "Banka - odstraněno")]
    BANK_REMOVED,
    [Display(Name = "Manuální")]
    MANUAL
}