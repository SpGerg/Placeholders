using Placeholders.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Placeholders.API.Features
{
    public static class PlaceholdersAPI
    {
        public static IReadOnlyList<IPlaceholderInfo> RegisterdPlaceholders => _registeredPlaceholders.ToList();

        private static readonly List<IPlaceholderInfo> _registeredPlaceholders = new List<IPlaceholderInfo>(); 

        public static void Register(IPlaceholder placeholder)
        {
            if (string.IsNullOrWhiteSpace(placeholder.Identifier))
            {
                throw new ArgumentException("Placeholders identifier is null or whitespace.");
            }

            if (_registeredPlaceholders.Contains(placeholder))
            {
                throw new ArgumentException("Placeholder is contains.");
            }

            if (_registeredPlaceholders.Any(identifier => identifier.Identifier == placeholder.Identifier))
            {
                throw new ArgumentException($"The placeholder named \"{placeholder.Name}\" already has this identifier");
            }

            _registeredPlaceholders.Add(placeholder);
        }

        public static void Unregister(IPlaceholder placeholder)
        {
            _registeredPlaceholders.Remove(placeholder);
        }

        public static string SetPlaceholders(string uuid, string message)
        {
            var result = message;
            var matches = Regex.Matches(result, @"{(\S*)}");

            foreach (Match match in matches)
            {
                var placeholderString = match.Value.Replace("{", "").Replace("}", "");
                var placeholder = _registeredPlaceholders.FirstOrDefault(_placeholder => placeholderString.StartsWith(_placeholder.Identifier));

                if (placeholder == default)
                {
                    continue;
                }

                var requested = string.Empty;

                if (placeholder is IPlaceholderHook placeholderHook)
                {
                    requested = placeholderHook.OnRequest(placeholderString.Replace($"{placeholder.Identifier}_", ""));
                }

                if (placeholder is IPlaceholderPlayerHook placeholderHookPlayer)
                {
                    requested = placeholderHookPlayer.OnRequest(uuid, placeholderString.Replace($"{placeholder.Identifier}_", ""));
                }

                result = result.Replace(match.Value, requested);
            }

            return result;
        }
    }
}
