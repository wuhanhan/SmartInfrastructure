/*
 * Licensed to the SkyAPM under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The SkyAPM licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SkyApm.Tracing;

namespace SmartInfrastructure.SkyApm.Diagnostics.AspNetCore
{
    public class HttpRequestCarrierHeaderCollection : ICarrierHeaderCollection
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _headers;

        public HttpRequestCarrierHeaderCollection(HttpRequest httpRequest)
        {
            _headers = httpRequest.Headers.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToArray();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _headers.GetEnumerator();
        }
        
        public void Add(string key, string value)
        {
            throw new System.NotImplementedException();
        }
    }
}