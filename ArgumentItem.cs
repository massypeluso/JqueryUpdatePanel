﻿
namespace JqueryWebControls
{
    public class ArgumentItem
    {
        public ArgumentItem()
        {
        }

        public ArgumentItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
