﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace QuickZip.Tools
{
    public class DirectoryExistsRule : ValidationRule
    {
        // source: https://www.codeproject.com/Articles/31947/WPF-AutoComplete-Folder-TextBox

        public static DirectoryExistsRule Instance = new DirectoryExistsRule();

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                if (!(value is string))
                    return new ValidationResult(false, "InvalidPath");

                 if (!Directory.Exists((string)value))
                    return new ValidationResult(false, "Path Not Found");                                        
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Invalid Path");
            }

            return new ValidationResult(true, null);
        }
    }
}
