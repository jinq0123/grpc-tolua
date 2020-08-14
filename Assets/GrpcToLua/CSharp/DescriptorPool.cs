#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
// https://developers.google.com/protocol-buffers/
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using gpr = global::Google.Protobuf.Reflection;

namespace GrpcToLua
{
    using IDescriptor = gpr.IDescriptor;
    using FileDescriptor = gpr.FileDescriptor;
    using MessageDescriptor = gpr.MessageDescriptor;
    using MethodDescriptor = gpr.MethodDescriptor;
    using ServiceDescriptor = gpr.ServiceDescriptor;

    /// <summary>
    /// Contains lookup tables containing all the descriptors.
    /// </summary>
    sealed class DescriptorPool
    {
        private static readonly IDictionary<string, MessageDescriptor> messages =
            new Dictionary<string, MessageDescriptor>();
        private static readonly IDictionary<string, MethodDescriptor> methods =
            new Dictionary<string, MethodDescriptor>();

        private static readonly HashSet<FileDescriptor> files =
            new HashSet<FileDescriptor>();

        /// <summary>
        /// Finds a message of the given name within the pool.
        /// </summary>
        /// <param name="fullName">Fully-qualified name to look up</param>
        /// <returns>The message with the given name, or null if doesn't exist</returns>
        public static MessageDescriptor FindMessage(string fullName)
        {
            MessageDescriptor result;
            messages.TryGetValue(fullName, out result);
            return result;
        }

        /// <summary>
        /// Finds a method of the given name within the pool.
        /// </summary>
        /// <param name="fullName">Fully-qualified name to look up</param>
        /// <returns>The method with the given name, or null if doesn't exist</returns>
        public static MethodDescriptor FindMethod(string fullName)
        {
            MethodDescriptor result;
            methods.TryGetValue(fullName, out result);
            return result;
        }

        /// <summary>
        /// Adds a file descriptor.
        /// </summary>
        public static void AddFileDescriptor(FileDescriptor descriptor)
        {
            ValidateSymbolName(descriptor);
            VerifyDependencies(descriptor);
            files.Add(descriptor);  // ignore old
            descriptor.Services.Select((s) => { AddServiceDescriptor(s); return 0; });
            descriptor.MessageTypes.Select((m) => { AddMessageDescriptor(m); return 0; });
        }

        /// <summary>
        /// Adds a service descriptor.
        /// </summary>
        public static void AddServiceDescriptor(ServiceDescriptor descriptor)
        {
            ValidateSymbolName(descriptor);
            descriptor.Methods.Select((m) => { AddMethodDescriptor(m); return 0; });
        }

        /// <summary>
        /// Adds a methos descriptor.
        /// </summary>
        public static void AddMethodDescriptor(MethodDescriptor descriptor)
        {
            ValidateSymbolName(descriptor);
            methods[descriptor.FullName] = descriptor;
        }

        /// <summary>
        /// Adds a message descriptor.
        /// </summary>
        public static void AddMessageDescriptor(MessageDescriptor descriptor)
        {
            ValidateSymbolName(descriptor);
            messages[descriptor.FullName] = descriptor;
        }

        private static readonly RegexOptions CompiledRegexWhereAvailable =
            Enum.IsDefined(typeof(RegexOptions), 8) ? (RegexOptions)8 : RegexOptions.None;
        private static readonly Regex ValidationRegex = new Regex("^[_A-Za-z][_A-Za-z0-9]*$",
                                                                  CompiledRegexWhereAvailable);

        /// <summary>
        /// Verifies that the descriptor's name is valid (i.e. it contains
        /// only letters, digits and underscores, and does not start with a digit).
        /// </summary>
        /// <param name="descriptor"></param>
        private static void ValidateSymbolName(IDescriptor descriptor)
        {
            if (descriptor.Name == "")
            {
                throw new Exception("Missing name.");
            }
            if (!ValidationRegex.IsMatch(descriptor.Name))
            {
                throw new Exception("\"" + descriptor.Name + "\" is not a valid identifier.");
            }
        }

        // Dependencies must be all here already.
        private static void VerifyDependencies(FileDescriptor file)
        {
            foreach (FileDescriptor dependency in file.Dependencies)
            {
                if (files.Add(dependency))
                {
                    throw new Exception("Dependency \"" + dependency.Name + "\" is not found.");
                }
            }
        }
    }  // class DescriptorPool
}  // namespace GrpcToLua

