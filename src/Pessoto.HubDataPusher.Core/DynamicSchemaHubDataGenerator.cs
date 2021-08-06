﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Pessoto.HubDataPusher.Core
{
    public class DynamicSchemaHubDataGenerator : IHubDataGenerator
    {
        private const double minTemperature = 20;
        private readonly Random r = new();
        private readonly string[] IdValues;
        private readonly string[] PropertyValueNames;
        private readonly Dictionary<string, object> data;

        public string IdPropertyName { get; }
        //public string IdPropertyValuePrefix { get; }
        public int NumberOfIds { get; }
        //public string ValuePropertyName { get; }
        //public int NumberOfProperties { get; }

        public DynamicSchemaHubDataGenerator(IOptions<DynamicSchemaHubDataGeneratorOptions> options)
        {
            IdPropertyName = options.Value.IdPropertyName;
            //IdPropertyValuePrefix = options.Value.IdPropertyValuePrefix;
            NumberOfIds = options.Value.NumberOfIds;
            //ValuePropertyName = options.Value.ValuePropertyName;
            //NumberOfProperties = options.Value.NumberOfProperties;

            IdValues = new string[NumberOfIds];
            for (int i = 0; i < IdValues.Length; i++)
            {
                IdValues[i] = $"{options.Value.IdPropertyValuePrefix}_{i + 1:0000000000}";//Pre-allocates all the string and avoid allocating it every time a payload is generated
            }

            PropertyValueNames = new string[options.Value.NumberOfProperties];
            for (int i = 0; i < PropertyValueNames.Length; i++)
            {
                PropertyValueNames[i] = $"{options.Value.ValuePropertyName}_{i + 1:0000000000}";//Pre-allocates all the string and avoid allocating it every time a payload is generated
            }

            data = new Dictionary<string, object>(PropertyValueNames.Length + 1);
        }

        public BinaryData GeneratePayload()
        {
            data[IdPropertyName] = IdValues[NextRandom(0, NumberOfIds)];

            for (int i = 0; i < PropertyValueNames.Length; i++)
            {
                data[PropertyValueNames[i]] = minTemperature + NextDoubleRandom() * 15;
            }

            return BinaryData.FromObjectAsJson(data);
        }

        private int NextRandom(int minInclusive, int maxExclusive)
        {
            lock (r)
            {
                return r.Next(minInclusive, maxExclusive);
            }
        }

        private double NextDoubleRandom()
        {
            lock (r)
            {
                return r.NextDouble();
            }
        }
    }
}