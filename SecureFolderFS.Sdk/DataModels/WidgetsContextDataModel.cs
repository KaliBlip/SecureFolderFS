﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.DataModels
{
    [Serializable]
    public sealed class WidgetsContextDataModel
    {
        public Dictionary<string, WidgetDataModel>? WidgetDataModels { get; set; }
    }
}
