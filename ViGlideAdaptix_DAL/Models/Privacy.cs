﻿using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class Privacy
{
    public int PrivacyId { get; set; }

    public string? Detail { get; set; }

    public bool Status { get; set; }
}