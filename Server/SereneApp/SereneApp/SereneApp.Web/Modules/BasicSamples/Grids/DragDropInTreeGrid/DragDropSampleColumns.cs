﻿
namespace SereneApp.BasicSamples.Columns
{
    using Serenity.ComponentModel;
    using System;

    [ColumnsScript("BasicSamples.DragDropSample")]
    [BasedOnRow(typeof(Entities.DragDropSampleRow), CheckNames = true)]
    public class DragDropSampleColumns
    {
        [EditLink, Width(300)]
        public String Title { get; set; }
    }
}