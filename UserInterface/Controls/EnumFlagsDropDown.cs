﻿//
// EnumFlagsDropDown.cs: User control to render checkboxes, representing flags in
// Enum tagged with Flags attribute, in a drop-down list.
//
// Copyright (C) 2014 Rikard Johansson
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option) any
// later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// this program. If not, see http://www.gnu.org/licenses/.
//

using System;

namespace ExifOrganizer.UI.Controls
{
    /*
     * TODO:
     *  - Handle None and All (subset) values: check if match
     *  - move bitfield (of values) to CheckBoxDrop down: then only cast from Enum to int is required (alt. use array of values)
     */

    public partial class EnumFlagsDropDown : CheckBoxDropDown
    {
        private Type enumType;
        private Enum enumValue;

        public EnumFlagsDropDown()
            : base()
        {
            InitializeComponent();
        }

        public Func<Enum, string> EnumText
        {
            get;
            set;
        }

        public Type EnumType
        {
            get { return enumType; }
            set
            {
                if (value == null)
                {
                    enumType = null;
                    Clear();
                    return;
                }

                if (!value.IsEnum)
                    throw new ArgumentException($"Type must be Enum: {value}", nameof(value));
                if (!value.HasAttributesFlags())
                    throw new ArgumentException($"Enum type must have attribute flags: {value}", nameof(value));

                if (value == enumType)
                    return;

                enumType = value;
                enumValue = null;

                Clear();
                foreach (Enum item in Enum.GetValues(value))
                {
                    if (!item.OneBitSet())
                        continue;
                    Add(new CheckBoxItem() { Value = item.GetInt64(), Text = (EnumText != null) ? EnumText(item) : item.ToString() });
                }
            }
        }

        public Enum EnumValue
        {
            get { return enumValue; }
            set
            {
                if (value == null)
                    return;
                if (enumType == null)
                    throw new ArgumentException("Enum type not yet defined", nameof(enumType));
                if (value.GetType() != enumType)
                    throw new ArgumentException($"Value must be of predefined Enum type: {enumType}", nameof(value));

                if (value == enumValue)
                    return;

                Enum previousValue = enumValue;
                enumValue = value;

                foreach (Enum item in Enum.GetValues(enumType))
                {
                    if (!item.OneBitSet())
                        continue;

                    if (previousValue != null)
                    {
                        if (item.HasFlag(previousValue) == item.HasFlag(enumValue))
                            continue; // Enum flag not altered
                    }

                    bool active = item.GetInt64() > 0 && enumValue.HasFlag(item);
                    Update(new CheckBoxItem() { Value = item.GetInt64(), Text = (EnumText != null) ? EnumText(item) : item.ToString(), Checked = active });
                }
            }
        }

        protected override void CheckedChanged(CheckBoxItem item)
        {
            base.CheckedChanged(item);

            Enum previous = enumValue;
            long value = (previous != null) ? previous.GetInt64() : 0;

            if (item.Checked)
                value |= item.Value;
            else
                value &= ~item.Value;

            enumValue = (Enum)Enum.ToObject(enumType, value);
        }
    }
}