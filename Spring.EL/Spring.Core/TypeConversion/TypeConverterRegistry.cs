#region License

/*
 * Copyright 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Resources;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using Spring.Core.TypeResolution;
using Spring.Util;

#endregion

namespace Spring.Core.TypeConversion
{
    /// <summary>
    /// Registry class that allows users to register and retrieve type converters.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    public class TypeConverterRegistry
    {
        /// <summary>
        /// Name of the .Net config section that contains Spring.Net type aliases.
        /// </summary>
        private const string TypeConvertersSectionName = "spring/typeConverters";

        private static readonly object syncRoot = new object();
        private static IDictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();
        
        /// <summary>
        /// Registers standard and configured type converters.
        /// </summary>
        static TypeConverterRegistry()
        {
            lock (syncRoot)
            {
                converters[typeof(string[])] = new StringArrayConverter();
                converters[typeof(Type)] = new RuntimeTypeConverter();
                converters[typeof(Color)] = new RGBColorConverter();
                converters[typeof(Uri)] = new UriConverter();
                converters[typeof(FileInfo)] = new FileInfoConverter();
                converters[typeof(NameValueCollection)] = new NameValueConverter();
                converters[typeof(ResourceManager)] = new ResourceManagerConverter();
                converters[typeof(Regex)] = new RegexConverter();
                converters[typeof(TimeSpan)] = new TimeSpanConverter();
                converters[typeof(ICredentials)] = new CredentialConverter();
                converters[typeof(NetworkCredential)] = new CredentialConverter();
        
                // register user-defined type converters
            }
        }
        
        /// <summary>
        /// Returns <see cref="TypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">Type to get the converter for.</param>
        /// <returns>a type converter for the specified type.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
        public static TypeConverter GetConverter(Type type)
        {
            AssertUtils.ArgumentNotNull(type, "type");

            TypeConverter converter;
            if (!converters.TryGetValue(type, out converter))
            {
                if (type.IsEnum)
                {
                    converter = new EnumConverter(type);
                }
                else
                {
                    converter = TypeDescriptor.GetConverter(type);
                }
            }
            
            return converter;
        }
        
        /// <summary>
        /// Registers <see cref="TypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">Type to register the converter for.</param>
        /// <param name="converter">Type converter to register.</param>
        /// <exception cref="ArgumentNullException">If either of arguments is <c>null</c>.</exception>
        public static void RegisterConverter(Type type, TypeConverter converter)
        {
            AssertUtils.ArgumentNotNull(type, "type");
            AssertUtils.ArgumentNotNull(converter, "converter");

            lock (syncRoot)
            {
                converters[type] = converter;
            }
        }

       
        
    }
}