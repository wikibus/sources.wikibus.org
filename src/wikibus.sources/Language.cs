﻿using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using NullGuard;

namespace Wikibus.Sources
{
    /// <summary>
    /// Represents a language
    /// </summary>
    [JsonConverter(typeof(LexvoIso639LanguageConverter))]
    public class Language
    {
        private readonly CultureInfo cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        public Language(string name)
            : this(new CultureInfo(name))
        {
        }

        private Language(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        /// <summary>
        /// Gets the language's name
        /// </summary>
        public string Name
        {
            get { return this.cultureInfo.Name; }
        }

        public bool IsValid => CultureInfo.GetCultures(CultureTypes.NeutralCultures).Contains(this.cultureInfo);

        public static bool operator ==([AllowNull] Language left, [AllowNull] Language right)
        {
            return Equals(left, right);
        }

        public static bool operator !=([AllowNull] Language left, [AllowNull] Language right)
        {
            return !Equals(left, right);
        }

        public override bool Equals([AllowNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Language)obj);
        }

        public override int GetHashCode()
        {
            return this.cultureInfo.GetHashCode();
        }

        protected bool Equals([AllowNull] Language other)
        {
            if (other == null)
            {
                return false;
            }

            return this.cultureInfo.Equals(other.cultureInfo);
        }
    }
}
