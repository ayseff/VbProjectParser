﻿using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbnfFrameworkCore.Converters
{
    public class AggregateConverter : IValueConverter
    {
        private Dictionary<Type, IValueConverter> Converters = new Dictionary<Type, IValueConverter>();

        /// <summary>
        /// Adds a value converter which can convert $ConvertsType to string and back to object
        /// </summary>
        /// <param name="ConvertsType">Type which the given Converter is supposed to handle</param>
        /// <param name="Converter"></param>
        public void Add<TObj>(IValueConverter<TObj> Converter)
        {
            if (Converter == null)
                throw new ArgumentNullException("Converter");

            Type type = typeof(TObj);

            if (Converters.ContainsKey(type))
                throw new InvalidOperationException($"A converter for type {type.FullName} has already been registered");

            Converters.Add(type, Converter);
        }

        public string ConvertToString(object value)
        {
            return ConvertToString(value, null);
        }

        /// <returns>String.Empty if value is null. Otherweise, converts with the appropriatea converter.</returns>
        public string ConvertToString(object value, Type assumeObjectIsOfType)
        {
            if (value == null)
                return string.Empty;

            Type type = assumeObjectIsOfType == null ? value.GetType() : assumeObjectIsOfType;

            if (!Converters.ContainsKey(type))
                return string.Empty; // TODO XXXXX change back
                                     //throw new InvalidOperationException(String.Format("No converter for {0} has been registered", type.FullName));

            var converter = Converters[type];
            return converter.ConvertToString(value);
        }

        public object ConvertBack(string text, Type TargetType)
        {
            if (TargetType == null)
                throw new ArgumentNullException("toType");

            if (text == null)
                throw new ArgumentNullException("text");


            if (!Converters.ContainsKey(TargetType))
                throw new InvalidOperationException($"No converter for {TargetType.FullName} has been registered");

            var converter = Converters[TargetType];
            return converter.ConvertBack(text, TargetType);
        }



    }
}
