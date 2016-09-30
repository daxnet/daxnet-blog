using DaxnetBlog.Common;
using System;

namespace DaxnetBlog.Domain.Model
{
    public class Account : IEntity<int>
    {

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string NickName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime? DateLastLogin { get; set; }

        public int Id { get; set; }

        public override string ToString() => UserName;

        /// <summary>
        /// Checks if the given password is valid.
        /// </summary>
        /// <param name="givenPassword">The given password.</param>
        /// <returns>True if the password is valid, otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when the required parameters are null.
        /// </exception>
        public bool ValidatePassword(string givenPassword)
        {
            if (string.IsNullOrEmpty(givenPassword))
            {
                throw new ArgumentNullException(nameof(givenPassword));
            }

            if (string.IsNullOrEmpty(this.UserName))
            {
                throw new ArgumentNullException(nameof(UserName));
            }

            return string.CompareOrdinal(this.PasswordHash, Crypto.ComputeHash(givenPassword, UserName)) == 0;
        }
    }
}
